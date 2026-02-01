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

    /// <summary>
    /// Check if this price identifier uses ABDATA base price for calculation
    /// (codes 11, 13, 14 map to ABDATA; 12, 15-17, 21 are custom contracted prices)
    /// </summary>
    public bool UsesAbdataBasePrice => _code is "11" or "13" or "14";

    /// <summary>
    /// Get which ABDATA price field to use for CALC-004 calculation
    /// </summary>
    /// <returns>
    /// "AEK" for codes 11 (purchase price per AMPreisV) and 13 (actual purchase price)
    /// "AVK" for code 14 (billing price with surcharges)
    /// null for contracted/special prices that don't use ABDATA
    /// </returns>
    public string? GetAbdataPriceField()
    {
        return _code switch
        {
            "11" => "AEK", // Pharmacy purchase price per AMPreisV
            "13" => "AEK", // Actual pharmacy purchase price
            "14" => "AVK", // Billing price per AMPreisV §§ 4,5
            "90" => null,  // Special price (0.00)
            _ => null      // Contracted prices (12, 15, 16, 17, 21) - not in ABDATA
        };
    }

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
