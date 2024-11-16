namespace GOCAP.Database;

public abstract class DbContextBase : DbContext
{
    public DbContextBase() { }
    public DbContextBase(DbContextOptions options) : base(options) { }
}
