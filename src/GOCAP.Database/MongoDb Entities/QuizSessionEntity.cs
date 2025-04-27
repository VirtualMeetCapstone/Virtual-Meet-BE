using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOCAP.Database.MongoDb_Entities
{

    [BsonCollection("QuizSessions")]
    public class QuizSessionEntity : EntityMongoBase
    {
        public Guid QuizSessionId { get; set; } 

        public List<QuizEntity> Quizzes { get; set; } = new(); 

        public List<Player> Players { get; set; } = new(); 
    }
    public class Player
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
    }

}
