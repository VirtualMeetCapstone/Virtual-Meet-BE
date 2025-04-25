namespace GOCAP.Repository
{
    [RegisterService(typeof(IRoomStatisticsRepository))]
    internal class RoomStatisticsRepository(AppMongoDbContext context)
    : MongoRepositoryBase<RoomStatisticsEntity>(context), IRoomStatisticsRepository
    {
        private readonly AppMongoDbContext _context = context;
        public async Task<QueryResult<RoomStatisticsEntity>> GetWithPagingAsync(QueryInfo queryInfo)
        {
            var filter = Builders<RoomStatisticsEntity>.Filter.Gt(c => c.TotalJoins, 0);

            if (!string.IsNullOrEmpty(queryInfo.SearchText))
            {
    
                var textFilter = Builders<RoomStatisticsEntity>.Filter.Regex(c => c.RoomId, new BsonRegularExpression(queryInfo.SearchText, "i"));
                filter = Builders<RoomStatisticsEntity>.Filter.And(filter, textFilter);
            }

            var orderByField = !string.IsNullOrEmpty(queryInfo.OrderBy) ? queryInfo.OrderBy : nameof(CommentEntity.CreateTime);
            var sortDefinition = queryInfo.OrderType == OrderType.Ascending
                ? Builders<RoomStatisticsEntity>.Sort.Ascending(orderByField)
                : Builders<RoomStatisticsEntity>.Sort.Descending(orderByField);

            long totalCount = queryInfo.NeedTotalCount ? await _context.RoomStatistic.CountDocumentsAsync(filter) : 0;

          
            var data = await _context.RoomStatistic.Find(filter)
                                        .Sort(sortDefinition)
                                        .Skip(queryInfo.Skip)
                                        .Limit(queryInfo.Top)
                                        .ToListAsync();

            return new QueryResult<RoomStatisticsEntity>
            {
                Data = data,
                TotalCount = (int)totalCount
            };
        }



        public async Task<RoomStatisticsEntity> GetByRoomIdAsync(string roomId)
        {
            var filter = Builders<RoomStatisticsEntity>.Filter.And(
                Builders<RoomStatisticsEntity>.Filter.Eq(c => c.RoomId, roomId),
                Builders<RoomStatisticsEntity>.Filter.Gt(c => c.TotalJoins, 0)
            );

            return await _context.RoomStatistic.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<RoomStatisticsEntity>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var filter = Builders<RoomStatisticsEntity>.Filter.And(
               Builders<RoomStatisticsEntity>.Filter.Gte(c => c.StartTime, startDate),
               Builders<RoomStatisticsEntity>.Filter.Lte(c => c.EndTime, endDate),
               Builders<RoomStatisticsEntity>.Filter.Gt(c => c.TotalJoins, 0)
           );

            return await _context.RoomStatistic.Find(filter).ToListAsync();
        }
    }
}
