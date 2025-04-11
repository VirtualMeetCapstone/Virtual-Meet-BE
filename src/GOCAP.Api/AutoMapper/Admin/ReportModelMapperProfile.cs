namespace GOCAP.Api.AutoMapper;

public class ReportModelMapperProfile : ModelMapperProfileBase
{
    public ReportModelMapperProfile()
    {
        CreateMap<DateRange, DateRangeModel>()
            .ForMember(dest => dest.From, opt => opt.MapFrom(src => new DateTime(src.From)))
            .ForMember(dest => dest.To, opt => opt.MapFrom(src => new DateTime(src.To)))
            .ReverseMap()
            .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.From.Ticks))
            .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.To.Ticks));
        CreateMap<UserReport, UserReportModel>().ReverseMap();
        CreateMap<PostReport, PostReportModel>().ReverseMap();
    }
}