
using GOCAP.Repository.Intention;

namespace GOCAP.ExternalServices;

[RegisterService(typeof(VipPaymentService), ServiceLifetime.Scoped)]
public class VipPaymentService : IVipPaymentService
{
    private readonly PayOS _payOS;
    private readonly string _domain;
    private readonly IVipPaymentRepository _vipPaymentRepository;

    private static readonly List<VipPackageItem> AvailablePackages = new()
    {
        new VipPackageItem { Id = 1, Name = "VIP 1 ngày", Price = 10000, DurationDays = 1 },
        new VipPackageItem { Id = 2, Name = "VIP 1 tuần", Price = 50000, DurationDays = 7 },
        new VipPackageItem { Id = 3, Name = "VIP 1 tháng", Price = 100000, DurationDays = 30 },
    };

    public VipPaymentService(PayOsSettings payOsSettings, PayOS payOS, IVipPaymentRepository vipPaymentRepository)
    {
        _payOS = payOS;
        _domain = "https://fe.dev-vmeet.site/up-vip";
        _vipPaymentRepository = vipPaymentRepository;
    }

    public async Task<CreatePaymentResult> CreateVipPaymentAsync(Guid userId, int packageId)
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
            returnUrl: $"{_domain}?status=success&orderId={orderId}&totalAmount={package.Price}&packageId={packageId}"
        );

        var paymentResult = await _payOS.createPaymentLink(paymentData);

        var paymentHistory = new PaymentHistory
        {
            UserId = userId,
            Level = package.Name,
            PackageId = package.Id,
            Amount = package.Price,
            OrderCode = orderCode.ToString(),
            IsPaid = false,
        };

        await _vipPaymentRepository.AddPaymentAsync(paymentHistory);

        return paymentResult;
    }


    public async Task<bool> MarkPaymentAsPaidAsync(string orderCode)
    {
        var payment = await _vipPaymentRepository.GetByOrderCodeAsync(orderCode);
        if (payment == null || payment.IsPaid) return false;

        var status = await _payOS.getPaymentLinkInformation(long.Parse(orderCode));

        if (status == null || status.status != "PAID")
            return false;

    
        payment.IsPaid = true;
        await _vipPaymentRepository.UpdatePaymentAsync(payment);
        return true;
    }

    public async Task<QueryResult<PaymentHistoryModel>> GetAllPaymentsAsync(QueryInfo queryInfo)
    {
        return await _vipPaymentRepository.GetAllPaymentsAsync(queryInfo);
    }

    public async Task<QueryResult<PaymentHistoryModel>> GetPaymentsByUserIdAsync(Guid userId, QueryInfo queryInfo)
    {
        return await _vipPaymentRepository.GetPaymentsByUserIdAsync(userId, queryInfo);
    }

}
