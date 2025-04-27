using GOCAP.Database;
using GOCAP.Database.MongoDb_Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Numerics;

namespace GOCAP.Api.Controllers
{
    public class QuizController : Controller
    {

        private readonly AppMongoDbContext _dbContext;

        public QuizController(AppMongoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var quiz = await _dbContext.Quizzes.Find(q => q.Id == id).FirstOrDefaultAsync();
            if (quiz == null)
                return NotFound();
            return Ok(quiz);
        }

        [HttpPost("addListQuiz")]
        public async Task<IActionResult> Create([FromBody] QuizEntity quiz)
        {   
            quiz.QuizId = Guid.NewGuid();

            await _dbContext.Quizzes.InsertOneAsync(quiz);
            return Ok(quiz);
        }
        [HttpGet("getQuizById/{id}")]
        public async Task<IActionResult> GetQuizById(Guid id)
        {
            var quiz = await _dbContext.Quizzes.Find(q => q.QuizId == id).FirstOrDefaultAsync();

            if (quiz == null)
            {
                return NotFound("Quiz not found.");
            }

            return Ok(quiz); 
        }
        [HttpDelete("deleteQuiz/{id}")]
        public async Task<IActionResult> DeleteQuiz(Guid id)
        {
            var result = await _dbContext.Quizzes.DeleteOneAsync(q => q.QuizId == id);

            if (result.DeletedCount == 0)
            {
                return NotFound("Quiz not found.");
            }

            return Ok("Quiz deleted successfully.");
        }
        [HttpPut("updateQuiz/{id}")]
        public async Task<IActionResult> UpdateQuiz(Guid id, [FromBody] QuizEntity updatedQuiz)
        {
            // Lấy quiz cũ ra
            var existingQuiz = await _dbContext.Quizzes.Find(q => q.QuizId == id).FirstOrDefaultAsync();
            if (existingQuiz == null)
            {
                return NotFound("Quiz not found.");
            }

            // Giữ nguyên _id
            updatedQuiz.Id = existingQuiz.Id;
            updatedQuiz.QuizId = id; // giữ đúng QuizId nữa

            var result = await _dbContext.Quizzes.ReplaceOneAsync(q => q.QuizId == id, updatedQuiz);

            return Ok("Quiz updated successfully.");
        }


        [HttpGet("getlistQuizOfUser/{id}")]
        public async Task<IActionResult> GetListQuiz(string id)
        {
            var quiz = await _dbContext.Quizzes.Find(q => q.UserId == id).ToListAsync();

            if (quiz == null)
            {
                return NotFound("Quiz not found.");
            }

            return Ok(quiz);
        }

        [HttpPost("addRoomSession")]
        public async Task<IActionResult> AddRoomSession([FromBody] List<QuizEntity> quizzes)
        {
            QuizSessionEntity quizSessionEntity = new QuizSessionEntity{ QuizSessionId= Guid.NewGuid() ,
                Players = [],
                Quizzes = quizzes,
            };
            await _dbContext.QuizSessions.InsertOneAsync(quizSessionEntity);

            return Ok(quizSessionEntity);
        }
        [HttpPost("addPlayer")]
        public async Task<IActionResult> AddPlayer([FromBody] AddPlayerRequest request)
        {

            var quizSession = await _dbContext.QuizSessions
                .Find(x => x.QuizSessionId == request.QuizSessionId)
                .FirstOrDefaultAsync();
            if(quizSession.Players.FirstOrDefault(p=>p.Id==request.Player.Id)==null)
            {
                request.Player.Score = 0;
                quizSession.Players.Add(request.Player);

                await _dbContext.QuizSessions.ReplaceOneAsync(
                    x => x.QuizSessionId == request.QuizSessionId,
                    quizSession);

                return Ok(quizSession);
            }
            else
            {
                return Ok();
            }
         
        }
        [HttpPost("addScore")]
        public async Task<IActionResult> AddScore([FromBody] AddScoreRequest request)
        {
            var quizSession = await _dbContext.QuizSessions
                .Find(x => x.QuizSessionId == request.QuizSessionId)
                .FirstOrDefaultAsync();

            if (quizSession == null)
                return NotFound();

            var player = quizSession.Players.FirstOrDefault(p => p.Id == request.PlayerId);
            if (player == null)
                return NotFound();

            player.Score += request.ScoreToAdd;

            await _dbContext.QuizSessions.ReplaceOneAsync(
                x => x.QuizSessionId == request.QuizSessionId,
                quizSession);

            return Ok(player);
        }


        [HttpGet("getRoomSession/{id}")]
        public async Task<IActionResult> GetRoomSessionById(Guid id)
        {
            var quiz = await _dbContext.QuizSessions.Find(q => q.QuizSessionId == id).FirstOrDefaultAsync();

            return Ok(quiz);
        }
        [HttpGet("getPlayerByRoomSessionId/{id}")]
        public async Task<IActionResult> GetPlayerByRoomsessionId(Guid id)
        {
            var quiz = await _dbContext.QuizSessions.Find(q => q.QuizSessionId == id).FirstOrDefaultAsync();
            var res = quiz.Players.OrderByDescending(x => x.Score).ToList();

            return Ok(res);
        }



    }
    public class AddPlayerRequest
    {
        public Guid QuizSessionId { get; set; }  // ID của quiz session
        public Player Player { get; set; }  // Thông tin người chơi
    }
    public class AddScoreRequest
    {
        public Guid QuizSessionId { get; set; }
        public string PlayerId { get; set; }
        public int ScoreToAdd { get; set; }
    }


}