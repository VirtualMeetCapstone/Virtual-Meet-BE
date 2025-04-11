namespace GOCAP.ExternalServices;

public interface IVipPaymentService
{
    Task<CreatePaymentResult> CreateVipPaymentAsync(int packageId);

}
