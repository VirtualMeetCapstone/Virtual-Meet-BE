﻿namespace GOCAP.Domain;

public class Group : CreatedObjectBase
{
    public Guid OwnerId { get; set; }
    public User? Owner { get; set; }
    public string? Picture { get; set; }
    public ICollection<GroupMember> Members { get; set; } = [];
}
