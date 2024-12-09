namespace GOCAP.Database;

public class AppSqlDbContext : DbContext
{
    public AppSqlDbContext() { }
    public AppSqlDbContext(DbContextOptions<AppSqlDbContext> options) : base(options) { }

    public DbSet<UserEntity> Users { get; set; }
    public DbSet<RoleEntity> Roles { get; set; }
    public DbSet<PostEntity> Posts { get; set; }
    public DbSet<NotificationEntity> Notifications { get; set; }
    //public DbSet<FollowEntity> Follows { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(GOCAPConfiguration.GetSqlServerConnectionString());
    }
    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    base.OnModelCreating(modelBuilder);

    //    modelBuilder.Entity<FollowEntity>()
    //                .HasOne(f => f.Follower)
    //                .WithMany(u => u.Followings)
    //                .HasForeignKey(f => f.FollowerId)
    //                .OnDelete(DeleteBehavior.Restrict);

    //    modelBuilder.Entity<FollowEntity>()
    //                .HasOne(f => f.Following)
    //                .WithMany(u => u.Followers)
    //                .HasForeignKey(f => f.FollowingId)
    //                .OnDelete(DeleteBehavior.Restrict);
    //}
}
