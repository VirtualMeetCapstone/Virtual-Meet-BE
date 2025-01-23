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
            .WithMessage("Name must not exceed 256 characters.")
            .MinimumLength(3)
            .WithMessage("Name must be at least 3 characters long.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Name must not exceed 256 characters.");

        RuleFor(x => x.MaximumMembers);
    }
}
