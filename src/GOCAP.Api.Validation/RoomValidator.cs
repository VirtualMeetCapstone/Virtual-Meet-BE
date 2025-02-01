using GOCAP.Common;

namespace GOCAP.Api.Validation;

public class RoomValidator : ValidatorBase<RoomCreationModel>
{
    public RoomValidator()
    {
        RuleFor(x => x.Topic)
            .NotEmpty()
            .WithMessage("Topic must not be empty.")
            .NotNull()
            .WithMessage("Topic must not be null.")
            .MaximumLength(256)
            .WithMessage("Name must not exceed 256 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(GOCAPConstants.MaxLengthDescription)
            .WithMessage("Name must not exceed 1000 characters.");

        RuleFor(x => x.MaximumMembers);
    }
}
