namespace GOCAP.Services.Intention;

public interface IUserContextService
{
    Guid Id { get; }
    string Name {  get; }
    string Email { get; }
    string Picture { get; }
    string Role {  get; }
}
