namespace GOCAP.Api.Validation;

public class StoryValidator : ValidatorBase<StoryCreationModel>
{
    public StoryValidator()
    {
        RuleFor(x => x.MediaUpload)
            .NotNull()
            .WithMessage("Story must not be null.");
    }

}
