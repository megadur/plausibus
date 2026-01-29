using CsvHelper;
using CsvHelper.Configuration;
using ErezeptValidator.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace ErezeptValidator.Services.DataSeeding;

/// <summary>
/// Service to initialize the Ta1 PostgreSQL database and seed reference data.
/// </summary>
public class DatabaseInitializer
{
    private readonly ILogger<DatabaseInitializer> _logger;
    private readonly IServiceProvider _serviceProvider;

    public DatabaseInitializer(ILogger<DatabaseInitializer> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Ensures the database is created and seeded with reference data.
    /// In production, use migrations instead of EnsureCreated().
    /// </summary>
    public async Task InitializeAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<Ta1DbContext>();

        try
        {
            _logger.LogInformation("Initializing Ta1 reference data...");

            // Seed factor codes if empty
            if (!await context.FactorCodes.AnyAsync())
            {
                _logger.LogInformation("Seeding factor codes...");
                context.FactorCodes.AddRange(
                    new FactorCode { Code = "11", Content = "Stück", Description = "Pieces", UseCase = "Discrete units" },
                    new FactorCode { Code = "55", Content = "Gramm", Description = "Grams", UseCase = "Weight" },
                    new FactorCode { Code = "57", Content = "Milliliter", Description = "Milliliters", UseCase = "Volume" },
                    new FactorCode { Code = "99", Content = "Sonstiges", Description = "Other", UseCase = "Miscellaneous" }
                );
                await context.SaveChangesAsync();
            }

            // Seed price codes if empty
            if (!await context.PriceCodes.AnyAsync())
            {
                _logger.LogInformation("Seeding price codes...");
                context.PriceCodes.AddRange(
                    new PriceCode { Code = "11", Content = "AEK", Description = "Apotheken-Einkaufspreis", VatRatePercentage = 0 },
                    new PriceCode { Code = "12", Content = "AEK + Zuzahlung", Description = "AEK plus patient co-pay", VatRatePercentage = 0 },
                    new PriceCode { Code = "13", Content = "AVK", Description = "Apotheken-Verkaufspreis", VatRatePercentage = 19 },
                    new PriceCode { Code = "14", Content = "Festbetrag", Description = "Fixed fee", VatRatePercentage = 19 },
                    new PriceCode { Code = "15", Content = "Zuzahlung", Description = "Patient co-pay", VatRatePercentage = 0 },
                    new PriceCode { Code = "16", Content = "Zuzahlung + Festbetrag", Description = "Co-pay + fixed fee", VatRatePercentage = 19 },
                    new PriceCode { Code = "17", Content = "Zuzahlung + AVK", Description = "Co-pay + selling price", VatRatePercentage = 19 },
                    new PriceCode { Code = "21", Content = "Rezepturpreis", Description = "Compounding price", VatRatePercentage = 19 },
                    new PriceCode { Code = "90", Content = "Sonstiges", Description = "Other", VatRatePercentage = 19 }
                );
                await context.SaveChangesAsync();
            }

            // Seed special codes from CSV if empty
            if (!await context.SpecialCodes.AnyAsync())
            {
                _logger.LogInformation("Seeding special codes from CSV...");
                var csvPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "docs", "Abrechnung", "TA1_Anhang_1_SOK1_20250826_Sonderkennzeichen.xlsx - SOK.csv");
                if (File.Exists(csvPath))
                {
                    using var reader = new StreamReader(csvPath);
                    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                    var specialCodes = csv.GetRecords<dynamic>().Select(r => new SpecialCode
                    {
                        Code = r.SOK.ToString(),
                        Description = r.Beschreibung.ToString(),
                        VatRatePercentage = int.Parse(r.USt.ToString()),
                        ERezept = int.Parse(r["E-Rezept"].ToString())
                    }).ToList();
                    context.SpecialCodes.AddRange(specialCodes);
                    await context.SaveChangesAsync();
                    _logger.LogInformation("Seeded {Count} special codes from CSV", specialCodes.Count);
                }
                else
                {
                    _logger.LogWarning("SOK CSV file not found at {Path}", csvPath);
                }
            }

            _logger.LogInformation("Ta1 reference data initialization complete.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing Ta1 reference database");
            throw;
        }
    }
}
