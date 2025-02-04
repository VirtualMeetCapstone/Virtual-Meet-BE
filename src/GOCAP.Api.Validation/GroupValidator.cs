using GOCAP.Common;

namespace GOCAP.Api.Validation;

public class GroupValidator : ValidatorBase<GroupCreationModel>
{
    public GroupValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name must not be empty.")
            .WithErrorCode("1000")
            .MaximumLength(GOCAPConstants.MaxLengthName)
            .WithMessage($"Name must not be exceed {GOCAPConstants.MaxLengthName}.")
            .WithErrorCode("1001");

        RuleFor(x => x.Picture)
            .MaximumLength(GOCAPConstants.MaxLengthUrl)
            .WithMessage($"Picture must not be exceed {GOCAPConstants.MaxLengthUrl}.")
            .WithErrorCode("1001");
    }
}
