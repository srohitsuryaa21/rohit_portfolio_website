namespace RohitPortfolio.Api.Models;

public sealed class BlogPost
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required string Title { get; set; }
    public required string Slug { get; set; }
    public required string Summary { get; set; }
    public required string ContentMarkdown { get; set; }
    public string[] Tags { get; set; } = [];
    public string? CoverImageUrl { get; set; }
    public string? RelatedProjectKey { get; set; }
    public string? SeoTitle { get; set; }
    public string? SeoDescription { get; set; }
    public DateTimeOffset PublishedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    public bool IsPublished { get; set; }
    public bool IsFeatured { get; set; }
}
