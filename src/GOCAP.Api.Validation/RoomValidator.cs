namespace GOCAP.Api.Validation;

public class RoomValidator : ValidatorBase<RoomCreationModel>
{
	public RoomValidator()
	{
		RuleFor(x => x.Topic)
			.NotEmpty().WithMessage("Topic must not be empty.")
			.MaximumLength(256).WithMessage(ValidationMessage.MaxLength);

		RuleFor(x => x.Description)
			.MaximumLength(AppConstants.MaxLengthDescription)
			.WithMessage(ValidationMessage.MaxLength);

		RuleFor(x => x.MaximumMembers)
			.NotNull()
			.NotEmpty().WithMessage("Maximum member must be greate than 0.")
			.GreaterThan(0).WithMessage(ValidationMessage.GreaterThan);
	}
}
