﻿using Microsoft.EntityFrameworkCore;

namespace GOCAP.Database;

public class AppSqlDbContext
    (DbContextOptions<AppSqlDbContext> options,
    IAppConfiguration _configuration) : DbContext(options)
{
    /// <summary>
    /// Please always updating the number of db sets and the db sets name by order.
    /// There are 30 db sets.
    /// </summary>
    public virtual DbSet<GroupEntity> Groups { get; set; }
    public virtual DbSet<GroupMemberEntity> GroupMembers { get; set; }
    public virtual DbSet<RoomChannelEntity> RoomChannels { get; set; }
    public virtual DbSet<RoomEntity> Rooms { get; set; }
    public virtual DbSet<RoomTagEntity> RoomTags { get; set; }
    public virtual DbSet<RoomEventEntity> RoomEvents { get; set; }
    public virtual DbSet<RoomFavouriteEntity> RoomFavourites { get; set; }
    public virtual DbSet<RoomMemberEntity> RoomMembers { get; set; }
    public virtual DbSet<RoomMemberRoleEntity> RoomMemberRoles { get; set; }
    public virtual DbSet<RoomNotificationEntity> RoomNotifications { get; set; }
    public virtual DbSet<RoomSettingEntity> RoomSettings { get; set; }
    public virtual DbSet<HashTagEntity> HashTags { get; set; }
    public virtual DbSet<RoomHashTagEntity> RoomHashTags { get; set; }
    public virtual DbSet<UserActivityEntity> UserActivities { get; set; }
    public virtual DbSet<UserBlockEntity> UserBlocks { get; set; }
    public virtual DbSet<UserEntity> Users { get; set; }
    public virtual DbSet<UserFollowEntity> UserFollows { get; set; }
    public virtual DbSet<PostEntity> Posts { get; set; }
    public virtual DbSet<PostTagEntity> PostTags { get; set; }
    public virtual DbSet<PostReactionEntity> PostReactions { get; set; }
    public virtual DbSet<UserRewardEntity> UserRewards { get; set; }
    public virtual DbSet<UserRoleEntity> UserRoles { get; set; }
    public virtual DbSet<RoleEntity> Roles { get; set; }
    public virtual DbSet<RoleHierarchyEntity> RoleHierarchies { get; set; }
    public virtual DbSet<PermissionEntity> Permissions { get; set; }
    public virtual DbSet<RolePermissionEntity> RolePermissions { get; set; }
    public virtual DbSet<StoryEntity> Stories { get; set; }
    public virtual DbSet<StoryViewEntity> StoryViews { get; set; }
    public virtual DbSet<StoryReactionEntity> StoryReactions { get; set; }
    public virtual DbSet<StoryHightLightEntity> StoryHightLights { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var environment = _configuration.GetEnvironment();
        if (environment == AppConstants.Enviroments.Test)
        {
            // Use InMemoryDatabase for test environment
            optionsBuilder.UseInMemoryDatabase($"{AppConstants.Enviroments.Test}_{Guid.NewGuid()}");
        }
        else
        {
            // Use SQL Server for other environments (e.g., Development, Production)
            optionsBuilder.UseSqlServer(_configuration.GetSqlServerConnectionString());
        }
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

        modelBuilder.Entity<PostTagEntity>()
                    .HasOne(pt => pt.Post)
                    .WithMany(p => p.Tags)
                    .HasForeignKey(pt => pt.PostId)
                    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PostTagEntity>()
                    .HasOne(pt => pt.TaggedUser)
                    .WithMany()
                    .HasForeignKey(pt => pt.TaggedUserId)
                    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RoomTagEntity>()
                    .HasOne(pt => pt.Room)
                    .WithMany(p => p.Tags)
                    .HasForeignKey(pt => pt.RoomId)
                    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RoomTagEntity>()
                    .HasOne(pt => pt.TaggedUser)
                    .WithMany()
                    .HasForeignKey(pt => pt.TaggedUserId)
                    .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelBuilder);
    }
}
