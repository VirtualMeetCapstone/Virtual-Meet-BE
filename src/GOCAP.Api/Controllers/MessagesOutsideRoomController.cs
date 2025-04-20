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

        #region Message Operations

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
            else // Group chat
            {
                message.ConversationId = request.GroupId;
                message.GroupId = request.GroupId;
                message.GroupName = request.GroupName;
                message.GroupMembers = request.GroupMembers ?? new List<UserInfo>();
            }

            if (!string.IsNullOrEmpty(request.ReplyToMessageId))
            {
                message.ReplyToMessageId = request.ReplyToMessageId;
                var repliedMessage = await _dbContext.MessagesOutsideRoom
                    .Find(m => m.Id == request.ReplyToMessageId)
                    .FirstOrDefaultAsync();
                message.ReplyMessage = repliedMessage;
            }

            await _dbContext.MessagesOutsideRoom.InsertOneAsync(message);

   
         

            return Ok(message);
        }

        [HttpGet("conversation/{conversationId}")]
        public async Task<IActionResult> GetConversationMessages(string conversationId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            var messages = await _dbContext.MessagesOutsideRoom
                .Find(m => m.ConversationId == conversationId && !m.IsDeleted)
                .SortByDescending(m => m.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return Ok(messages.OrderBy(m => m.CreatedAt));
        }

        [HttpGet("private/{userId1}/{userId2}")]
        public async Task<IActionResult> GetPrivateConversation(string userId1, string userId2, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            var conversationId = GetPrivateConversationId(userId1, userId2);
            return await GetConversationMessages(conversationId, page, pageSize);
        }

        [HttpGet("group/{groupId}")]
        public async Task<IActionResult> GetGroupMessages(string groupId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            return await GetConversationMessages(groupId, page, pageSize);
        }

        [HttpPut("{messageId}/edit")]
        public async Task<IActionResult> EditMessage(string messageId, [FromBody] EditMessageRequest request)
        {
            var update = Builders<MessagesOutsideRoomEntity>.Update
                .Set(m => m.Content, request.NewContent)
                .Set(m => m.EditedAt, DateTime.UtcNow)
                .Set(m => m.UpdatedAt, DateTime.UtcNow);

            var result = await _dbContext.MessagesOutsideRoom
                .UpdateOneAsync(m => m.Id == messageId, update);

            if (result.ModifiedCount == 0)
            {
                return NotFound();
            }

            var updatedMessage = await _dbContext.MessagesOutsideRoom
                .Find(m => m.Id == messageId)
                .FirstOrDefaultAsync();

            // Notify recipients

            return Ok(updatedMessage);
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

        #endregion

        #region Reaction Operations

        [HttpPost("{messageId}/reactions")]
        public async Task<IActionResult> AddReaction(string messageId, [FromBody] ReactionRequest request)
        {
            var message = await _dbContext.MessagesOutsideRoom
                .Find(m => m.Id == messageId)
                .FirstOrDefaultAsync();

            if (message == null)
            {
                return NotFound();
            }

            // Remove existing reaction from this user if exists
            message.Reactions.RemoveAll(r => r.UserId == request.UserId);

            // Add new reaction
            message.Reactions.Add(new ReactionInfo
            {
                UserId = request.UserId,
                Emoji = request.Emoji
            });

            message.UpdatedAt = DateTime.UtcNow;

            await _dbContext.MessagesOutsideRoom.ReplaceOneAsync(m => m.Id == messageId, message);

           
            return Ok(message);
        }

        [HttpDelete("{messageId}/reactions/{userId}")]
        public async Task<IActionResult> RemoveReaction(string messageId, string userId)
        {
            var message = await _dbContext.MessagesOutsideRoom
                .Find(m => m.Id == messageId)
                .FirstOrDefaultAsync();

            if (message == null)
            {
                return NotFound();
            }

            var removedCount = message.Reactions.RemoveAll(r => r.UserId == userId);

            if (removedCount > 0)
            {
                message.UpdatedAt = DateTime.UtcNow;
                await _dbContext.MessagesOutsideRoom.ReplaceOneAsync(m => m.Id == messageId, message);

                // Notify recipients
            }

            return Ok(message);
        }

        #endregion

        #region Read Status Operations

        [HttpPost("{messageId}/mark-as-read")]
        public async Task<IActionResult> MarkAsRead(string messageId, [FromBody] MarkAsReadRequest request)
        {
            var message = await _dbContext.MessagesOutsideRoom
                .Find(m => m.Id == messageId)
                .FirstOrDefaultAsync();

            if (message == null)
            {
                return NotFound();
            }

            var existingStatus = message.ReadStatuses.FirstOrDefault(s => s.UserId == request.UserId);
            if (existingStatus != null)
            {
                existingStatus.IsRead = true;
                existingStatus.ReadAt = DateTime.UtcNow;
            }
            else
            {
                message.ReadStatuses.Add(new ReadStatus
                {
                    UserId = request.UserId,
                    IsRead = true,
                    ReadAt = DateTime.UtcNow
                });
            }

            message.UpdatedAt = DateTime.UtcNow;
            await _dbContext.MessagesOutsideRoom.ReplaceOneAsync(m => m.Id == messageId, message);

            

            return Ok(message);
        }

        #endregion

        #region Helper Methods

        private string GetPrivateConversationId(string userId1, string userId2)
        {
            var ids = new[] { userId1, userId2 }.OrderBy(id => id).ToArray();
            return $"private_{ids[0]}_{ids[1]}";
        }

        [HttpGet("chat-history/{userId}")]
        public async Task<IActionResult> GetChatHistory(string userId)
        {
            // Lấy tất cả tin nhắn có liên quan đến user
            var messages = await _dbContext.MessagesOutsideRoom
                .Find(m =>
                    !m.IsDeleted &&
                    (m.Sender.Id == userId || (m.Receiver != null && m.Receiver.Id == userId)))
                .SortByDescending(m => m.CreatedAt)
                .ToListAsync();

            // Nhóm tin nhắn theo người còn lại (người đối thoại)
            var recentContacts = messages
                .Select(m =>
                {
                    var contact = m.Sender.Id == userId ? m.Receiver : m.Sender;
                    return new
                    {
                        ContactId = contact?.Id,
                        ContactName = contact?.Name,
                        ContactImage = contact?.ImageUrl,
                        LastMessage = m.Content,
                        LastMessageTime = m.CreatedAt
                    };
                })
                .Where(x => x.ContactId != null)
                .GroupBy(x => x.ContactId)
                .Select(g => g.OrderByDescending(m => m.LastMessageTime).First()) // Lấy tin nhắn mới nhất với mỗi contact
                .OrderByDescending(x => x.LastMessageTime)
                .ToList();

            return Ok(recentContacts);
        }





        #endregion
    }

    #region Request Models

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
        [Required]
        public string UserId { get; set; }
    }

    #endregion
}