using GOCAP.Common;
using GOCAP.Database;
using Microsoft.EntityFrameworkCore;

namespace GOCAP.Migrations;

public static class ModelBuilderExtensions
{
    public static void SeedRoles(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RoleEntity>().HasData(
            new RoleEntity { Id = SeedData.UserRoleId, Name = "User", Type = RoleType.User, CreateTime = SeedData.CreateTime, LastModifyTime = SeedData.LastModifyTime },
        new RoleEntity { Id = SeedData.AdminRoleId, Name = "Admin", Type = RoleType.Admin, CreateTime = SeedData.CreateTime, LastModifyTime = SeedData.LastModifyTime },
        new RoleEntity { Id = SeedData.SystemRoleId, Name = "System", Type = RoleType.System, CreateTime = SeedData.CreateTime, LastModifyTime = SeedData.LastModifyTime },
        new RoleEntity { Id = SeedData.RoomOwnerRoleId, Name = "RoomOwner", Type = RoleType.RoomOwner, CreateTime = SeedData.CreateTime, LastModifyTime = SeedData.LastModifyTime },
        new RoleEntity { Id = SeedData.RoomCoOwnerRoleId, Name = "RoomCoOwner", Type = RoleType.RoomCoOwner, CreateTime = SeedData.CreateTime, LastModifyTime = SeedData.LastModifyTime },
        new RoleEntity { Id = SeedData.RoomMemberRoleId, Name = "RoomMember", Type = RoleType.RoomMember, CreateTime = SeedData.CreateTime, LastModifyTime = SeedData.LastModifyTime }
        );
    }
}
