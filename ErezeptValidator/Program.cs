using ErezeptValidator.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() {
        Title = "E-Rezept Validator API",
        Version = "v1",
        Description = "API for validating E-Rezept prescriptions according to TA1 specifications"
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

app.Run();
