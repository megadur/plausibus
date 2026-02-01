using System;

namespace ErezeptValidator.Models.ValueObjects;

/// <summary>
/// Value object representing a quantity (e.g., tablets, grams, milliliters)
/// </summary>
public readonly struct Quantity : IEquatable<Quantity>, IComparable<Quantity>
{
    public decimal Value { get; }
    public string? Unit { get; }

    private Quantity(decimal value, string? unit = null)
    {
        Value = value;
        Unit = unit;
    }

    /// <summary>
    /// Create a quantity
    /// </summary>
    public static Quantity Of(decimal value, string? unit = null)
    {
        if (value < 0)
            throw new ArgumentException("Quantity cannot be negative", nameof(value));

        return new Quantity(value, unit);
    }

    /// <summary>
    /// Try to parse decimal as Quantity
    /// </summary>
    public static bool TryParse(decimal? value, out Quantity quantity, string? unit = null)
    {
        if (value == null || value < 0)
        {
            quantity = default;
            return false;
        }

        quantity = Of(value.Value, unit);
        return true;
    }

    /// <summary>
    /// Zero quantity
    /// </summary>
    public static Quantity Zero => new Quantity(0);

    /// <summary>
    /// Check if quantity is zero
    /// </summary>
    public bool IsZero => Value == 0;

    /// <summary>
    /// Divide this quantity by another (for ratio calculations)
    /// </summary>
    public decimal DivideBy(Quantity other)
    {
        if (other.Value == 0)
            throw new DivideByZeroException("Cannot divide by zero quantity");

        return Value / other.Value;
    }

    // Equality
    public bool Equals(Quantity other)
        => Value == other.Value && Unit == other.Unit;

    public override bool Equals(object? obj)
        => obj is Quantity other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(Value, Unit);

    // Comparison (only compares value, not unit)
    public int CompareTo(Quantity other)
        => Value.CompareTo(other.Value);

    // Operators
    public static bool operator ==(Quantity left, Quantity right)
        => left.Equals(right);

    public static bool operator !=(Quantity left, Quantity right)
        => !left.Equals(right);

    public static bool operator <(Quantity left, Quantity right)
        => left.CompareTo(right) < 0;

    public static bool operator >(Quantity left, Quantity right)
        => left.CompareTo(right) > 0;

    public override string ToString()
        => string.IsNullOrEmpty(Unit) ? Value.ToString() : $"{Value} {Unit}";
}
