namespace GOCAP.Domain;

public class TransferGroup
{
    public Guid GroupId { get; set; }
    public Guid CurrentOwnerId { get; set; }
    public Guid NewOwnerId { get; set; }
}
