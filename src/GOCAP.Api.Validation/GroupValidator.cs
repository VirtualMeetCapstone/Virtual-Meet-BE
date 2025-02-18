namespace GOCAP.Api.Validation;

public class GroupValidator : ValidatorBase<GroupCreationModel>
{
    public GroupValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name must not be empty.")
            .WithErrorCode("1000")
            .MaximumLength(AppConstants.MaxLengthName)
            .WithMessage($"Name must not be exceed {AppConstants.MaxLengthName}.")
            .WithErrorCode("1001");
    }
}
