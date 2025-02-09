namespace GOCAP.Api.Validation;

public class CommentContentValidator : ValidatorBase<CommentContentModel>
{
    public CommentContentValidator()
    {
        RuleFor(x => x.Type)
            .Must(x => Enum.IsDefined(typeof(MediaType), x))
            .WithMessage("Invalid media type.");
      
    }
}
