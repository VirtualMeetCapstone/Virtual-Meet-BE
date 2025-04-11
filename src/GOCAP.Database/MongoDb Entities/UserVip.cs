
namespace GOCAP.Database
{
    public class UserVip : EntityMongoBase
    {
        public Guid? UserId { get; set; }
        public string Level { get; set; } = "free"; 
        public DateTime? ExpireAt { get; set; } 
    }
}
