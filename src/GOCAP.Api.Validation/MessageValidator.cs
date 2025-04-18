namespace GOCAP.Api.Validation;

public class MessageValidator : ValidatorBase<MessageCreationModel>
{
    public MessageValidator()
    {
        RuleFor(x => x.SenderId)
            .NotEmpty()
            .WithMessage(ValidationMessage.Required);

        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.Content) || (x.Attachments != null && x.Attachments.Count > 0))
            .WithMessage(ValidationMessage.Format(ValidationMessage.ListMinItems, nameof(MessageCreationModel.Content), nameof(MessageCreationModel.Attachments)));

        RuleFor(x => x.Attachments)
            .Must(attachments => attachments == null || attachments.Count <= 5)
            .WithMessage(ValidationMessage.MaxLength);

        RuleFor(x => x.Content)
            .Must(content => content == null || content.Length <= AppConstants.MaxLengthDescription)
            .WithMessage(ValidationMessage.MaxLength);
    }
}
