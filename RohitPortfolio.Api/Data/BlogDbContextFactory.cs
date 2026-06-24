using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RohitPortfolio.Api.Data;

public sealed class BlogDbContextFactory : IDesignTimeDbContextFactory<BlogDbContext>
{
    public BlogDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<BlogDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=rohit_portfolio_blog;Username=postgres;Password=postgres")
            .Options;

        return new BlogDbContext(options);
    }
}
