namespace GOCAP.Domain;

public class Role : CreatedObjectBase
{
    public ICollection<UserRole>? UserRoles { get; set; }
}
