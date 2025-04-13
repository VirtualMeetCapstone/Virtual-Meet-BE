namespace GOCAP.Repository.Intention
{
    public interface IVipPaymentRepository
    {
        Task<PaymentHistory?> GetPaymentByUserAndPackageIdAsync(Guid userId, int packageId);
        Task AddPaymentAsync(PaymentHistory payment);
        Task<PaymentHistory?> GetByOrderCodeAsync(string orderCode);
        Task UpdatePaymentAsync(PaymentHistory payment);
        Task<QueryResult<PaymentHistoryModel>> GetAllPaymentsAsync(QueryInfo queryInfo);
        Task<QueryResult<PaymentHistoryModel>> GetPaymentsByUserIdAsync(Guid userId, QueryInfo queryInfo);
    }
}
