using Markdig;

namespace RohitPortfolio.Api.Services;

public sealed class MarkdownService
{
    private readonly MarkdownPipeline _pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .DisableHtml()
        .Build();

    public string ToHtml(string markdown) => Markdown.ToHtml(markdown, _pipeline);

    public int EstimateReadingMinutes(string markdown)
    {
        var words = markdown
            .Split([' ', '\r', '\n', '\t'], StringSplitOptions.RemoveEmptyEntries)
            .Length;

        return Math.Max(1, (int)Math.Ceiling(words / 220d));
    }
}
