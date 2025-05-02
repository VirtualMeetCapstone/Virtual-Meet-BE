using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using GOCAP.Api.Hubs;
using GOCAP.Database;
using GOCAP.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using System.ComponentModel.DataAnnotations;

namespace GOCAP.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesOutsideRoomController : ControllerBase
    {
        private readonly AppMongoDbContext _dbContext;

        public MessagesOutsideRoomController(AppMongoDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var message = new MessagesOutsideRoomEntity
            {
                Content = request.Content,
                Sender = new UserInfo
                {
                    Id = request.SenderId,
                    Name = request.SenderName,
                    ImageUrl = request.SenderImageUrl
                },
                ConversationType = request.ConversationType,
                MessageType = request.MessageType,
                AttachmentUrls = request.AttachmentUrls ?? new List<string>(),
                CreatedAt = DateTime.UtcNow
            };

            if (request.ConversationType=="Private")
            {
                message.ConversationId = GetPrivateConversationId(request.SenderId, request.ReceiverId);
                message.Receiver = new UserInfo
                {
                    Id = request.ReceiverId,
                    Name = request.ReceiverName,
                    ImageUrl = request.ReceiverImageUrl
                };
            }
            await _dbContext.MessagesOutsideRoom.InsertOneAsync(message);
            return Ok(message);
        }

        [HttpGet("private/{userId1}/{userId2}")]
        public async Task<IActionResult> GetPrivateConversation(string userId1, string userId2, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            var conversationId = GetPrivateConversationId(userId1, userId2);
            var messages = await _dbContext.MessagesOutsideRoom
                         .Find(m => m.ConversationId == conversationId && !m.IsDeleted)
                         .SortByDescending(m => m.CreatedAt)
                         .Skip((page - 1) * pageSize)
                         .Limit(pageSize)
                         .ToListAsync();

            return Ok(messages.OrderBy(m => m.CreatedAt));
        }

        [HttpDelete("{messageId}")]
        public async Task<IActionResult> DeleteMessage(string messageId)
        {
            var update = Builders<MessagesOutsideRoomEntity>.Update
                .Set(m => m.IsDeleted, true)
                .Set(m => m.UpdatedAt, DateTime.UtcNow);

            var result = await _dbContext.MessagesOutsideRoom
                .UpdateOneAsync(m => m.Id == messageId, update);

            if (result.ModifiedCount == 0)
            {
                return NotFound();
            }

            var message = await _dbContext.MessagesOutsideRoom
                .Find(m => m.Id == messageId)
                .FirstOrDefaultAsync();


            return Ok(new { success = true });
        }




        [HttpPost("mark-last-message-as-read")]
        public async Task<IActionResult> MarkLastMessageAsRead([FromBody] MarkAsReadRequest request)
        {
            // Tìm tin nhắn cuối cùng của cuộc trò chuyện
            var message = await _dbContext.MessagesOutsideRoom
                 .Find(m =>
                 !m.IsDeleted && (request.SenderId==m.Receiver.Id)&&(request.ReceiverId==m.Sender.Id)).SortByDescending(m=>m.CreatedAt).FirstOrDefaultAsync();
            message.ReadStatuses.Add(
                new ReadStatus
                {
                    UserId = request.SenderId,
                    IsRead = true,
                    ReadAt = DateTime.UtcNow,
                });
            await _dbContext.MessagesOutsideRoom.ReplaceOneAsync(m => m.Id == message.Id, message);

            return Ok(message);        

        }

        private string GetPrivateConversationId(string userId1, string userId2)
        {
            var ids = new[] { userId1, userId2 }.OrderBy(id => id).ToArray();
            return $"private_{ids[0]}_{ids[1]}";
        }

        [HttpGet("chat-history/{userId}")]
        public async Task<IActionResult> GetChatHistory(string userId)
        {
            var messages = await _dbContext.MessagesOutsideRoom
                .Find(m =>
                    !m.IsDeleted &&
                    (m.Sender.Id == userId || (m.Receiver != null && m.Receiver.Id == userId)))
                .SortByDescending(m => m.CreatedAt)
                .ToListAsync();

            var recentContacts = messages
                .Select(m =>
                {
                    bool isRead = false;

                    if (m.ReadStatuses != null)
                    {
                        var readStatus = m.ReadStatuses.FirstOrDefault(r => r.UserId.Trim().ToLower() == userId.Trim().ToLower());
                        if (readStatus != null)
                        {
                            isRead = readStatus.IsRead;
                        }
                    }
                    
                    var isYou = m.Sender.Id == userId;
                    var contact = m.Sender.Id == userId ? m.Receiver : m.Sender;
                    return new
                    { 
                        ContactId = contact?.Id,
                        ContactName = contact?.Name,
                        ContactImage = contact?.ImageUrl,
                        LastMessage = m.Content,
                        LastMessageTime = m.CreatedAt,
                        IsRead = isRead,
                        IsYou = isYou,
                    };
                })
                .Where(x => x.ContactId != null)
                .GroupBy(x => x.ContactId)
                .Select(g => g.OrderByDescending(m => m.LastMessageTime).First()) // Lấy tin nhắn mới nhất với mỗi contact
                .OrderByDescending(x => x.LastMessageTime)
                .ToList();

            return Ok(recentContacts);
        }


    }


    public class SendMessageRequest
    {
        [Required]
        public string Content { get; set; }

        [Required]
        public string SenderId { get; set; }

        [Required]
        public string SenderName { get; set; }

        public string SenderImageUrl { get; set; }

        [Required]
        public string ConversationType { get; set; }

        // For private chat
        public string ReceiverId { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverImageUrl { get; set; }

        // For group chat
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public List<UserInfo> GroupMembers { get; set; }

        public string ReplyToMessageId { get; set; }
        public string MessageType { get; set; } = "text";
        public List<string> AttachmentUrls { get; set; }
    }

    public class EditMessageRequest
    {
        [Required]
        public string NewContent { get; set; }
    }

    public class ReactionRequest
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Emoji { get; set; }
    }

    public class MarkAsReadRequest
    {
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
    }


}