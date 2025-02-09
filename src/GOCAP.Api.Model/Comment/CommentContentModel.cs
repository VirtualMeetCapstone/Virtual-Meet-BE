using System.Text.Json.Serialization;

namespace GOCAP.Api.Model;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(TextCommentModel), typeDiscriminator: "text")]
[JsonDerivedType(typeof(ImageCommentModel), typeDiscriminator: "image")]
[JsonDerivedType(typeof(VideoCommentModel), typeDiscriminator: "video")]
[JsonDerivedType(typeof(StickerCommentModel), typeDiscriminator: "sticker")]
[JsonDerivedType(typeof(GifCommentModel), typeDiscriminator: "gif")]
public abstract class CommentContentModel
{
    public abstract MediaType Type { get; }
}

public class TextCommentModel : CommentContentModel
{
    public override MediaType Type => MediaType.Text;
    public required string Text { get; set; }
}

public class ImageCommentModel : CommentContentModel
{
    public override MediaType Type => MediaType.Image;

    public required IFormFile Image { get; set; } 
}

public class VideoCommentModel : CommentContentModel
{
    public override MediaType Type => MediaType.Video;

    public required IFormFile Video { get; set; }

    public int Duration { get; set; }
}

public class StickerCommentModel : CommentContentModel
{
    public override MediaType Type => MediaType.Sticker;

    public required string StickerUrl { get; set; }
}

public class GifCommentModel : CommentContentModel
{
    public override MediaType Type => MediaType.Gif;

    public required string GifUrl { get; set; }
}