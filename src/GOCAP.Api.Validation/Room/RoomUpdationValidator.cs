namespace GOCAP.Api.Validation;

public class RoomUpdationValidator : ValidatorBase<RoomUpdationModel>
{
    public RoomUpdationValidator()
    {
        RuleFor(x => x.Topic)
            .NotEmpty().WithMessage(ValidationMessage.Required)
            .MaximumLength(256).WithMessage(ValidationMessage.MaxLength);

        RuleFor(x => x.Description)
            .MaximumLength(AppConstants.MaxLengthDescription)
            .WithMessage(ValidationMessage.MaxLength);

        RuleFor(x => x.MaximumMembers)
            .NotNull()
            .NotEmpty().WithMessage(ValidationMessage.Required)
            .GreaterThan(0).WithMessage(ValidationMessage.GreaterThan);
    }
}
