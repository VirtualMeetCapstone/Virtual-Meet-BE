namespace GOCAP.Api.Model;

public class GroupDetailModel
{
    public GroupOwnerModel? Owner { get; set; }
    public List<GroupOwnerModel> Members { get; set; } = [];
    public long CreateTime { get; set; }
    public long LastModifyTime { get; set; }
}




