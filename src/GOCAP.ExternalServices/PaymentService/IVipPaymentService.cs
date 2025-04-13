namespace GOCAP.ExternalServices;

public interface IVipPaymentService
{
    Task<CreatePaymentResult> CreateVipPaymentAsync(Guid userId, int packageId);
    Task<bool> MarkPaymentAsPaidAsync(string orderCode);
    Task<QueryResult<PaymentHistoryModel>> GetAllPaymentsAsync(QueryInfo queryInfo);
    Task<QueryResult<PaymentHistoryModel>> GetPaymentsByUserIdAsync(Guid userId, QueryInfo queryInfo);
}

