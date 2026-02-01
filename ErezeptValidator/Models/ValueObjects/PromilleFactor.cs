using System;

namespace ErezeptValidator.Models.ValueObjects;

/// <summary>
/// Value object representing a Promilleanteil (per mille) factor
/// Stored internally as micro-units for 6 decimal precision
/// Example: 1000.000000 = 1,000,000,000 micro-units
/// </summary>
public readonly struct PromilleFactor : IEquatable<PromilleFactor>, IComparable<PromilleFactor>
{
    private readonly long _microUnits;

    // Tolerance for comparisons (0.000001)
    private const decimal Tolerance = 0.000001m;

    private PromilleFactor(long microUnits)
    {
        _microUnits = microUnits;
    }

    /// <summary>
    /// Create from decimal value
    /// </summary>
    public static PromilleFactor FromDecimal(decimal value)
    {
        if (value < 0)
            throw new ArgumentException("Factor cannot be negative", nameof(value));

        return new PromilleFactor((long)Math.Round(value * 1_000_000, MidpointRounding.AwayFromZero));
    }

    /// <summary>
    /// Calculate from ratio: (dispensed / package) × 1000
    /// </summary>
    public static PromilleFactor FromRatio(Quantity dispensed, Quantity package)
    {
        if (package.Value <= 0)
            throw new ArgumentException("Package quantity must be positive", nameof(package));

        var factor = (dispensed.Value / package.Value) * 1000m;
        return FromDecimal(factor);
    }

    /// <summary>
    /// Try to parse decimal as PromilleFactor
    /// </summary>
    public static bool TryParse(decimal? value, out PromilleFactor factor)
    {
        if (value == null || value < 0)
        {
            factor = default;
            return false;
        }

        factor = FromDecimal(value.Value);
        return true;
    }

    /// <summary>
    /// Special factor for codes without quantity reference (always 1.0)
    /// </summary>
    public static PromilleFactor One => FromDecimal(1m);

    /// <summary>
    /// Artificial insemination marker factor (always 1000.0)
    /// </summary>
    public static PromilleFactor ArtificialInsemination => FromDecimal(1000m);

    /// <summary>
    /// Convert to decimal for display
    /// </summary>
    public decimal ToDecimal() => _microUnits / 1_000_000m;

    /// <summary>
    /// Check if this factor equals another within tolerance
    /// </summary>
    public bool EqualsWithinTolerance(PromilleFactor other)
    {
        var diff = Math.Abs(ToDecimal() - other.ToDecimal());
        return diff < Tolerance;
    }

    // Equality (exact comparison)
    public bool Equals(PromilleFactor other)
        => _microUnits == other._microUnits;

    public override bool Equals(object? obj)
        => obj is PromilleFactor other && Equals(other);

    public override int GetHashCode()
        => _microUnits.GetHashCode();

    // Comparison
    public int CompareTo(PromilleFactor other)
        => _microUnits.CompareTo(other._microUnits);

    // Operators
    public static bool operator ==(PromilleFactor left, PromilleFactor right)
        => left.Equals(right);

    public static bool operator !=(PromilleFactor left, PromilleFactor right)
        => !left.Equals(right);

    public static bool operator <(PromilleFactor left, PromilleFactor right)
        => left.CompareTo(right) < 0;

    public static bool operator >(PromilleFactor left, PromilleFactor right)
        => left.CompareTo(right) > 0;

    // Format with up to 6 decimal places, removing trailing zeros
    public override string ToString() => ToDecimal().ToString("0.######");
}
