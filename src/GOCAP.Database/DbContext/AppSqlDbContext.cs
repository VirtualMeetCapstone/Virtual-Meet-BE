namespace GOCAP.Database;

public class AppSqlDbContext : DbContext
{
    public AppSqlDbContext() { }
    public AppSqlDbContext(DbContextOptions<AppSqlDbContext> options) : base(options) { }

    public DbSet<GroupEntity> Groups { get; set; }
    public DbSet<RoomEntity> Rooms { get; set; }
    public DbSet<RoomEventEntity> RoomEvents { get; set; }
    public DbSet<RoomFavouriteEntity> RoomFavourites { get; set; }
    public DbSet<RoomMemberEntity> RoomMembers { get; set; }
    public DbSet<RoomMemberRoleEntity> RoomMemberRoles { get; set; }
    public DbSet<RoomNotificationEntity> RoomNotifications { get; set; }
    public DbSet<RoomSettingEntity> RoomSettings { get; set; }
    public DbSet<RoomTagEntity> RoomTags { get; set; }
    public DbSet<UserActivityEntity> UserActivities { get; set; }
    public DbSet<UserBlockEntity> UserBlocks { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<UserFollowEntity> UserFollows { get; set; }
    public DbSet<UserNotificationEntity> UserNotifications { get; set; }
    public DbSet<UserPostEntity> UserPosts { get; set; }
    public DbSet<UserRewardEntity> UserRewards { get; set; }
    public DbSet<UserRoleEntity> UserRoles { get; set; }
    public DbSet<UserStoryEntity> UserStories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Data Source=BIGBOSS;Database=GOCAP;User Id=sa;Password=123456789;TrustServerCertificate=true;Trusted_Connection=SSPI;Encrypt=true");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserBlockEntity>()
        .HasOne(ub => ub.BlockedByUser) 
        .WithMany(u => u.Blocks)   
        .HasForeignKey(ub => ub.BlockedByUserId)
        .OnDelete(DeleteBehavior.Restrict); 

        modelBuilder.Entity<UserBlockEntity>()
            .HasOne(ub => ub.BlockedUser)  
            .WithMany()                  
            .HasForeignKey(ub => ub.BlockedUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserFollowEntity>()
            .HasOne(uf => uf.Follower) 
            .WithMany(u => u.Follows) 
            .HasForeignKey(uf => uf.FollowerId)
            .OnDelete(DeleteBehavior.Restrict); 

        modelBuilder.Entity<UserFollowEntity>()
            .HasOne(uf => uf.Following) 
            .WithMany() 
            .HasForeignKey(uf => uf.FollowingId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RoomFavouriteEntity>()
                    .HasOne(rf => rf.User)
                    .WithMany(u => u.RoomFavourites)
                    .HasForeignKey(rf => rf.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RoomMemberEntity>()
                    .HasOne(rf => rf.Room)
                    .WithMany(u => u.Members)
                    .HasForeignKey(rf => rf.RoomId)
                    .OnDelete(DeleteBehavior.Restrict);
        base.OnModelCreating(modelBuilder); 
    }
}
