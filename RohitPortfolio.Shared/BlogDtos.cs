namespace RohitPortfolio.Shared;

public sealed record BlogPostSummaryDto(
    Guid Id,
    string Title,
    string Slug,
    string Summary,
    string[] Tags,
    string? CoverImageUrl,
    string? RelatedProjectKey,
    DateTimeOffset PublishedAt,
    int ReadingMinutes,
    bool IsFeatured);

public sealed record BlogPostDetailDto(
    Guid Id,
    string Title,
    string Slug,
    string Summary,
    string ContentMarkdown,
    string ContentHtml,
    string[] Tags,
    string? CoverImageUrl,
    string? RelatedProjectKey,
    string SeoTitle,
    string SeoDescription,
    DateTimeOffset PublishedAt,
    DateTimeOffset UpdatedAt,
    int ReadingMinutes,
    bool IsFeatured);

public sealed record BlogPostUpsertRequest(
    string Title,
    string? Slug,
    string Summary,
    string ContentMarkdown,
    string[] Tags,
    string? CoverImageUrl,
    string? RelatedProjectKey,
    string? SeoTitle,
    string? SeoDescription,
    DateTimeOffset? PublishedAt,
    bool IsPublished,
    bool IsFeatured);

public sealed record BlogPostListResponse(
    IReadOnlyList<BlogPostSummaryDto> Items,
    int TotalCount,
    int Page,
    int PageSize);
