using ErezeptValidator.Data;
using ErezeptValidator.Data.Contexts;
using ErezeptValidator.Services.CodeLookup;
using ErezeptValidator.Services.DataSeeding;
using ErezeptValidator.Services.Validation;
using ErezeptValidator.Services.Validation.Validators;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add PostgreSQL database context for TA1 reference data
builder.Services.AddDbContext<Ta1DbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Ta1ReferenceDatabase")));
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() {
        Title = "E-Rezept Validator API",
        Version = "v1.0.0-mvp",
        Description = "API for validating E-Rezept prescriptions according to TA1 Version 039 specifications.\n\n" +
                      "**MVP Scope**: Format validations (FMT-001 to FMT-010), General rules (GEN-001 to GEN-008), " +
                      "Basic calculations (CALC-001 to CALC-003). Includes PZN validation via ABDATA and SOK code lookups."
    });

    // Enable XML comments for Swagger documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Add memory cache for PZN lookups (24-hour TTL)
builder.Services.AddMemoryCache();

// Register ABDATA repository
builder.Services.AddScoped<IPznRepository, PznRepository>();

// Register TA1 repository
builder.Services.AddScoped<ITa1Repository, Ta1Repository>();

// Register code lookup service with caching
builder.Services.AddScoped<ICodeLookupService, CodeLookupService>();

// TODO: Update old validators to match new interface
// Register validators (execution order: Format -> General -> Calculation)
// builder.Services.AddScoped<FormatValidator>();
// builder.Services.AddScoped<GeneralRuleValidator>();
// builder.Services.AddScoped<CalculationValidator>();

// Register validation service (orchestrator)
// builder.Services.AddScoped<IValidationService, ValidationService>();

// Register new validation pipeline validators
builder.Services.AddScoped<IValidator, PznFormatValidator>();
builder.Services.AddScoped<IValidator, PznExistsValidator>();
builder.Services.AddScoped<IValidator, BtmDetectionValidator>();
builder.Services.AddScoped<IValidator, FhirFormatValidator>();
builder.Services.AddScoped<IValidator, FhirAbgabedatenValidator>();

// Register validation pipeline
builder.Services.AddScoped<ValidationPipeline>();

// Register database initializer and data loaders
builder.Services.AddSingleton<DatabaseInitializer>();
builder.Services.AddScoped<SokCodeLoader>();

// Add logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "E-Rezept Validator API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
}

app.UseHttpsRedirection();

app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    service = "E-Rezept Validator",
    version = "1.0.0",
    timestamp = DateTime.UtcNow
}))
.WithName("HealthCheck")
.WithOpenApi();

app.Logger.LogInformation("E-Rezept Validator API starting...");
app.Logger.LogInformation("Swagger UI available at: https://localhost:{Port}",
    app.Configuration.GetValue<int>("ASPNETCORE_HTTPS_PORT", 7001));

// Initialize Ta1 reference database
using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    await initializer.InitializeAsync();
}

app.Run();
