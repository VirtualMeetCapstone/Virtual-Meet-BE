﻿namespace GOCAP.Domain;

public class UserBlock : DateTrackingBase
{
	public Guid BlockedUserId { get; set; }
	public Guid BlockedByUserId { get; set; }
	public string Name { get; set; } = string.Empty;
	public Media? Picture { get; set; }
	public User? BlockedUser { get; set; }
	public User? BlockedByUser { get; set; }
}
