namespace GOCAP.Api.Validation;

public class GroupValidator : ValidatorBase<GroupCreationModel>
{
    public GroupValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name must not be empty.")
            .MaximumLength(AppConstants.MaxLengthName)
            .WithMessage($"Name must not be exceed {AppConstants.MaxLengthName}.");

        RuleFor(x => x.OwnerId)
            .NotEmpty()
            .WithMessage("OwnerId must not be empty.");
    }
}
