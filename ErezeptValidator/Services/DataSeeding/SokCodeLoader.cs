using CsvHelper;
using CsvHelper.Configuration;
using ErezeptValidator.Data.Contexts;
using ErezeptValidator.Models.Ta1Reference;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace ErezeptValidator.Services.DataSeeding;

/// <summary>
/// Service for loading SOK (Sonderkennzeichen) codes from CSV files into the database.
/// Handles both SOK1 (standard federal codes) and SOK2 (contract-specific codes).
/// </summary>
public class SokCodeLoader
{
    private readonly Ta1DbContext _context;
    private readonly ILogger<SokCodeLoader> _logger;

    public SokCodeLoader(Ta1DbContext context, ILogger<SokCodeLoader> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Load all SOK codes from CSV files into the database
    /// </summary>
    public async Task<(int sok1Count, int sok2Count)> LoadAllCodesAsync()
    {
        var sok1Count = await LoadSok1CodesAsync();
        var sok2Count = await LoadSok2CodesAsync();

        _logger.LogInformation("Successfully loaded {Sok1Count} SOK1 codes and {Sok2Count} SOK2 codes",
            sok1Count, sok2Count);

        return (sok1Count, sok2Count);
    }

    /// <summary>
    /// Load SOK1 codes from CSV file
    /// </summary>
    private async Task<int> LoadSok1CodesAsync()
    {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            "..", "..", "..", "..", "docs", "Abrechnung",
            "TA1_Anhang_1_SOK1_20250826_Sonderkennzeichen.xlsx - SOK.csv");

        if (!File.Exists(filePath))
        {
            _logger.LogWarning("SOK1 CSV file not found at {FilePath}", filePath);
            return 0;
        }

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ",",
            HasHeaderRecord = true,
            TrimOptions = TrimOptions.Trim,
            MissingFieldFound = null,
            PrepareHeaderForMatch = args => args.Header
                .Replace(".", "")
                .Replace("-", "")
                .Replace(" ", "")
                .Replace("\n", "")
                .Replace("ä", "ae")
                .Replace("ö", "oe")
                .Replace("ü", "ue")
                .Replace("Ä", "ae")
                .Replace("Ö", "oe")
                .Replace("Ü", "ue")
                .Replace("ß", "ss")
                .ToLower()
        };

        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, config);

        var records = csv.GetRecords<Sok1CsvRecord>().ToList();
        var specialCodes = new List<SpecialCode>();

        foreach (var record in records)
        {
            try
            {
                var specialCode = new SpecialCode
                {
                    Code = record.SOK,
                    Description = record.Beschreibung ?? string.Empty,
                    CodeType = "SOK1",
                    VatRate = ParseVatRate(record.USt),
                    ERezept = ParseERezept(record.ERezept),
                    PharmacyDiscount = record.Apothekenrabatt,
                    RequiresAdditionalData = ParseAdditionalDataRequired(record.Zusatzdaten),
                    ValidFromBillingMonth = ParseBillingMonth(record.GueltigAbAbrechnungsmonat),
                    ValidFromDispensingDate = ParseDate(record.GueltigAbAbgabedatum),
                    ExpiredBillingMonth = ParseBillingMonth(record.AusserkrafttretenAbrechnungsmonat),
                    ExpiredDispensingDate = ParseDate(record.AusserkrafttretenAbgabedatum),
                    Notes = record.Kommentar,
                    Category = DetermineCategory(record.Beschreibung ?? string.Empty),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                specialCodes.Add(specialCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing SOK1 record: {Code}", record.SOK);
            }
        }

        // Remove existing SOK1 codes first
        await _context.SpecialCodes.Where(s => s.CodeType == "SOK1").ExecuteDeleteAsync();

        // Remove duplicates (keep first occurrence)
        var uniqueCodes = specialCodes
            .GroupBy(s => s.Code)
            .Select(g => g.First())
            .ToList();

        if (uniqueCodes.Count < specialCodes.Count)
        {
            _logger.LogWarning("Removed {DuplicateCount} duplicate SOK1 codes",
                specialCodes.Count - uniqueCodes.Count);
        }

        // Add new codes
        await _context.SpecialCodes.AddRangeAsync(uniqueCodes);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Loaded {Count} unique SOK1 codes from CSV", uniqueCodes.Count);
        return uniqueCodes.Count;
    }

    /// <summary>
    /// Load SOK2 codes from CSV file
    /// </summary>
    private async Task<int> LoadSok2CodesAsync()
    {
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            "..", "..", "..", "..", "docs", "Abrechnung",
            "TA1_Anhang_2_SOK2_20260115_Zwischen_Krankenkassen_und_Apotheken_vereinbarte_Sonderkennzeichen.xlsx - SOK.csv");

        if (!File.Exists(filePath))
        {
            _logger.LogWarning("SOK2 CSV file not found at {FilePath}", filePath);
            return 0;
        }

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ",",
            HasHeaderRecord = true,
            TrimOptions = TrimOptions.Trim,
            MissingFieldFound = null,
            PrepareHeaderForMatch = args => args.Header
                .Replace(".", "")
                .Replace("-", "")
                .Replace(" ", "")
                .Replace("\n", "")
                .Replace("ä", "ae")
                .Replace("ö", "oe")
                .Replace("ü", "ue")
                .Replace("Ä", "ae")
                .Replace("Ö", "oe")
                .Replace("Ü", "ue")
                .Replace("ß", "ss")
                .ToLower()
        };

        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, config);

        var records = csv.GetRecords<Sok2CsvRecord>().ToList();
        var specialCodes = new List<SpecialCode>();

        foreach (var record in records)
        {
            try
            {
                var specialCode = new SpecialCode
                {
                    Code = record.SOK,
                    Description = record.Beschreibung ?? string.Empty,
                    CodeType = "SOK2",
                    VatRate = ParseVatRate(record.USt),
                    ERezept = 1, // Default: SOK2 codes are typically compatible unless marked otherwise
                    PharmacyDiscount = record.Apothekenrabatt,
                    RequiresAdditionalData = false, // SOK2 doesn't have this field
                    ValidFromBillingMonth = ParseBillingMonth(record.GueltigAbAbrechnungsmonat),
                    ValidFromDispensingDate = ParseDate(record.GueltigAbAbgabedatum),
                    ExpiredBillingMonth = ParseBillingMonth(record.AusserkrafttretenAbrechnungsmonat),
                    ExpiredDispensingDate = ParseDate(record.AusserkrafttretenAbgabedatum),
                    IssuedTo = record.VergebenAn,
                    IssuedDate = ParseIssuedDate(record.Vergabedatum),
                    Category = "contract", // SOK2 codes are contract-specific
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                specialCodes.Add(specialCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing SOK2 record: {Code}", record.SOK);
            }
        }

        // Remove existing SOK2 codes first
        await _context.SpecialCodes.Where(s => s.CodeType == "SOK2").ExecuteDeleteAsync();

        // Remove duplicates (keep first occurrence)
        var uniqueCodes = specialCodes
            .GroupBy(s => s.Code)
            .Select(g => g.First())
            .ToList();

        if (uniqueCodes.Count < specialCodes.Count)
        {
            _logger.LogWarning("Removed {DuplicateCount} duplicate SOK2 codes",
                specialCodes.Count - uniqueCodes.Count);
        }

        // Add new codes
        await _context.SpecialCodes.AddRangeAsync(uniqueCodes);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Loaded {Count} unique SOK2 codes from CSV", uniqueCodes.Count);
        return uniqueCodes.Count;
    }

    #region Helper Methods

    /// <summary>
    /// Parse VAT rate from CSV (0, 1, 2, "-", or empty)
    /// </summary>
    private short? ParseVatRate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value == "-")
            return null;

        if (short.TryParse(value, out var rate) && rate >= 0 && rate <= 2)
            return rate;

        return null;
    }

    /// <summary>
    /// Parse E-Rezept compatibility flag (0, 1, 2)
    /// </summary>
    private short ParseERezept(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return 0; // Default: not compatible

        if (short.TryParse(value, out var flag) && flag >= 0 && flag <= 2)
            return flag;

        return 0;
    }

    /// <summary>
    /// Parse additional data required flag (Zusatzdaten: 0, 1, 2, 4)
    /// </summary>
    private bool ParseAdditionalDataRequired(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        if (int.TryParse(value, out var code))
            return code > 0; // Any value > 0 means additional data required

        return false;
    }

    /// <summary>
    /// Parse billing month format (e.g., "12/2024" → "2024-12", "bereits gültig" → null)
    /// </summary>
    private string? ParseBillingMonth(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            value.Contains("bereits", StringComparison.OrdinalIgnoreCase) ||
            value.Contains("nach", StringComparison.OrdinalIgnoreCase))
            return null;

        // Try parsing MM/YYYY format
        var parts = value.Split('/');
        if (parts.Length == 2 &&
            int.TryParse(parts[0].Trim(), out var month) &&
            int.TryParse(parts[1].Trim(), out var year))
        {
            return $"{year:D4}-{month:D2}";
        }

        return null;
    }

    /// <summary>
    /// Parse date from various formats in CSV
    /// </summary>
    private DateOnly? ParseDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            value.Contains("bereits", StringComparison.OrdinalIgnoreCase) ||
            value.Contains("nach", StringComparison.OrdinalIgnoreCase))
            return null;

        // Try dd.MM.yyyy format (German standard)
        if (DateOnly.TryParseExact(value, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date1))
            return date1;

        // Try dd/MM/yyyy format
        if (DateOnly.TryParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date2))
            return date2;

        // Try d.M.yyyy format (single digit day/month)
        if (DateOnly.TryParseExact(value, "d.M.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date3))
            return date3;

        return null;
    }

    /// <summary>
    /// Parse issued date from SOK2 format (e.g., "26/8/2005")
    /// </summary>
    private DateOnly? ParseIssuedDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        // Try d/M/yyyy format
        if (DateOnly.TryParseExact(value, "d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            return date;

        // Try dd/MM/yyyy format
        if (DateOnly.TryParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date2))
            return date2;

        return null;
    }

    /// <summary>
    /// Determine category from description (simple keyword matching)
    /// </summary>
    private string? DetermineCategory(string description)
    {
        var lowerDesc = description.ToLowerInvariant();

        if (lowerDesc.Contains("cannabis") || lowerDesc.Contains("btm"))
            return "cannabis";
        if (lowerDesc.Contains("rezeptur") || lowerDesc.Contains("zubereitung"))
            return "compounding";
        if (lowerDesc.Contains("fertigarzneimittel") || lowerDesc.Contains("homöopathika"))
            return "medication";
        if (lowerDesc.Contains("notfall") || lowerDesc.Contains("notdienst"))
            return "fee";
        if (lowerDesc.Contains("beratung") || lowerDesc.Contains("service"))
            return "service";

        return "other";
    }

    #endregion

    #region CSV Record Classes

    /// <summary>
    /// CSV record for SOK1 codes
    /// </summary>
    private class Sok1CsvRecord
    {
        public string Nr { get; set; } = string.Empty;
        public string SOK { get; set; } = string.Empty;
        public string? Beschreibung { get; set; }
        public string? USt { get; set; }
        public string? ERezept { get; set; }
        public string? Apothekenrabatt { get; set; }
        public string? Zusatzdaten { get; set; }
        public string? GueltigAbAbrechnungsmonat { get; set; }
        public string? GueltigAbAbgabedatum { get; set; }
        public string? AusserkrafttretenAbrechnungsmonat { get; set; }
        public string? AusserkrafttretenAbgabedatum { get; set; }
        public string? Kommentar { get; set; }
    }

    /// <summary>
    /// CSV record for SOK2 codes
    /// </summary>
    private class Sok2CsvRecord
    {
        public string SOK { get; set; } = string.Empty;
        public string? Beschreibung { get; set; }
        public string? VergebenAn { get; set; }
        public string? Vergabedatum { get; set; }
        public string? GueltigAbAbrechnungsmonat { get; set; }
        public string? GueltigAbAbgabedatum { get; set; }
        public string? AusserkrafttretenAbrechnungsmonat { get; set; }
        public string? AusserkrafttretenAbgabedatum { get; set; }
        public string? USt { get; set; }
        public string? Apothekenrabatt { get; set; }
    }

    #endregion
}
