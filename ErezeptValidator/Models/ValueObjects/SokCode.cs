using System;
using System.Collections.Generic;

namespace ErezeptValidator.Models.ValueObjects;

/// <summary>
/// Value object representing a SOK (Sonderkennzeichen) special code
/// </summary>
public readonly struct SokCode : IEquatable<SokCode>
{
    private readonly string _code;

    // Special codes without quantity reference (CALC-002)
    private static readonly HashSet<string> CodesWithoutQuantityReference = new()
    {
        "1.1.1", "1.1.2", "1.2.1", "1.2.2",
        "1.3.1", "1.3.2",
        "1.6.5",
        "1.10.2", "1.10.3"
    };

    private SokCode(string code)
    {
        _code = code;
    }

    /// <summary>
    /// Create a SOK code
    /// </summary>
    public static SokCode Create(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("SOK code cannot be empty", nameof(code));

        return new SokCode(code.Trim());
    }

    /// <summary>
    /// Try to create a SOK code
    /// </summary>
    public static bool TryCreate(string? code, out SokCode sokCode)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            sokCode = default;
            return false;
        }

        sokCode = Create(code);
        return true;
    }

    /// <summary>
    /// Artificial insemination marker code
    /// </summary>
    public static SokCode ArtificialInsemination => new SokCode("09999643");

    /// <summary>
    /// Check if this code has no quantity reference (requires factor 1.0)
    /// </summary>
    public bool HasNoQuantityReference => CodesWithoutQuantityReference.Contains(_code);

    /// <summary>
    /// Check if this is the artificial insemination marker
    /// </summary>
    public bool IsArtificialInsemination => _code == "09999643";

    /// <summary>
    /// Check if this is a compounding (Rezeptur) special code
    /// Compounding codes: 06460702 (standard), 09999011 (alternative)
    /// Reference: TA1 REZ-013
    /// </summary>
    public bool IsCompounding => _code is "06460702" or "09999011";

    /// <summary>
    /// Get the artificial insemination code constant
    /// </summary>
    private static string ArtificialInseminationCode => "09999643";

    // Equality
    public bool Equals(SokCode other)
        => _code == other._code;

    public override bool Equals(object? obj)
        => obj is SokCode other && Equals(other);

    public override int GetHashCode()
        => _code.GetHashCode();

    // Operators
    public static bool operator ==(SokCode left, SokCode right)
        => left.Equals(right);

    public static bool operator !=(SokCode left, SokCode right)
        => !left.Equals(right);

    // Implicit conversion to string
    public static implicit operator string(SokCode code)
        => code._code;

    public override string ToString() => _code;
}
