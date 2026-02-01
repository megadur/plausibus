using System;
using System.Linq;

namespace ErezeptValidator.Models.ValueObjects;

/// <summary>
/// Value object representing a PZN (Pharmazentralnummer)
/// 8-digit identifier with Modulo 11 checksum validation
/// </summary>
public readonly struct Pzn : IEquatable<Pzn>
{
    private readonly string _value;

    private Pzn(string value)
    {
        _value = value;
    }

    /// <summary>
    /// Create a PZN with full validation
    /// </summary>
    public static Pzn Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("PZN cannot be empty", nameof(value));

        // Normalize: trim and pad to 8 digits
        var normalized = value.Trim().PadLeft(8, '0');

        // Validate format
        if (normalized.Length != 8)
            throw new ArgumentException($"PZN must be 8 digits, got: {value}", nameof(value));

        if (!normalized.All(char.IsDigit))
            throw new ArgumentException($"PZN must contain only digits, got: {value}", nameof(value));

        // Validate checksum
        if (!ValidateChecksum(normalized))
            throw new ArgumentException($"PZN checksum is invalid: {value}", nameof(value));

        return new Pzn(normalized);
    }

    /// <summary>
    /// Create a PZN without checksum validation (for testing/special cases)
    /// </summary>
    public static Pzn CreateWithoutValidation(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("PZN cannot be empty", nameof(value));

        var normalized = value.Trim().PadLeft(8, '0');

        if (normalized.Length != 8 || !normalized.All(char.IsDigit))
            throw new ArgumentException($"PZN must be 8 digits, got: {value}", nameof(value));

        return new Pzn(normalized);
    }

    /// <summary>
    /// Try to create a PZN
    /// </summary>
    public static bool TryCreate(string? value, out Pzn pzn)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            pzn = default;
            return false;
        }

        try
        {
            pzn = Create(value);
            return true;
        }
        catch
        {
            pzn = default;
            return false;
        }
    }

    /// <summary>
    /// Validate PZN format (8 digits)
    /// </summary>
    public static bool IsValidFormat(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var normalized = value.Trim().PadLeft(8, '0');
        return normalized.Length == 8 && normalized.All(char.IsDigit);
    }

    /// <summary>
    /// Validate Modulo 11 checksum
    /// </summary>
    private static bool ValidateChecksum(string pzn)
    {
        if (pzn.Length != 8)
            return false;

        int sum = 0;
        for (int i = 0; i < 7; i++)
        {
            sum += (pzn[i] - '0') * (i + 2);
        }

        int checkDigit = sum % 11;
        int lastDigit = pzn[7] - '0';

        return checkDigit == lastDigit;
    }

    /// <summary>
    /// Check if this PZN has a valid checksum
    /// </summary>
    public bool HasValidChecksum => ValidateChecksum(_value);

    // Equality
    public bool Equals(Pzn other)
        => _value == other._value;

    public override bool Equals(object? obj)
        => obj is Pzn other && Equals(other);

    public override int GetHashCode()
        => _value.GetHashCode();

    // Operators
    public static bool operator ==(Pzn left, Pzn right)
        => left.Equals(right);

    public static bool operator !=(Pzn left, Pzn right)
        => !left.Equals(right);

    // Implicit conversion to string
    public static implicit operator string(Pzn pzn)
        => pzn._value;

    public override string ToString() => _value;
}
