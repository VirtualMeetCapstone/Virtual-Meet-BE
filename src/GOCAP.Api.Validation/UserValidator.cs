namespace GOCAP.Api.Validation;

public class UserValidator : ValidatorBase<UserCreationModel>
{
    public UserValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name must not be empty.")
            .NotNull().WithMessage("Name must not be null.")
            .MaximumLength(256).WithMessage("Name must not exceed 256 characters.")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters long.");
    }
}
