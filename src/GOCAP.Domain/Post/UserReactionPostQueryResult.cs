namespace GOCAP.Domain
{
    public class UserReactionPostQueryResult : QueryResult<UserReactionPost>
    {
        public Dictionary<ReactionType, int> Reactions { get; set; } = [];
    }
}
