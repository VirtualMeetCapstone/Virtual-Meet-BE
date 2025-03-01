using GOCAP.Commom;

namespace GOCAP.Api.Validation
{
	public class RoomValidator : ValidatorBase<RoomCreationModel>
	{
		public RoomValidator()
		{
			RuleFor(x => x.Topic)
				.NotEmpty().WithMessage(ErrorMessageBase.Required)
				.MaximumLength(256).WithMessage(ErrorMessageBase.MaxLength);

			RuleFor(x => x.Description)
				.MaximumLength(AppConstants.MaxLengthDescription)
				.WithMessage(ErrorMessageBase.MaxLength);

			RuleFor(x => x.MaximumMembers)
				.NotEmpty().WithMessage(ErrorMessageBase.Required)
				.GreaterThan(0).WithMessage(ErrorMessageBase.GreaterThan);
		}
	}
}
