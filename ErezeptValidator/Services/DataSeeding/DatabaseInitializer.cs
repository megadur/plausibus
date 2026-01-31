using ErezeptValidator.Data.Contexts;
using ErezeptValidator.Models.Ta1Reference;
using Microsoft.EntityFrameworkCore;

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
                    new PriceCode { Code = "11", Content = "AEK", Description = "Apotheken-Einkaufspreis", TaxStatus = "excl. VAT" },
                    new PriceCode { Code = "12", Content = "AEK + Zuzahlung", Description = "AEK plus patient co-pay", TaxStatus = "excl. VAT" },
                    new PriceCode { Code = "13", Content = "AVK", Description = "Apotheken-Verkaufspreis", TaxStatus = "incl. VAT" },
                    new PriceCode { Code = "14", Content = "Festbetrag", Description = "Fixed fee", TaxStatus = "incl. VAT" },
                    new PriceCode { Code = "15", Content = "Zuzahlung", Description = "Patient co-pay", TaxStatus = "excl. VAT" },
                    new PriceCode { Code = "16", Content = "Zuzahlung + Festbetrag", Description = "Co-pay + fixed fee", TaxStatus = "incl. VAT" },
                    new PriceCode { Code = "17", Content = "Zuzahlung + AVK", Description = "Co-pay + selling price", TaxStatus = "incl. VAT" },
                    new PriceCode { Code = "21", Content = "Rezepturpreis", Description = "Compounding price", TaxStatus = "incl. VAT" },
                    new PriceCode { Code = "90", Content = "Sonstiges", Description = "Other", TaxStatus = "incl. VAT" }
                );
                await context.SaveChangesAsync();
            }

            // Seed special codes from CSV if empty
            if (!await context.SpecialCodes.AnyAsync())
            {
                _logger.LogInformation("Loading special codes from CSV files...");
                var sokLoader = scope.ServiceProvider.GetRequiredService<SokCodeLoader>();
                var (sok1Count, sok2Count) = await sokLoader.LoadAllCodesAsync();
                _logger.LogInformation("Loaded {Sok1Count} SOK1 codes and {Sok2Count} SOK2 codes", sok1Count, sok2Count);
            }
            else
            {
                var totalCodes = await context.SpecialCodes.CountAsync();
                _logger.LogInformation("Special codes already exist in database ({Count} total)", totalCodes);
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
