using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace RohitPortfolio.Api.Services;

public static partial class SlugService
{
    public static string Create(string value)
    {
        var normalized = value.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);

        foreach (var character in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(character);
            }
        }

        var slug = NonSlugCharacters().Replace(builder.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant(), "-");
        slug = RepeatedDashes().Replace(slug, "-").Trim('-');

        return string.IsNullOrWhiteSpace(slug) ? $"post-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}" : slug;
    }

    [GeneratedRegex("[^a-z0-9]+")]
    private static partial Regex NonSlugCharacters();

    [GeneratedRegex("-{2,}")]
    private static partial Regex RepeatedDashes();
}
