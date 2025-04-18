namespace GOCAP.Database
{
    public class PaymentHistoryModel
    {
        public string Level { get; set; }
        public int PackageId { get; set; }
        public string OrderCode { get; set; }
        public bool IsPaid { get; set; }
        public int Amount { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? ExpireAt { get; set; }
    }
}
