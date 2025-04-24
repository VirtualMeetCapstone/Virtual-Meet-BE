using GOCAP.Database;
namespace GOCAP.Repository
{
    public class RoomStatisticsMapperProfile : EntityMapperProfileBase
    {
        public RoomStatisticsMapperProfile()
        {
            CreateMap<RoomStatistics, RoomStatisticsEntity>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<RoomStatisticsEntity, RoomStatistics>();
        }
    }
}
