using System.Text;
using System.Text.RegularExpressions;

namespace Maros.Application.Common;

public static class SlugGenerator
{
    public static string Generate(string input)
    {
        var normalized = input.Normalize(NormalizationForm.FormD);
        var withoutAccents = new string(normalized
            .Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
            .ToArray());

        var slug = withoutAccents.ToLowerInvariant();
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-").Trim('-');
        return slug;
    }
}