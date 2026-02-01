using System;

namespace ErezeptValidator.Models.ValueObjects;

/// <summary>
/// Value object representing monetary amounts
/// Stored internally as cents (integers) for precision
/// </summary>
public readonly struct Money : IEquatable<Money>, IComparable<Money>
{
    private readonly long _cents;

    public string Currency { get; }

    private Money(long cents, string currency)
    {
        _cents = cents;
        Currency = currency ?? "EUR";
    }

    /// <summary>
    /// Create Money from Euro amount
    /// </summary>
    public static Money Euro(decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));

        return new Money((long)Math.Round(amount * 100, MidpointRounding.AwayFromZero), "EUR");
    }

    /// <summary>
    /// Create Money from cents
    /// </summary>
    public static Money FromCents(long cents, string currency = "EUR")
    {
        if (cents < 0)
            throw new ArgumentException("Cents cannot be negative", nameof(cents));

        return new Money(cents, currency);
    }

    /// <summary>
    /// Try to parse decimal as Money
    /// </summary>
    public static bool TryParse(decimal? amount, out Money money)
    {
        if (amount == null || amount < 0)
        {
            money = default;
            return false;
        }

        money = Euro(amount.Value);
        return true;
    }

    /// <summary>
    /// Zero money
    /// </summary>
    public static Money Zero => new Money(0, "EUR");

    /// <summary>
    /// Convert to decimal for display
    /// </summary>
    public decimal ToDecimal() => _cents / 100m;

    /// <summary>
    /// Add two Money amounts (same currency)
    /// </summary>
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot add {Currency} and {other.Currency}");

        return new Money(_cents + other._cents, Currency);
    }

    /// <summary>
    /// Multiply Money by a factor
    /// </summary>
    public Money Multiply(decimal factor)
    {
        return new Money((long)Math.Round(_cents * factor, MidpointRounding.AwayFromZero), Currency);
    }

    // Equality
    public bool Equals(Money other)
        => _cents == other._cents && Currency == other.Currency;

    public override bool Equals(object? obj)
        => obj is Money other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(_cents, Currency);

    // Comparison
    public int CompareTo(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException($"Cannot compare {Currency} and {other.Currency}");

        return _cents.CompareTo(other._cents);
    }

    // Operators
    public static bool operator ==(Money left, Money right) => left.Equals(right);
    public static bool operator !=(Money left, Money right) => !left.Equals(right);
    public static bool operator <(Money left, Money right) => left.CompareTo(right) < 0;
    public static bool operator >(Money left, Money right) => left.CompareTo(right) > 0;
    public static bool operator <=(Money left, Money right) => left.CompareTo(right) <= 0;
    public static bool operator >=(Money left, Money right) => left.CompareTo(right) >= 0;

    public static Money operator +(Money left, Money right) => left.Add(right);
    public static Money operator *(Money money, decimal factor) => money.Multiply(factor);

    public override string ToString() => $"{ToDecimal():F2} {Currency}";
}
