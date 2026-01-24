using ErezeptValidator.Data.Contexts;
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
            _logger.LogInformation("Checking Ta1 reference database...");

            // Check if database exists
            var canConnect = await context.Database.CanConnectAsync();

            if (!canConnect)
            {
                _logger.LogInformation("Creating Ta1 reference database...");
                await context.Database.EnsureCreatedAsync();
                _logger.LogInformation("Ta1 reference database created successfully");
            }
            else
            {
                _logger.LogInformation("Ta1 reference database already exists");

                // Apply any pending migrations (for future use)
                var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    _logger.LogInformation("Applying {Count} pending migrations...", pendingMigrations.Count());
                    await context.Database.MigrateAsync();
                    _logger.LogInformation("Migrations applied successfully");
                }
            }

            // Verify seed data
            var factorCount = await context.FactorCodes.CountAsync();
            var priceCount = await context.PriceCodes.CountAsync();

            _logger.LogInformation("Ta1 reference data status:");
            _logger.LogInformation("  - Factor codes: {FactorCount}", factorCount);
            _logger.LogInformation("  - Price codes: {PriceCount}", priceCount);
            _logger.LogInformation("  - Special codes: {SpecialCount}", await context.SpecialCodes.CountAsync());

            if (factorCount == 0 || priceCount == 0)
            {
                _logger.LogWarning("Reference data appears incomplete. Expected 4 factor codes and 9 price codes.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing Ta1 reference database");
            throw;
        }
    }
}
