
namespace GOCAP.ExternalServices;

[RegisterService(typeof(VipPaymentService), ServiceLifetime.Scoped)]
public class VipPaymentService : IVipPaymentService
{
    private readonly PayOsSettings _payOsSettings;
    private readonly PayOS _payOS;
    private readonly string _domain;

    private static readonly List<VipPackageItem> AvailablePackages = new()
    {
        new VipPackageItem { Id = 1, Name = "VIP 1 ngày", Price = 10000, DurationDays = 1 },
        new VipPackageItem { Id = 2, Name = "VIP 1 tuần", Price = 50000, DurationDays = 7 },
        new VipPackageItem { Id = 3, Name = "VIP 1 tháng", Price = 100000, DurationDays = 30 },
    };

    public VipPaymentService(PayOsSettings payOsSettings, PayOS payOS)
    {
        _payOsSettings = payOsSettings;
        _payOS = payOS;
        _domain = "https://fe.dev-vmeet.site/up-vip";
    }

    public async Task<CreatePaymentResult> CreateVipPaymentAsync(int packageId)
    {
        var package = AvailablePackages.FirstOrDefault(p => p.Id == packageId);
        if (package == null)
            throw new ArgumentException("Gói VIP không hợp lệ.");

        long orderCode = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var orderId = Guid.NewGuid();

        var paymentData = new PaymentData(
            orderCode: orderCode,
            amount: package.Price,
            description: package.Name.Length > 25 ? package.Name[..25] : package.Name,
            items: new List<ItemData> {
            new(package.Name, 1, package.Price)
            },
            cancelUrl: $"{_domain}?status=failed&orderId={orderId}",
            returnUrl: $"{_domain}?status=success&orderId={orderId}&totalAmount={package.Price}"
        );

        return await _payOS.createPaymentLink(paymentData);
    }

}
