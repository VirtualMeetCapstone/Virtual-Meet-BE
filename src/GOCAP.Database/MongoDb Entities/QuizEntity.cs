using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOCAP.Database.MongoDb_Entities
{
    [BsonCollection("Quizzes")]
    public class QuizEntity : EntityMongoBase
    {   
        public Guid QuizId { get; set; }
        public List<Quiz> Quizzes { get; set; }
        public string UserId {  get; set; }
        public string Topic {  get; set; }
    }
    public class Quiz {
        public string Text { get; set; } = string.Empty;

        public List<string> Options { get; set; } = new();

        public int CorrectAnswer { get; set; }

        public int Points { get; set; }
    }


}
