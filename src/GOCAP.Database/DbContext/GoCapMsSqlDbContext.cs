namespace GOCAP.Database;

public class GoCapMsSqlDbContext : DbContextBase
{
    public GoCapMsSqlDbContext() { }
    public GoCapMsSqlDbContext(DbContextOptions<GoCapMsSqlDbContext> options) : base(options) { }

    public virtual DbSet<UserEntity> Users { get; set; }
    public virtual DbSet<RoleEntity> Roles { get; set; }
    public virtual DbSet<UserRoleEntity> UserRoles { get; set; }
    public virtual DbSet<PostEntity> Posts { get; set; }
    public virtual DbSet<MediaEntity> Medias { get; set; }
    public virtual DbSet<NotificationEntity> Notifications { get; set; }
    public virtual DbSet<FollowEntity> Follows { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Data Source=BIGBOSS\\SQLEXPRESS;Database=GOCAP;User Id=sa;Password=123456789;TrustServerCertificate=true;
            Trusted_Connection=SSPI;Encrypt=true;Connection Timeout=30;Pooling=True;Max Pool Size=1000;");
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>()
                    .HasMany(u => u.Posts)
                    .WithOne(p => p.User)
                    .HasForeignKey(p => p.UserId);

        modelBuilder.Entity<UserEntity>()
                    .HasMany(u => u.Followers)
                    .WithOne(f => f.Following)
                    .HasForeignKey(f => f.FollowingId);

        modelBuilder.Entity<UserEntity>()
                    .HasMany(u => u.Following)
                    .WithOne(f => f.Follower)
                    .HasForeignKey(f => f.FollowerId);

        modelBuilder.Entity<UserEntity>()
                    .HasMany(u => u.Notifications)
                    .WithOne(n => n.User)
                    .HasForeignKey(n => n.UserId);

        modelBuilder.Entity<RoleEntity>()
                    .HasMany(r => r.UserRoles)
                    .WithOne(ur => ur.Role)
                    .HasForeignKey(ur => ur.RoleId);

        modelBuilder.Entity<UserEntity>()
                    .HasMany(u => u.UserRoles)
                    .WithOne(ur => ur.User)
                    .HasForeignKey(ur => ur.UserId);

        modelBuilder.Entity<FollowEntity>()
                    .HasOne(f => f.Follower)
                    .WithMany(u => u.Following)
                    .HasForeignKey(f => f.FollowerId)
                    .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<FollowEntity>()
                    .HasOne(f => f.Following)
                    .WithMany(u => u.Followers)
                    .HasForeignKey(f => f.FollowingId)
                    .OnDelete(DeleteBehavior.NoAction);
    }
}
