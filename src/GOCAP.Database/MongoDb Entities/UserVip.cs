
namespace GOCAP.Database
{
    public class UserVip : EntityMongoBase
    {
        public Guid? UserId { get; set; }
        public int PackageId { get; set; } 
        public DateTime? ExpireAt { get; set; } 
    }
}
