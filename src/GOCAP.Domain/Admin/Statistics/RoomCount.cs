namespace GOCAP.Domain;
public class RoomCount
{
    public int Total { get; set; }
    public int Available { get; set; }
    public int Occupied { get; set; }
    public int Reserved { get; set; }
    public int OutOfService { get; set; }
}