namespace GOCAP.Domain;

public class Role : DomainBase
{
    public string? Name { get; set; }

    public ICollection<UserRole>? UserRoles { get; set; }
}
