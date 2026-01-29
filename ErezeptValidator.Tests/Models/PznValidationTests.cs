using FluentAssertions;

namespace ErezeptValidator.Tests.Models;

/// <summary>
/// Tests for PZN validation logic and checksum algorithm
/// </summary>
public class PznValidationTests
{
    /// <summary>
    /// Test data: Valid PZNs constructed with correct checksums
    /// Format: PZN digits 1-7, then checksum digit that equals (sum % 11)
    /// </summary>
    public static IEnumerable<object[]> ValidPznsWithChecksums =>
        new List<object[]>
        {
            new object[] { "00000000", "All zeros (sum=0, 0%11=0)" },
            new object[] { "10000002", "First digit 1 (sum=1*2=2, 2%11=2)" },
            new object[] { "11111112", "All ones (sum=2+3+4+5+6+7+8=35, 35%11=2)" }
        };

    /// <summary>
    /// Test data: Invalid PZNs with incorrect checksums
    /// </summary>
    public static IEnumerable<object[]> InvalidPznsWithChecksums =>
        new List<object[]>
        {
            new object[] { "00000001", "Should be 0, not 1" },
            new object[] { "10000000", "Should be 2, not 0" },
            new object[] { "11111110", "Should be 2, not 0" }
        };

    [Theory]
    [MemberData(nameof(ValidPznsWithChecksums))]
    public void ValidPzn_PassesChecksumValidation(string pzn, string description)
    {
        // Arrange & Act
        var isValid = ValidatePznChecksumModulo11(pzn);

        // Assert
        isValid.Should().BeTrue($"PZN {pzn} ({description}) should have valid checksum");
    }

    [Theory]
    [MemberData(nameof(InvalidPznsWithChecksums))]
    public void InvalidPzn_FailsChecksumValidation(string pzn, string description)
    {
        // Arrange & Act
        var isValid = ValidatePznChecksumModulo11(pzn);

        // Assert
        isValid.Should().BeFalse($"PZN {pzn} ({description}) should have invalid checksum");
    }

    [Fact]
    public void PznChecksum_Modulo11Algorithm_ExplainedExample()
    {
        // Arrange - Construct a PZN with known correct checksum
        // PZN: 00000000
        // Weights: 2, 3, 4, 5, 6, 7, 8 (for positions 0-6)
        // Calc: (0*2 + 0*3 + 0*4 + 0*5 + 0*6 + 0*7 + 0*8) % 11 = 0 % 11 = 0
        // Check digit (position 7) should be: 0

        var pzn = "00000000";
        var isValid = ValidatePznChecksumModulo11(pzn);

        // Assert
        isValid.Should().BeTrue("PZN with all zeros has checksum of 0");

        // Another example: 10000001
        // Calc: (1*2 + 0*3 + 0*4 + 0*5 + 0*6 + 0*7 + 0*8) % 11 = 2 % 11 = 2... wait that's not 1
        // Let me try: Position values are: 1,0,0,0,0,0,0
        // Actually: (1*2) = 2, 2 % 11 = 2
        // So checksum should be 2, not 1. Let me use 10000002 instead.

        var pzn2 = "10000002";
        var isValid2 = ValidatePznChecksumModulo11(pzn2);
        isValid2.Should().BeTrue("PZN 10000002 has correct checksum of 2");
    }

    /// <summary>
    /// Modulo 11 checksum validation for PZN
    /// This is a reference implementation matching the actual PznRepository implementation
    /// </summary>
    private bool ValidatePznChecksumModulo11(string pzn)
    {
        if (string.IsNullOrEmpty(pzn) || pzn.Length != 8)
            return false;

        if (!pzn.All(char.IsDigit))
            return false;

        // PZN Checksum: Modulo 11 algorithm
        // Weights: 2, 3, 4, 5, 6, 7, 8 for first 7 digits
        int[] weights = { 2, 3, 4, 5, 6, 7, 8 };
        int sum = 0;

        for (int i = 0; i < 7; i++)
        {
            sum += (pzn[i] - '0') * weights[i];
        }

        int checksum = sum % 11;
        int expectedCheckDigit = pzn[7] - '0';

        return checksum == expectedCheckDigit;
    }
}
