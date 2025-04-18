namespace GOCAP.Domain;

public class NotificationMessageBuilder
{
    private string _actor = string.Empty;
    private string _action = string.Empty;
    private string _target = string.Empty;
    private string _context = string.Empty;
    private string _extraInfo = string.Empty;

    public NotificationMessageBuilder SetActor(string actor)
    {
        _actor = actor;
        return this;
    }

    public NotificationMessageBuilder SetAction(string action)
    {
        _action = action;
        return this;
    }

    public NotificationMessageBuilder SetTarget(string target)
    {
        _target = target;
        return this;
    }

    public NotificationMessageBuilder SetContext(string context)
    {
        _context = context;
        return this;
    }

    public NotificationMessageBuilder SetExtraInfo(string extraInfo)
    {
        _extraInfo = extraInfo;
        return this;
    }

    public string Build()
    {
        return $"{_actor} has {_action} {_target}" +
               (!string.IsNullOrEmpty(_extraInfo) ? $" {_extraInfo}" : "") +
               (!string.IsNullOrEmpty(_context) ? $" {_context}" : "") +
               ".";
    }
}
