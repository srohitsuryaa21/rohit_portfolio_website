using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using RohitPortfolio.Api.Data;
using RohitPortfolio.Api.Models;
using RohitPortfolio.Api.Services;
using RohitPortfolio.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddScoped<MarkdownService>();
builder.Services.AddDbContextPool<BlogDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("BlogDb");
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException("Missing ConnectionStrings:BlogDb. Use PostgreSQL for the portfolio blog backend.");
    }

    options.UseNpgsql(connectionString);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("portfolio", policy =>
    {
        var origins = builder.Configuration.GetSection("Blog:AllowedOrigins").Get<string[]>()
            ?? ["https://rohitsuryaa.com", "https://www.rohitsuryaa.com", "http://localhost:5000", "https://localhost:5001"];

        policy.WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("portfolio");

var blog = app.MapGroup("/api/blog").WithTags("Blog");

blog.MapGet("/posts", async (
    BlogDbContext db,
    MarkdownService markdown,
    string? tag,
    bool? featured,
    string? search,
    int page = 1,
    int pageSize = 12) =>
{
    page = Math.Max(1, page);
    pageSize = Math.Clamp(pageSize, 1, 24);

    var query = db.Posts
        .AsNoTracking()
        .Where(post => post.IsPublished && post.PublishedAt <= DateTimeOffset.UtcNow);

    if (!string.IsNullOrWhiteSpace(tag))
    {
        query = query.Where(post => post.Tags.Contains(tag));
    }

    if (featured is not null)
    {
        query = query.Where(post => post.IsFeatured == featured);
    }

    if (!string.IsNullOrWhiteSpace(search))
    {
        query = query.Where(post =>
            EF.Functions.ILike(post.Title, $"%{search}%") ||
            EF.Functions.ILike(post.Summary, $"%{search}%") ||
            EF.Functions.ILike(post.ContentMarkdown, $"%{search}%"));
    }

    var total = await query.CountAsync();
    var posts = await query
        .OrderByDescending(post => post.PublishedAt)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
    var items = posts.Select(post => ToSummary(post, markdown)).ToList();

    return Results.Ok(new BlogPostListResponse(items, total, page, pageSize));
});

blog.MapGet("/posts/{slug}", async (BlogDbContext db, MarkdownService markdown, string slug) =>
{
    var post = await db.Posts
        .AsNoTracking()
        .FirstOrDefaultAsync(post => post.Slug == slug && post.IsPublished && post.PublishedAt <= DateTimeOffset.UtcNow);

    return post is null ? Results.NotFound() : Results.Ok(ToDetail(post, markdown));
});

blog.MapGet("/tags", async (BlogDbContext db) =>
{
    var posts = await db.Posts
        .AsNoTracking()
        .Where(post => post.IsPublished && post.PublishedAt <= DateTimeOffset.UtcNow)
        .Select(post => post.Tags)
        .ToListAsync();

    var tags = posts
        .SelectMany(tagSet => tagSet)
        .Where(tag => !string.IsNullOrWhiteSpace(tag))
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .OrderBy(tag => tag)
        .ToArray();

    return Results.Ok(tags);
});

var admin = app.MapGroup("/api/admin/blog")
    .WithTags("Blog Admin")
    .AddEndpointFilter(RequireAdminApiKey);

admin.MapPost("/posts", async (BlogDbContext db, BlogPostUpsertRequest request) =>
{
    var slug = SlugService.Create(string.IsNullOrWhiteSpace(request.Slug) ? request.Title : request.Slug);
    var exists = await db.Posts.AnyAsync(post => post.Slug == slug);
    if (exists)
    {
        return Results.Conflict(new { message = $"A post with slug '{slug}' already exists." });
    }

    var post = FromRequest(request, slug);
    db.Posts.Add(post);
    await db.SaveChangesAsync();

    return Results.Created($"/api/blog/posts/{post.Slug}", new { post.Id, post.Slug });
});

admin.MapPut("/posts/{id:guid}", async (BlogDbContext db, Guid id, BlogPostUpsertRequest request) =>
{
    var post = await db.Posts.FirstOrDefaultAsync(post => post.Id == id);
    if (post is null)
    {
        return Results.NotFound();
    }

    var slug = SlugService.Create(string.IsNullOrWhiteSpace(request.Slug) ? request.Title : request.Slug);
    var duplicateSlug = await db.Posts.AnyAsync(existing => existing.Id != id && existing.Slug == slug);
    if (duplicateSlug)
    {
        return Results.Conflict(new { message = $"A post with slug '{slug}' already exists." });
    }

    ApplyRequest(post, request, slug);
    await db.SaveChangesAsync();

    return Results.Ok(new { post.Id, post.Slug });
});

admin.MapDelete("/posts/{id:guid}", async (BlogDbContext db, Guid id) =>
{
    var deleted = await db.Posts.Where(post => post.Id == id).ExecuteDeleteAsync();
    return deleted == 0 ? Results.NotFound() : Results.NoContent();
});

app.Run();

static BlogPostSummaryDto ToSummary(BlogPost post, MarkdownService markdown) =>
    new(
        post.Id,
        post.Title,
        post.Slug,
        post.Summary,
        post.Tags,
        post.CoverImageUrl,
        post.RelatedProjectKey,
        post.PublishedAt,
        markdown.EstimateReadingMinutes(post.ContentMarkdown),
        post.IsFeatured);

static BlogPostDetailDto ToDetail(BlogPost post, MarkdownService markdown) =>
    new(
        post.Id,
        post.Title,
        post.Slug,
        post.Summary,
        post.ContentMarkdown,
        markdown.ToHtml(post.ContentMarkdown),
        post.Tags,
        post.CoverImageUrl,
        post.RelatedProjectKey,
        post.SeoTitle ?? post.Title,
        post.SeoDescription ?? post.Summary,
        post.PublishedAt,
        post.UpdatedAt,
        markdown.EstimateReadingMinutes(post.ContentMarkdown),
        post.IsFeatured);

static BlogPost FromRequest(BlogPostUpsertRequest request, string slug)
{
    var post = new BlogPost
    {
        Title = request.Title.Trim(),
        Slug = slug,
        Summary = request.Summary.Trim(),
        ContentMarkdown = request.ContentMarkdown.Trim(),
    };

    ApplyRequest(post, request, slug);
    return post;
}

static void ApplyRequest(BlogPost post, BlogPostUpsertRequest request, string slug)
{
    post.Title = request.Title.Trim();
    post.Slug = slug;
    post.Summary = request.Summary.Trim();
    post.ContentMarkdown = request.ContentMarkdown.Trim();
    post.Tags = request.Tags
        .Where(tag => !string.IsNullOrWhiteSpace(tag))
        .Select(tag => tag.Trim())
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToArray();
    post.CoverImageUrl = string.IsNullOrWhiteSpace(request.CoverImageUrl) ? null : request.CoverImageUrl.Trim();
    post.RelatedProjectKey = string.IsNullOrWhiteSpace(request.RelatedProjectKey) ? null : request.RelatedProjectKey.Trim();
    post.SeoTitle = string.IsNullOrWhiteSpace(request.SeoTitle) ? request.Title.Trim() : request.SeoTitle.Trim();
    post.SeoDescription = string.IsNullOrWhiteSpace(request.SeoDescription) ? request.Summary.Trim() : request.SeoDescription.Trim();
    post.PublishedAt = request.PublishedAt ?? DateTimeOffset.UtcNow;
    post.UpdatedAt = DateTimeOffset.UtcNow;
    post.IsPublished = request.IsPublished;
    post.IsFeatured = request.IsFeatured;
}

static ValueTask<object?> RequireAdminApiKey(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
{
    var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
    var configuredKey = configuration["Blog:AdminApiKey"];

    if (string.IsNullOrWhiteSpace(configuredKey) ||
        !context.HttpContext.Request.Headers.TryGetValue("X-Admin-Key", out var suppliedKey) ||
        !FixedTimeEquals(configuredKey, suppliedKey.ToString()))
    {
        return ValueTask.FromResult<object?>(Results.Unauthorized());
    }

    return next(context);
}

static bool FixedTimeEquals(string expected, string actual)
{
    var expectedBytes = Encoding.UTF8.GetBytes(expected);
    var actualBytes = Encoding.UTF8.GetBytes(actual);

    return expectedBytes.Length == actualBytes.Length &&
           CryptographicOperations.FixedTimeEquals(expectedBytes, actualBytes);
}
