using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using RohitPortfolio.Shared;

namespace RohitPortfolio.Blazor.Services;

public sealed class BlogApiClient(HttpClient http, IConfiguration configuration)
{
    private readonly string _baseUrl = (configuration["BlogApi:BaseUrl"] ?? "").TrimEnd('/');

    public async Task<BlogPostListResponse> GetPostsAsync(string? tag = null, bool? featured = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_baseUrl))
        {
            var posts = FallbackBlogStore.Posts
                .Where(post => string.IsNullOrWhiteSpace(tag) || post.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
                .Where(post => featured is null || post.IsFeatured == featured)
                .OrderByDescending(post => post.PublishedAt)
                .Select(FallbackBlogStore.ToSummary)
                .ToArray();

            return new BlogPostListResponse(posts, posts.Length, 1, posts.Length);
        }

        var query = new List<string>();
        if (!string.IsNullOrWhiteSpace(tag)) query.Add($"tag={Uri.EscapeDataString(tag)}");
        if (featured is not null) query.Add($"featured={featured.Value.ToString().ToLowerInvariant()}");
        var url = $"{_baseUrl}/api/blog/posts{(query.Count > 0 ? $"?{string.Join("&", query)}" : "")}";

        return await http.GetFromJsonAsync<BlogPostListResponse>(url, cancellationToken)
               ?? new BlogPostListResponse([], 0, 1, 12);
    }

    public async Task<BlogPostDetailDto?> GetPostAsync(string slug, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_baseUrl))
        {
            return FallbackBlogStore.Posts.FirstOrDefault(post => post.Slug == slug);
        }

        try
        {
            return await http.GetFromJsonAsync<BlogPostDetailDto>($"{_baseUrl}/api/blog/posts/{Uri.EscapeDataString(slug)}", cancellationToken);
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }
}

internal static class FallbackBlogStore
{
    public static readonly BlogPostDetailDto[] Posts =
    [
        new(
            Guid.Parse("01974b16-8d79-7b66-a8ff-b49df37d4a11"),
            "Why I rebuilt my portfolio in Blazor",
            "why-i-rebuilt-my-portfolio-in-blazor",
            "A short build note on turning a static portfolio into a C# product surface with interaction, role targeting, and a backend-ready architecture.",
            """
            ## The goal

            A portfolio should not behave like a passive resume. It should prove that I can design, build, ship, and explain software.

            Rebuilding the site in **Blazor WebAssembly** lets the portfolio itself become evidence: C#, components, routing, state, JavaScript interop, responsive UI, and deployment all in one place.

            ## The architectural direction

            The frontend remains static and fast on GitHub Pages. The blog backend is designed separately as an ASP.NET Core API with PostgreSQL, so future technical notes can be managed without rebuilding the whole site.

            ## Why this matters

            Recruiters and engineering teams can see more than project screenshots. They can see decisions, trade-offs, and implementation discipline.
            """,
            """
            <h2>The goal</h2>
            <p>A portfolio should not behave like a passive resume. It should prove that I can design, build, ship, and explain software.</p>
            <p>Rebuilding the site in <strong>Blazor WebAssembly</strong> lets the portfolio itself become evidence: C#, components, routing, state, JavaScript interop, responsive UI, and deployment all in one place.</p>
            <h2>The architectural direction</h2>
            <p>The frontend remains static and fast on GitHub Pages. The blog backend is designed separately as an ASP.NET Core API with PostgreSQL, so future technical notes can be managed without rebuilding the whole site.</p>
            <h2>Why this matters</h2>
            <p>Recruiters and engineering teams can see more than project screenshots. They can see decisions, trade-offs, and implementation discipline.</p>
            """,
            ["Blazor", "C#", ".NET", "Portfolio"],
            null,
            "portfolio",
            "Why I rebuilt my portfolio in Blazor",
            "A build note on turning a static portfolio into a C# product surface.",
            new DateTimeOffset(2026, 6, 24, 8, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 6, 24, 8, 0, 0, TimeSpan.Zero),
            2,
            true),
        new(
            Guid.Parse("01974b16-9b8a-747e-89f9-bf4a90f43f22"),
            "The portfolio backend I would build first",
            "portfolio-backend-i-would-build-first",
            "The practical backend shape for this site: Minimal APIs, PostgreSQL, Markdown, admin-key protection, and no unnecessary CMS bloat.",
            """
            ## Keep the backend small

            The right first backend is not a full CMS. It is a focused publishing API:

            - public read endpoints
            - protected create/update/delete endpoints
            - PostgreSQL persistence
            - Markdown content
            - SEO fields
            - tags and related projects

            ## Why PostgreSQL

            Blog content is relational enough for SQL, but flexible enough to benefit from PostgreSQL arrays, indexing, JSON options, and full-text search later.

            ## What comes next

            Once the API is hosted, the Blazor frontend only needs a `BlogApi:BaseUrl` setting to switch from fallback content to live database content.
            """,
            """
            <h2>Keep the backend small</h2>
            <p>The right first backend is not a full CMS. It is a focused publishing API:</p>
            <ul><li>public read endpoints</li><li>protected create/update/delete endpoints</li><li>PostgreSQL persistence</li><li>Markdown content</li><li>SEO fields</li><li>tags and related projects</li></ul>
            <h2>Why PostgreSQL</h2>
            <p>Blog content is relational enough for SQL, but flexible enough to benefit from PostgreSQL arrays, indexing, JSON options, and full-text search later.</p>
            <h2>What comes next</h2>
            <p>Once the API is hosted, the Blazor frontend only needs a <code>BlogApi:BaseUrl</code> setting to switch from fallback content to live database content.</p>
            """,
            ["Backend", "PostgreSQL", "Minimal APIs", "EF Core"],
            null,
            "backend",
            "The portfolio backend I would build first",
            "A focused ASP.NET Core and PostgreSQL backend plan for the portfolio blog.",
            new DateTimeOffset(2026, 6, 24, 8, 15, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 6, 24, 8, 15, 0, TimeSpan.Zero),
            2,
            true)
    ];

    public static BlogPostSummaryDto ToSummary(BlogPostDetailDto post) =>
        new(post.Id, post.Title, post.Slug, post.Summary, post.Tags, post.CoverImageUrl, post.RelatedProjectKey, post.PublishedAt, post.ReadingMinutes, post.IsFeatured);
}
