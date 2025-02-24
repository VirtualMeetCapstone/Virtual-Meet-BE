namespace GOCAP.Api.Validation;

public class SearchValidator : ValidatorBase<SearchHistoryCreationModel>
{
    public SearchValidator()
    {
        RuleFor(x => x.Query)
            .NotNull()
            .WithMessage("Query must not be null.")
            .NotEmpty()
            .WithMessage("Query must not be empty.")
            .MaximumLength(AppConstants.MaxLengthSearchQuery)
            .WithMessage($"Query must not exceed {AppConstants.MaxLengthSearchQuery} characters.");
    }
}
