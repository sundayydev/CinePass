using CinePass.Core.Configurations;
using CinePass.Core.Repositories;
using CinePass.Core.Services;
using CinePass.Domain.IRepository;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// ===============================
// 1. INFRASTRUCTURE (Aspire)
// ===============================

// Kích hoạt Service Defaults (HealthChecks, OpenTelemetry…)
builder.AddServiceDefaults();

// A. Database (Postgres)
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("PostgresConnection");

    Console.WriteLine("==== DB CONNECTION STRING ====");
    Console.WriteLine(connectionString);
    Console.WriteLine("================================");

    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
    });
});

// B. Cache (Redis)
builder.AddRedisClient("cache");

// C. Storage (MinIO)
builder.AddMinioClient("storage");

// ===============================
// 2. DEPENDENCY INJECTION
// ===============================
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ICinemaRepository, CinemaRepository>();
builder.Services.AddScoped<CinemaService>();
builder.Services.AddScoped<IScreenRepository, ScreenRepository>();
builder.Services.AddScoped<ScreenService>();
builder.Services.AddScoped<ISeatRepository, SeatRepository>();
builder.Services.AddScoped<SeatService>();
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<MovieService>();
builder.Services.AddControllers();
builder.Services.AddProblemDetails();

// ===============================
// 3. API DOCUMENTATION
// ===============================

// Scalar API (UI tại /scalar/v1)
builder.Services.AddEndpointsApiExplorer();

// Swagger/Swashbuckle
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CinePass API",
        Version = "v1",
        Description = "Swagger JSON endpoint: /swagger/v1/swagger.json"
    });
});


// ===============================
// 4. CORS
// ===============================
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ===============================
// 5. AUTOMATIC MIGRATION
// ===============================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Lỗi khi thực hiện Database Migration.");
    }
}

// ===============================
// 6. PIPELINE
// ===============================
app.UseExceptionHandler();
app.UseCors();
// app.UseDeveloperExceptionPage();
if (app.Environment.IsDevelopment())
{
    // Scalar API Documentation UI
    app.MapScalarApiReference(); // truy cập /scalar/v1

    // Swagger UI và JSON
    app.UseSwagger(); // JSON tại /swagger/v1/swagger.json
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CinePass API V1");
        c.RoutePrefix = "swagger"; // UI tại /swagger
    });
}

// Map HealthCheck và Controllers
app.MapDefaultEndpoints(); // HealthCheck của Aspire
app.MapControllers();

app.Run();
