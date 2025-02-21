﻿namespace GOCAP.Migrations;

public static class SeedData
{
    public static readonly Guid UserRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid AdminRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid SystemRoleId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    public static readonly Guid RoomOwnerRoleId = Guid.Parse("44444444-4444-4444-4444-444444444444");
    public static readonly Guid RoomCoOwnerRoleId = Guid.Parse("55555555-5555-5555-5555-555555555555");
    public static readonly Guid RoomMemberRoleId = Guid.Parse("66666666-6666-6666-6666-666666666666");

    public static readonly long CreateTime = DateTime.Now.Ticks;
    public static readonly long LastModifyTime = CreateTime;
}
