namespace backend_bicigo.Iam.Domain.Model.ValueObjects;

/// <summary>
///     Value object representing a user e-mail address.
/// </summary>
public sealed record Email
{
    private const int MaxLength = 255;

    /// <summary>Initializes a new <see cref="Email"/> with the given raw string value.</summary>
    /// <param name="value">The raw e-mail. Must be non-null, non-whitespace and at most 255 characters.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is null, empty, whitespace, or too long.</exception>
    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be null, empty, or whitespace.", nameof(value));
        if (value.Length > MaxLength)
            throw new ArgumentException($"Email cannot be longer than {MaxLength} characters.", nameof(value));
        if (!value.Contains('@'))
            throw new ArgumentException("Email must contain '@'.", nameof(value));
        Value = value.Trim().ToLowerInvariant();
    }

    /// <summary>Gets the underlying primitive value.</summary>
    public string Value { get; }

    /// <inheritdoc />
    public override string ToString() => Value;
}