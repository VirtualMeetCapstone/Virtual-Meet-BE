namespace GOCAP.Api.Validation;

public class CommentValidator : ValidatorBase<CommentCreationModel>
{
    public CommentValidator()
    {
        RuleFor(x => x.AuthorId)
            .NotEmpty()
            .WithMessage("Author must not be empty.");

        //RuleFor(x => x.Contents)
        //    .Must(x => x.Count > 0 && x.Count < 3)
        //    .WithMessage("Content must not exceed 3 items.");

        //RuleForEach(x => x.Contents).SetValidator(new CommentContentValidator());
    }
}
