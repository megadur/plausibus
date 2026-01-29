using ErezeptValidator.Data.Contexts;
using ErezeptValidator.Models.Ta1Reference;
using Microsoft.EntityFrameworkCore;

namespace ErezeptValidator.Tests.Fixtures;

/// <summary>
/// Fixture for integration tests that need a real database
/// Uses in-memory EF Core database for fast, isolated testing
/// </summary>
public class DatabaseFixture : IDisposable
{
    public Ta1DbContext Context { get; private set; }

    public DatabaseFixture()
    {
        var options = new DbContextOptionsBuilder<Ta1DbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        Context = new Ta1DbContext(options);
        Context.Database.EnsureCreated();

        SeedTestData();
    }

    private void SeedTestData()
    {
        // Seed some test special codes
        Context.SpecialCodes.AddRange(
            new SpecialCode
            {
                Code = "09999005",
                Description = "Test SOK Code - E-Rezept Compatible",
                CodeType = "SOK1",
                VatRate = 2,
                ERezept = 1,
                Category = "medication"
            },
            new SpecialCode
            {
                Code = "09999006",
                Description = "Test SOK Code - Not E-Rezept Compatible",
                CodeType = "SOK1",
                VatRate = 2,
                ERezept = 0,
                Category = "medication"
            }
        );

        // Seed some test price codes
        Context.PriceCodes.AddRange(
            new PriceCode
            {
                Code = "11",
                Content = "AEK",
                Description = "Apotheken-Einkaufspreis",
                TaxStatus = "excl. VAT"
            },
            new PriceCode
            {
                Code = "13",
                Content = "AVK",
                Description = "Apotheken-Verkaufspreis",
                TaxStatus = "incl. VAT"
            }
        );

        // Seed some test factor codes
        Context.FactorCodes.AddRange(
            new FactorCode
            {
                Code = "11",
                Description = "Promilleanteil"
            },
            new FactorCode
            {
                Code = "12",
                Description = "Prozentanteil"
            }
        );

        Context.SaveChanges();
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }
}
