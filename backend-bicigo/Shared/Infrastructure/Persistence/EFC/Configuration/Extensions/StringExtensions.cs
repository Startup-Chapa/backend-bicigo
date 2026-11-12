namespace backend_bicigo.Shared.Infrastructure.Persistence.EFC.Configuration.Extensions;

/// <summary>
///     String extensions for persistence naming conventions.
/// </summary>
/// <remarks>
///     Provides snake_case conversion and pluralization helpers used by
///     <see cref="ModelBuilderExtensions"/> to configure consistent table and column names
///     across all bounded contexts.
/// </remarks>
public static class StringExtensions
{
    /// <summary>Convert a PascalCase/camelCase string to snake_case.</summary>
    /// <param name="text">The string to convert.</param>
    /// <returns>The snake_case representation.</returns>
    public static string ToSnakeCase(this string text)
    {
        if (string.IsNullOrEmpty(text)) return text;
        var builder = new System.Text.StringBuilder(text.Length + 16);
        builder.Append(char.ToLower(text[0]));
        for (var i = 1; i < text.Length; i++)
        {
            if (char.IsUpper(text[i]))
            {
                builder.Append('_');
                builder.Append(char.ToLower(text[i]));
            }
            else
            {
                builder.Append(text[i]);
            }
        }
        return builder.ToString();
    }

    /// <summary>Pluralize a table-name string using simple English rules.</summary>
    /// <param name="text">The singular name.</param>
    /// <returns>The pluralized name.</returns>
    public static string ToPlural(this string text)
    {
        if (string.IsNullOrEmpty(text)) return text;
        if (text.EndsWith("s", StringComparison.OrdinalIgnoreCase)) return text + "es";
        if (text.EndsWith("y", StringComparison.OrdinalIgnoreCase)
            && text.Length >= 2
            && !IsVowel(text[^2])) return text[..^1] + "ies";
        return text + "s";
    }

    private static bool IsVowel(char c) => char.ToLowerInvariant(c) is 'a' or 'e' or 'i' or 'o' or 'u';
}
