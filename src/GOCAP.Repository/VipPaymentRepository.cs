
namespace GOCAP.Repository;

[RegisterService(typeof(IVipPaymentRepository))]
internal class VipPaymentRepository(AppMongoDbContext _context)
    : MongoRepositoryBase<UserVip>(_context), IVipPaymentRepository
{
    private readonly AppMongoDbContext _context = _context;
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

    public async Task<QueryResult<PaymentHistoryModel>> GetAllPaymentsAsync(QueryInfo queryInfo)
    {
        var builder = Builders<PaymentHistory>.Filter;
        var filter = builder.Empty;

        if (!string.IsNullOrEmpty(queryInfo.SearchText))
        {
            var filters = new List<FilterDefinition<PaymentHistory>>
        {
            builder.Regex(p => p.Level, new BsonRegularExpression(queryInfo.SearchText, "i"))
        };

            if (Guid.TryParse(queryInfo.SearchText, out var guid))
            {
                filters.Add(builder.Eq(p => p.UserId, guid));
            }

            filter = builder.Or(filters);
        }

        var totalItems = queryInfo.NeedTotalCount
            ? (int)await _context.PaymentHistorys.CountDocumentsAsync(filter)
            : 0;

        var paymentList = await _context.PaymentHistorys
            .Find(filter)
            .Skip(queryInfo.Skip)
            .Limit(queryInfo.Top)
            .ToListAsync();

        var userIds = paymentList.Select(p => p.UserId).Distinct();
        var packageIds = paymentList.Select(p => p.PackageId).Distinct();

        var vipList = await _context.UserVips
      .Find(Builders<UserVip>.Filter.In(x => x.UserId, userIds.Cast<Guid?>()))
      .ToListAsync();


        var result = paymentList.Select(payment =>
        {
            var matchedVip = vipList.FirstOrDefault(v =>
                v.UserId == payment.UserId && v.PackageId == payment.PackageId);

            return new PaymentHistoryModel
            {
                Level = payment.Level,
                PackageId = payment.PackageId,
                OrderCode = payment.OrderCode,
                IsPaid = payment.IsPaid,
                Amount = payment.Amount,
                CreateTime = payment.CreateTime > 0
    ? new DateTime((long)payment.CreateTime, DateTimeKind.Utc)
    : (DateTime?)null,

                ExpireAt = matchedVip?.ExpireAt
            };
        }).ToList();

        return new QueryResult<PaymentHistoryModel>
        {
            Data = result,
            TotalCount = totalItems
        };
    }


    public async Task<QueryResult<PaymentHistoryModel>> GetPaymentsByUserIdAsync(Guid userId, QueryInfo queryInfo)
    {
        var builder = Builders<PaymentHistory>.Filter;
        var filter = builder.Eq(p => p.UserId, userId);

        if (!string.IsNullOrEmpty(queryInfo.SearchText))
        {
            var filters = new List<FilterDefinition<PaymentHistory>>();

            filters.Add(builder.Regex(p => p.Level, new BsonRegularExpression(queryInfo.SearchText, "i")));

            if (Guid.TryParse(queryInfo.SearchText, out var guid))
            {
                filters.Add(builder.Eq(p => p.UserId, guid));
            }

            filter = builder.And(filter, builder.Or(filters));
        }

        var totalItems = queryInfo.NeedTotalCount
            ? (int)await _context.PaymentHistorys.CountDocumentsAsync(filter)
            : 0;

        var items = await _context.PaymentHistorys
            .Find(filter)
            .Skip(queryInfo.Skip)
            .Limit(queryInfo.Top)
            .ToListAsync();

        // Lấy danh sách userVip liên quan
        var userVipList = await _context.UserVips
            .Find(uv => uv.UserId == userId)
            .ToListAsync();

        // Mapping sang model và include thêm ExpireAt từ bảng UserVip
        var result = items.Select(payment =>
        {
            DateTime? createTime = null;

            try
            {
                // Xử lý nếu CreateTime là ticks (quá lớn để là Unix milliseconds)
                if (payment.CreateTime > DateTime.MinValue.Ticks && payment.CreateTime <= DateTime.MaxValue.Ticks)
                {
                    createTime = new DateTime(payment.CreateTime, DateTimeKind.Utc);
                }
                // Xử lý nếu CreateTime là UnixTimeMilliseconds
                else if (payment.CreateTime >= 0 && payment.CreateTime <= 253402300799999)
                {
                    createTime = DateTimeOffset.FromUnixTimeMilliseconds(payment.CreateTime).UtcDateTime;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LỖI] Không thể parse CreateTime: {payment.CreateTime} - {ex.Message}");
            }

            DateTime? expireAt = null;
            if (payment.IsPaid)
            {
                var matchedVip = userVipList.FirstOrDefault(uv => uv.PackageId == payment.PackageId);
                expireAt = matchedVip?.ExpireAt;
            }

            return new PaymentHistoryModel
            {
                Level = payment.Level,
                PackageId = payment.PackageId,
                OrderCode = payment.OrderCode,
                IsPaid = payment.IsPaid,
                Amount = payment.Amount,
                CreateTime = createTime,
                ExpireAt = expireAt
            };
        }).ToList();


        return new QueryResult<PaymentHistoryModel>
        {
            Data = result,
            TotalCount = totalItems
        };
    }




    public async Task<QueryResult<PaymentStatisticModel>> GetPaymentStatisticsAsync(DateTime startDate, DateTime endDate)
    {
        var builder = Builders<PaymentHistory>.Filter;

        var timeFilter = builder.And(
            builder.Gte(p => p.CreateTime, startDate.Ticks),
            builder.Lte(p => p.CreateTime, endDate.Ticks)
        );

        var paymentList = await _context.PaymentHistorys
            .Find(timeFilter)
            .ToListAsync();

        var totalIncome = paymentList
            .Where(p => p.IsPaid)
            .Sum(p => p.Amount);

        var totalPaidOrders = paymentList.Count(p => p.IsPaid);
        var totalCancelledOrders = paymentList.Count(p => !p.IsPaid);
        var totalOrders = paymentList.Count();

        var statistic = new PaymentStatisticModel
        {
            TotalIncome = totalIncome,
            TotalPaidOrders = totalPaidOrders,
            TotalCancelledOrders = totalCancelledOrders,
            TotalOrders = totalOrders
        };

        return new QueryResult<PaymentStatisticModel>
        {
            Data = new List<PaymentStatisticModel> { statistic },
            TotalCount = 1
        };
    }




}
