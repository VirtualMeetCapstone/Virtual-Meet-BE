using GOCAP.Database;
using Microsoft.EntityFrameworkCore;

namespace GOCAP.Migrations;

/// <summary>
/// This db context is only used for generating the migrations and data.          
/// Please make sure the consistency between two db context in 2 different projects GOCAP.Database and GOCAP.Migrations.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext() { }
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    public DbSet<GroupEntity> Groups { get; set; }
    public DbSet<GroupMemberEntity> GroupMembers { get; set; }
    public DbSet<RoomChannelEntity> RoomChannels { get; set; }
    public DbSet<RoomEntity> Rooms { get; set; }
    public DbSet<RoomEventEntity> RoomEvents { get; set; }
    public DbSet<RoomFavouriteEntity> RoomFavourites { get; set; }
    public DbSet<RoomMemberEntity> RoomMembers { get; set; }
    public DbSet<RoomMemberRoleEntity> RoomMemberRoles { get; set; }
    public DbSet<RoomNotificationEntity> RoomNotifications { get; set; }
    public DbSet<RoomSettingEntity> RoomSettings { get; set; }
    public DbSet<HashTagEntity> HashTags { get; set; }
    public DbSet<RoomHashTagEntity> RoomHashTags { get; set; }
    public DbSet<UserActivityEntity> UserActivities { get; set; }
    public DbSet<UserBlockEntity> UserBlocks { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<UserFollowEntity> UserFollows { get; set; }
    public DbSet<UserNotificationEntity> UserNotifications { get; set; }
    public DbSet<PostEntity> Posts { get; set; }
    public DbSet<PostReactionEntity> PostReactions { get; set; }
    public DbSet<UserRewardEntity> UserRewards { get; set; }
    public DbSet<UserRoleEntity> UserRoles { get; set; }
    public DbSet<RoleEntity> Roles { get; set; }
    public DbSet<RoleHierarchyEntity> RoleHierarchies { get; set; }
    public DbSet<PermissionEntity> Permissions { get; set; }
    public DbSet<RolePermissionEntity> RolePermissions { get; set; }
    public DbSet<StoryEntity> Stories { get; set; }
    public DbSet<StoryViewEntity> StoryViews { get; set; }
    public DbSet<StoryReactionEntity> StoryReactions { get; set; }
    public DbSet<StoryHightLightEntity> StoryHightLights { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=db9981.public.databaseasp.net; Database=db9981; User Id=db9981; Password=manhtuong1; Encrypt=True; TrustServerCertificate=True; MultipleActiveResultSets=True;");
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

        modelBuilder.Entity<GroupMemberEntity>()
                    .HasOne(rf => rf.Group)
                    .WithMany(u => u.Members)
                    .HasForeignKey(rf => rf.GroupId)
                    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PostReactionEntity>()
                    .HasOne(rf => rf.Post)
                    .WithMany(u => u.Reactions)
                    .HasForeignKey(rf => rf.PostId)
                    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StoryViewEntity>()
                    .HasOne(rf => rf.Story)
                    .WithMany(u => u.Views)
                    .HasForeignKey(rf => rf.StoryId)
                    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StoryReactionEntity>()
                    .HasOne(rf => rf.Story)
                    .WithMany(u => u.Reactions)
                    .HasForeignKey(rf => rf.StoryId)
                    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StoryHightLightEntity>()
                    .HasOne(s => s.User)
                    .WithMany(u => u.StoryHightLights)
                    .HasForeignKey(s => s.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StoryHightLightEntity>()
                    .HasOne(s => s.PrevStory)
                    .WithOne()
                    .HasForeignKey<StoryHightLightEntity>(s => s.PrevStoryId)
                    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StoryHightLightEntity>()
                    .HasOne(s => s.NextStory)
                    .WithOne()
                    .HasForeignKey<StoryHightLightEntity>(s => s.NextStoryId)
                    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RoleHierarchyEntity>()
                    .HasOne(rh => rh.ParentRole)
                    .WithMany(r => r.ChildRoles)
                    .HasForeignKey(rh => rh.ParentRoleId)
                    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RoleHierarchyEntity>()
                    .HasOne(rh => rh.ChildRole)
                    .WithMany(r => r.ParentRoles)
                    .HasForeignKey(rh => rh.ChildRoleId)
                    .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelBuilder);
    }
}

