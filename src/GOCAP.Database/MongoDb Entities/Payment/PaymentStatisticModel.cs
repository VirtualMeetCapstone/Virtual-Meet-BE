namespace GOCAP.Database;

public class PaymentStatisticModel
{
    public decimal TotalIncome { get; set; }
    public int TotalPaidOrders { get; set; }
    public int TotalCancelledOrders { get; set; }
    public int TotalOrders { get; set; }
}
