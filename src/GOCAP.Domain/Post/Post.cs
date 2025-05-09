﻿namespace GOCAP.Domain;

public class Post : DateTrackingBase
{
    public string? Content { get; set; }
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public List<Media>? Medias { get; set; }
    public List<MediaUpload>? MediaUploads { get; set; }
    public PrivacyType? Privacy { get; set; }
    public ICollection<PostReaction> Reactions { get; set; } = [];
    public int TotalReactions { get; set; }
    public int CommentCount { get; set; }
    public Dictionary<int, int> ReactionCounts { get; set; } = [];
}
