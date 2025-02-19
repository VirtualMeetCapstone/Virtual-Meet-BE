namespace GOCAP.Repository;

[RegisterService(typeof(IStoryHightLightRepository))]
internal class StoryHightLightRepository(
    AppSqlDbContext context
     ) : SqlRepositoryBase<StoryHightLightEntity>(context), IStoryHightLightRepository
{
}