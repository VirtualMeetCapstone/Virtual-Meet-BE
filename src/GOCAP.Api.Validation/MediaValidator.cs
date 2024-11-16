namespace GOCAP.Api.Validation;

public class MediaValidator : ValidatorBase<MediaModel>
{
    public MediaValidator()
    {
        RuleFor(x => x.Url)
                .NotEmpty().WithMessage("URL must not be empty.")
                .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute)).WithMessage("URL is not valid.");
        RuleFor(x => x.Type)
                .IsInEnum().WithMessage("Media type is not valid.");
        RuleFor(x => x.ThumbnailUrl)
                .Must(uri => string.IsNullOrEmpty(uri) || Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                .WithMessage("Thumbnail URL is not valid.");
    }
}
