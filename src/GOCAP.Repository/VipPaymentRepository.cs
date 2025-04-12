namespace GOCAP.Repository;

[RegisterService(typeof(IVipPaymentRepository))]
internal class VipPaymentRepository(AppMongoDbContext _context)
        : MongoRepositoryBase<UserVip>(_context), IVipPaymentRepository
{
    public async Task<PaymentHistory?> GetPaymentByUserAndPackageIdAsync(Guid userId, int packageId)
    {
        return await _context.PaymentHistorys
            .Find(p => p.UserId == userId && p.PackageId == packageId && p.IsPaid)
            .FirstOrDefaultAsync();
    }

    public async Task AddPaymentAsync(PaymentHistory payment)
    {
        await _context.PaymentHistorys.InsertOneAsync(payment);
    }

    public async Task<PaymentHistory?> GetByOrderCodeAsync(string orderCode)
    {
        return await _context.PaymentHistorys
            .Find(p => p.OrderCode == orderCode)
            .FirstOrDefaultAsync();
    }


    public async Task UpdatePaymentAsync(PaymentHistory payment)
    {
        var filter = Builders<PaymentHistory>.Filter.Eq(p => p.Id, payment.Id);
        await _context.PaymentHistorys.ReplaceOneAsync(filter, payment);
    }


}
