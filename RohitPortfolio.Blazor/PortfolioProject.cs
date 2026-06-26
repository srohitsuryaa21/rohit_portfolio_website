namespace RohitPortfolio.Blazor;

public sealed record PortfolioProject(
    string Number,
    string Category,
    string TitleEn,
    string TitleDe,
    string DescriptionEn,
    string DescriptionDe,
    string[] Tags,
    string? GitHubUrl,
    string? PaperUrl,
    string Metric,
    string MetricLabel,
    string Challenge,
    string Approach,
    string Outcome,
    string VisualClass,
    string? DemoUrl = null);
