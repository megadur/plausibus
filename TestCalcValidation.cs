// Quick test for CALC-004 to CALC-007 validation rules
// Compile and run: dotnet script TestCalcValidation.cs

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Model;
using ErezeptValidator.Services.Validation;
using ErezeptValidator.Services.Validation.Validators;
using ErezeptValidator.Data;
using ErezeptValidator.Data.Contexts;
using ErezeptValidator.Services.CodeLookup;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("==========================================");
        Console.WriteLine("CALC-004 to CALC-007 Validation Test");
        Console.WriteLine("==========================================\n");

        // Setup DI
        var services = new ServiceCollection();

        // Add logging
        services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

        // Add memory cache
        services.AddMemoryCache();

        // Add database contexts
        services.AddDbContext<Ta1DbContext>(options =>
            options.UseNpgsql("Host=localhost;Port=5432;Database=erezept_validator;Username=erezept_user;Password=Segelboot.1"));

        // Add repositories
        services.AddScoped<IPznRepository, PznRepository>();
        services.AddScoped<ITa1Repository, Ta1Repository>();
        services.AddScoped<ICodeLookupService, CodeLookupService>();

        // Add validators
        services.AddScoped<IValidator, PznFormatValidator>();
        services.AddScoped<IValidator, PznExistsValidator>();
        services.AddScoped<IValidator, BtmDetectionValidator>();
        services.AddScoped<IValidator, FhirFormatValidator>();
        services.AddScoped<IValidator, FhirAbgabedatenValidator>();
        services.AddScoped<IValidator, CalculationValidator>();

        // Add validation pipeline
        services.AddScoped<ValidationPipeline>();

        var provider = services.BuildServiceProvider();

        // Test files
        var testFiles = new[]
        {
            new { Name = "Standard PZN", File = "docs/eRezept-Beispiele/PZN-Verordnung_Nr_1/PZN_Nr1_eAbgabedaten.xml" },
            new { Name = "Artificial Insemination", File = "docs/eRezept-Beispiele/PZN-Verordnung_Künstliche_Befruchtung/PZN-Verordnung_Künstliche_Befruchtung_V1/PZN_KB_V1_eAbgabedaten.xml" },
            new { Name = "Compounding", File = "docs/eRezept-Beispiele/Rezeptur-Verordnung_Nr_1/Rez_Nr1_eAbgabedaten.xml" }
        };

        foreach (var test in testFiles)
        {
            await RunTest(provider, test.Name, test.File);
            Console.WriteLine();
        }

        Console.WriteLine("==========================================");
        Console.WriteLine("All tests complete");
        Console.WriteLine("==========================================");
    }

    static async Task RunTest(ServiceProvider provider, string testName, string filePath)
    {
        Console.WriteLine($"📋 Test: {testName}");
        Console.WriteLine($"   File: {filePath}");

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"   ❌ File not found\n");
            return;
        }

        try
        {
            // Parse FHIR bundle
            var xml = File.ReadAllText(filePath);
            var parser = new FhirXmlParser(new ParserSettings
            {
                AcceptUnknownMembers = true,
                AllowUnrecognizedEnums = true
            });

            var bundle = parser.Parse<Bundle>(xml);
            Console.WriteLine($"   ✅ Bundle parsed: {bundle.Entry?.Count ?? 0} entries");

            // Run validation
            using var scope = provider.CreateScope();
            var pipeline = scope.ServiceProvider.GetRequiredService<ValidationPipeline>();
            var results = await pipeline.ValidateAsync(bundle);

            // Display results
            var totalErrors = results.Sum(r => r.ErrorCount);
            var totalWarnings = results.Sum(r => r.WarningCount);

            Console.WriteLine($"\n   📊 Results: {results.Count} validators, {totalErrors} errors, {totalWarnings} warnings");

            foreach (var result in results)
            {
                if (result.ErrorCount > 0 || result.WarningCount > 0)
                {
                    var icon = result.ErrorCount > 0 ? "❌" : "⚠️";
                    Console.WriteLine($"\n   {icon} {result.ValidatorName}:");

                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"      🔴 [{error.Code}] {error.Message}");
                    }

                    foreach (var warning in result.Warnings)
                    {
                        Console.WriteLine($"      🟡 [{warning.Code}] {warning.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"   ✅ {result.ValidatorName}: All checks passed");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   ❌ Error: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"      {ex.InnerException.Message}");
            }
        }
    }
}
