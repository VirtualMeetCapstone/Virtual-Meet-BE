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

        RuleFor(x => x.Type)
            .Must(type => Enum.IsDefined(typeof(MessageType), type))
            .WithMessage(ValidationMessage.InvalidFormat);

        When(x => x.Type == MessageType.Direct, () =>
        {
            RuleFor(x => x.RoomId)
                .Null()
                .WithMessage(ValidationMessage.Format(ValidationMessage.MustBe, nameof(MessageCreationModel.RoomId), "null"));

            RuleFor(x => x.GroupId)
                .Null()
                .WithMessage(ValidationMessage.Format(ValidationMessage.MustBe, nameof(MessageCreationModel.GroupId), "null"));

            RuleFor(x => x.ReceiverId)
                .NotNull()
                .NotEmpty()
                .WithMessage(ValidationMessage.Required);
        });

        When(x => x.Type == MessageType.Room, () =>
        {
            RuleFor(x => x.ReceiverId)
                .Null()
                .WithMessage(ValidationMessage.Format(ValidationMessage.MustBe, nameof(MessageCreationModel.ReceiverId), "null"));

            RuleFor(x => x.GroupId)
                .Null()
                .WithMessage(ValidationMessage.Format(ValidationMessage.MustBe, nameof(MessageCreationModel.GroupId), "null"));

            RuleFor(x => x.RoomId)
                .NotNull()
                .NotEmpty()
                .WithMessage(ValidationMessage.Required);
        });

        When(x => x.Type == MessageType.Group, () =>
        {
            RuleFor(x => x.RoomId)
                .Null()
                .WithMessage(ValidationMessage.Format(ValidationMessage.MustBe, nameof(MessageCreationModel.RoomId), "null"));

            RuleFor(x => x.ReceiverId)
                .Null()
                .WithMessage(ValidationMessage.Format(ValidationMessage.MustBe, nameof(MessageCreationModel.ReceiverId), "null"));

            RuleFor(x => x.GroupId)
                .NotNull()
                .NotEmpty()
                .WithMessage(ValidationMessage.Required);
        });

    }
}
