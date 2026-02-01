using System;

namespace ErezeptValidator.Models.ValueObjects;

/// <summary>
/// Value object representing a price identifier (Preiskennzeichen)
/// Valid codes: 11-17, 21, 90 (per TA3 Table 8.2.26)
/// </summary>
public readonly struct PriceIdentifier : IEquatable<PriceIdentifier>
{
    private readonly string _code;

    private static readonly string[] ValidCodes =
    {
        "11", "12", "13", "14", "15", "16", "17", // Standard codes
        "21",                                      // Special code
        "90"                                       // Artificial insemination
    };

    private PriceIdentifier(string code)
    {
        _code = code;
    }

    /// <summary>
    /// Create a price identifier with validation
    /// </summary>
    public static PriceIdentifier Create(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Price identifier cannot be empty", nameof(code));

        var normalized = code.Trim();

        if (!Array.Exists(ValidCodes, c => c == normalized))
            throw new ArgumentException($"Invalid price identifier: {code}. Valid codes: {string.Join(", ", ValidCodes)}", nameof(code));

        return new PriceIdentifier(normalized);
    }

    /// <summary>
    /// Try to create a price identifier
    /// </summary>
    public static bool TryCreate(string? code, out PriceIdentifier identifier)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            identifier = default;
            return false;
        }

        try
        {
            identifier = Create(code);
            return true;
        }
        catch
        {
            identifier = default;
            return false;
        }
    }

    /// <summary>
    /// Artificial insemination price identifier (90)
    /// </summary>
    public static PriceIdentifier ArtificialInsemination => new PriceIdentifier("90");

    /// <summary>
    /// Check if this is the artificial insemination code
    /// </summary>
    public bool IsArtificialInsemination => _code == "90";

    // Equality
    public bool Equals(PriceIdentifier other)
        => _code == other._code;

    public override bool Equals(object? obj)
        => obj is PriceIdentifier other && Equals(other);

    public override int GetHashCode()
        => _code.GetHashCode();

    // Operators
    public static bool operator ==(PriceIdentifier left, PriceIdentifier right)
        => left.Equals(right);

    public static bool operator !=(PriceIdentifier left, PriceIdentifier right)
        => !left.Equals(right);

    // Implicit conversion to string
    public static implicit operator string(PriceIdentifier identifier)
        => identifier._code;

    public override string ToString() => _code;
}
