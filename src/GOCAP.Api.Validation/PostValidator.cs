namespace GOCAP.Api.Validation;

public class PostValidator : ValidatorBase<PostCreationModel>
{
    public PostValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Content must not be empty.")
            .NotNull()
            .WithMessage("Content must not be null.")
            .MaximumLength(256)
            .WithMessage("Content must not exceed 256 characters.")
            .MinimumLength(3)
            .WithMessage("Content must be at least 3 characters long.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User must not be empty.")
            .NotNull()
            .WithMessage("User must not be null.");

        RuleFor(x => x.MediaUploads)
            .NotEmpty().WithMessage("MediaUploads cannot be empty.")
            .NotNull().WithMessage("MediaUploads cannot be null.")
            .Must(ValidateMediaCount).WithMessage("Invalid upload files. File must be image type or video type and between 1 to 5 images or 1 video only.");
    }

}
