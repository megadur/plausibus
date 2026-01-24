using Microsoft.EntityFrameworkCore;
using ErezeptValidator.Models.Ta1Reference;

namespace ErezeptValidator.Data.Contexts;

/// <summary>
/// Entity Framework Core database context for TA1 reference data.
/// Manages Special Codes (SOK), Factor Codes, Price Codes, and Validation Logs.
/// </summary>
public class Ta1DbContext : DbContext
{
    public Ta1DbContext(DbContextOptions<Ta1DbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Special identifier codes (Sonderkennzeichen) - SOK1 and SOK2
    /// </summary>
    public DbSet<SpecialCode> SpecialCodes { get; set; } = null!;

    /// <summary>
    /// Factor identifier codes (Faktorkennzeichen)
    /// </summary>
    public DbSet<FactorCode> FactorCodes { get; set; } = null!;

    /// <summary>
    /// Price identifier codes (Preiskennzeichen)
    /// </summary>
    public DbSet<PriceCode> PriceCodes { get; set; } = null!;

    /// <summary>
    /// Validation audit logs
    /// </summary>
    public DbSet<ValidationLog> ValidationLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Ensure schema exists
        modelBuilder.HasDefaultSchema("ta1_reference");

        // ========== SpecialCode Configuration ==========
        modelBuilder.Entity<SpecialCode>(entity =>
        {
            entity.ToTable(tb =>
            {
                tb.HasCheckConstraint("chk_code_format", "code ~ '^[0-9]{8}$'");
                tb.HasCheckConstraint("chk_vat_rate", "vat_rate IS NULL OR vat_rate IN (0, 1, 2)");
                tb.HasCheckConstraint("chk_e_rezept", "e_rezept IN (0, 1, 2)");
            });

            entity.HasIndex(e => e.Code)
                .HasDatabaseName("idx_special_codes_code")
                .IsUnique();

            entity.HasIndex(e => e.CodeType)
                .HasDatabaseName("idx_special_codes_type");

            entity.HasIndex(e => e.Category)
                .HasDatabaseName("idx_special_codes_category");

            entity.HasIndex(e => new { e.ValidFromDispensingDate, e.ExpiredDispensingDate })
                .HasDatabaseName("idx_special_codes_validity");
        });

        // ========== FactorCode Configuration ==========
        modelBuilder.Entity<FactorCode>(entity =>
        {
            entity.HasIndex(e => e.Code)
                .HasDatabaseName("idx_factor_codes_code")
                .IsUnique();

            // Seed factor codes (TA3 Section 8.2.25)
            entity.HasData(
                new FactorCode
                {
                    Id = 1,
                    Code = "11",
                    Content = "Anteil in Promille",
                    Description = "Share in promille (parts per thousand)",
                    UseCase = "Processed packages, partial quantities, supplements, compounded preparations",
                    CreatedAt = new DateTime(2026, 1, 24, 0, 0, 0, DateTimeKind.Utc)
                },
                new FactorCode
                {
                    Id = 2,
                    Code = "55",
                    Content = "Einzeldosis in Milligramm",
                    Description = "Single dose in milligrams (opioid substitution - take-home)",
                    UseCase = "Opioid substitution therapy with take-home prescription (§ 5 Abs. 2, 6 BtMVV)",
                    CreatedAt = new DateTime(2026, 1, 24, 0, 0, 0, DateTimeKind.Utc)
                },
                new FactorCode
                {
                    Id = 3,
                    Code = "57",
                    Content = "Einzeldosis in Milligramm",
                    Description = "Single dose in milligrams (opioid substitution - supervised administration)",
                    UseCase = "Opioid substitution therapy with supervised administration (§ 5 Abs. 2, 6 BtMVV)",
                    CreatedAt = new DateTime(2026, 1, 24, 0, 0, 0, DateTimeKind.Utc)
                },
                new FactorCode
                {
                    Id = 4,
                    Code = "99",
                    Content = "Anteil einer Packung in Promille",
                    Description = "Package share in promille (waste/disposal)",
                    UseCase = "Disposal or waste of partial package contents",
                    CreatedAt = new DateTime(2026, 1, 24, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        });

        // ========== PriceCode Configuration ==========
        modelBuilder.Entity<PriceCode>(entity =>
        {
            entity.HasIndex(e => e.Code)
                .HasDatabaseName("idx_price_codes_code")
                .IsUnique();

            // Seed price codes (TA3 Section 8.2.26)
            entity.HasData(
                new PriceCode
                {
                    Id = 1,
                    Code = "11",
                    Content = "Apothekeneinkaufspreis nach AMPreisV",
                    TaxStatus = "excl. VAT",
                    Description = "Pharmacy purchase price per Drug Price Regulation (AMPreisV)",
                    CreatedAt = new DateTime(2026, 1, 24, 0, 0, 0, DateTimeKind.Utc)
                },
                new PriceCode
                {
                    Id = 2,
                    Code = "12",
                    Content = "Zwischen Apotheke und pharmazeutischem Unternehmer vereinbarter Preis",
                    TaxStatus = "excl. VAT",
                    Description = "Price agreed between pharmacy and pharmaceutical manufacturer",
                    CreatedAt = new DateTime(2026, 1, 24, 0, 0, 0, DateTimeKind.Utc)
                },
                new PriceCode
                {
                    Id = 3,
                    Code = "13",
                    Content = "Tatsächlicher Apothekeneinkaufspreis",
                    TaxStatus = "excl. VAT",
                    Description = "Actual pharmacy purchase price",
                    CreatedAt = new DateTime(2026, 1, 24, 0, 0, 0, DateTimeKind.Utc)
                },
                new PriceCode
                {
                    Id = 4,
                    Code = "14",
                    Content = "Abrechnungspreis nach AMPreisV §§ 4, 5",
                    TaxStatus = "excl. VAT",
                    Description = "Billing price per Drug Price Regulation §§ 4, 5 (with pharmacy surcharges)",
                    CreatedAt = new DateTime(2026, 1, 24, 0, 0, 0, DateTimeKind.Utc)
                },
                new PriceCode
                {
                    Id = 5,
                    Code = "15",
                    Content = "Vertraglich vereinbarter Abrechnungspreis",
                    TaxStatus = "excl. VAT",
                    Description = "Contracted billing price (pharmacy-insurance agreement)",
                    CreatedAt = new DateTime(2026, 1, 24, 0, 0, 0, DateTimeKind.Utc)
                },
                new PriceCode
                {
                    Id = 6,
                    Code = "16",
                    Content = "Vertragspreise gemäß § 129a SGB V",
                    TaxStatus = "excl. VAT",
                    Description = "Contract prices per § 129a SGB V (Social Security Code)",
                    CreatedAt = new DateTime(2026, 1, 24, 0, 0, 0, DateTimeKind.Utc)
                },
                new PriceCode
                {
                    Id = 7,
                    Code = "17",
                    Content = "Abrechnungspreis \"Preis 2\" gemäß mg-Preisverzeichnis",
                    TaxStatus = "excl. VAT",
                    Description = "Billing price \"Preis 2\" per mg-price directory",
                    CreatedAt = new DateTime(2026, 1, 24, 0, 0, 0, DateTimeKind.Utc)
                },
                new PriceCode
                {
                    Id = 8,
                    Code = "21",
                    Content = "Rabattvertrag Abrechnungspreis \"Preis 1\" gemäß § 130a Abs. 8c SGB V",
                    TaxStatus = "excl. VAT",
                    Description = "Discount contract billing price \"Preis 1\" per § 130a Abs. 8c SGB V",
                    CreatedAt = new DateTime(2026, 1, 24, 0, 0, 0, DateTimeKind.Utc)
                },
                new PriceCode
                {
                    Id = 9,
                    Code = "90",
                    Content = "Sonderpreis (z.B. 0,00 EUR bei Markierung)",
                    TaxStatus = "excl. VAT",
                    Description = "Special price (e.g., 0.00 EUR for markers or placeholders)",
                    CreatedAt = new DateTime(2026, 1, 24, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        });

        // ========== ValidationLog Configuration ==========
        modelBuilder.Entity<ValidationLog>(entity =>
        {
            entity.ToTable(tb =>
            {
                tb.HasCheckConstraint("chk_result", "validation_result IN ('PASS', 'ERROR', 'WARNING')");
            });

            entity.HasIndex(e => e.PrescriptionId)
                .HasDatabaseName("idx_validation_logs_prescription");

            entity.HasIndex(e => e.ValidationTimestamp)
                .HasDatabaseName("idx_validation_logs_timestamp");

            entity.HasIndex(e => e.ValidationResult)
                .HasDatabaseName("idx_validation_logs_result");
        });
    }
}
