namespace GOCAP.Api.Validation;

public class RoomValidator : ValidatorBase<RoomCreationModel>
{
	public RoomValidator()
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

        RuleFor(x => x.Password)
            .MaximumLength(10).WithMessage(ValidationMessage.MaxLength);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(ValidationMessage.Required)
            .When(x => x.Privacy == PrivacyType.Private);

        RuleFor(x => x.Password)
			.Must(string.IsNullOrEmpty)
			.WithMessage("Password must be empty for public rooms.")
			.When(x => x.Privacy == PrivacyType.Public);
	}
}
