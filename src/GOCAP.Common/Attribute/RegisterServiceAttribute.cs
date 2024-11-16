namespace GOCAP.Common;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class RegisterServiceAttribute(Type serviceType, ServiceLifetime scope = ServiceLifetime.Scoped) : Attribute
{
    public ServiceLifetime Scope { get; set; } = scope;
    public Type ServiceType { get; set; } = serviceType;
}
    