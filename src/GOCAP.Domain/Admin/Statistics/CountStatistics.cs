namespace GOCAP.Domain;

public class CountStatistics
{
    //User
    public int UserTotal { get; set; }
    public int UserActive { get; set; }
    public int UserInActive { get; set; }
    public int UserDeleted { get; set; }
    public int UserBanned { get; set; }

    //Room
    public int RoomTotal { get; set; }
    public int RoomAvailable { get; set; }
    public int RoomOccupied { get; set; }
    public int RoomReserved { get; set; }
    public int RoomOutOfService { get; set; }

    //Group
    public int GroupTotal { get; set; }
}
