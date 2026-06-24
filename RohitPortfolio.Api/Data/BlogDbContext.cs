using Microsoft.EntityFrameworkCore;
using RohitPortfolio.Api.Models;

namespace RohitPortfolio.Api.Data;

public sealed class BlogDbContext(DbContextOptions<BlogDbContext> options) : DbContext(options)
{
    public DbSet<BlogPost> Posts => Set<BlogPost>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var post = modelBuilder.Entity<BlogPost>();

        post.ToTable("blog_posts");
        post.HasKey(x => x.Id);
        post.HasIndex(x => x.Slug).IsUnique();
        post.HasIndex(x => x.PublishedAt);
        post.HasIndex(x => x.IsPublished);
        post.Property(x => x.Title).HasMaxLength(180).IsRequired();
        post.Property(x => x.Slug).HasMaxLength(220).IsRequired();
        post.Property(x => x.Summary).HasMaxLength(420).IsRequired();
        post.Property(x => x.ContentMarkdown).IsRequired();
        post.Property(x => x.Tags).HasColumnType("text[]");
        post.Property(x => x.CoverImageUrl).HasMaxLength(500);
        post.Property(x => x.RelatedProjectKey).HasMaxLength(120);
        post.Property(x => x.SeoTitle).HasMaxLength(180);
        post.Property(x => x.SeoDescription).HasMaxLength(320);
    }
}
