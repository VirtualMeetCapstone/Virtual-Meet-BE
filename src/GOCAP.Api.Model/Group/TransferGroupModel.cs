namespace GOCAP.Api.Model;

public class TransferGroupModel
{
    public Guid GroupId { get; set; }
    public Guid CurrentOwnerId { get; set; }
    public Guid NewOwnerId { get; set; }
}
