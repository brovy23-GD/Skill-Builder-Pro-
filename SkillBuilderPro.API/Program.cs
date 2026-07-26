using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using SkillBuilderPro.API.Data;
using SkillBuilderPro.API.Middleware;
using SkillBuilderPro.API.Repositories;
using SkillBuilderPro.API.Services;
using SkillBuilderPro.Core.Interfaces;
using SkillBuilderPro.Core.Models;


namespace SkillBuilderPro.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? "Server=(localdb)\\MSSQLLocalDB;Database=SkillBuilderDb;Trusted_Connection=true;";

            var isDevelopment = builder.Environment.IsDevelopment();

            // ====== SERVICES ======

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                    options.JsonSerializerOptions.WriteIndented = isDevelopment;
                });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "SkillBuilderPro API",
                    Version = "v1.0",
                    Description = "Full-stack athletic development platform",
                    Contact = new OpenApiContact
                    {
                        Name = "Bobby Rovy",
                        Email = "brovy23@gmail.com",
                        Url = new Uri("https://github.com/brovy23-GD")
                    }
                });
            });

            // ====== DATABASE ======

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.CommandTimeout(30);
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 3);
                });
            });

            // ====== DEPENDENCY INJECTION ======

            builder.Services.AddScoped<IRepository<Drill>, DrillRepository>();
            builder.Services.AddScoped<IRepository<ProgressLog>, ProgressRepository>();
            builder.Services.AddScoped<IDrillService, DrillService>();
            builder.Services.AddScoped<IProgressService, ProgressService>();
            // Remove: builder.Services.AddScoped<IAuthService, AuthService>();

            // ====== CORS ======

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // ====== LOGGING ======

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            if (isDevelopment)
            {
                builder.Logging.SetMinimumLevel(LogLevel.Debug);
            }

            // ====== BUILD ======

            var app = builder.Build();

            // ====== MIDDLEWARE ======

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            if (isDevelopment)
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SkillBuilderPro API v1.0");
                    c.RoutePrefix = string.Empty;
                });
            }
            else
            {
                app.UseHttpsRedirection();
            }

            app.Use(async (context, next) =>
            {
                var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                try
                {
                    await next();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Request failed");
                    throw;
                }
            });

            app.UseCors("AllowAll");
            app.UseAuthorization();
            app.MapControllers();

            app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));
            app.MapGet("/api/info", () => Results.Ok(new { name = "SkillBuilderPro API", version = "1.0" }));

            // ====== DATABASE INITIALIZATION ======

            using (var scope = app.Services.CreateScope())
            {
                try
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                    logger.LogInformation("Applying migrations...");
                    await dbContext.Database.MigrateAsync();
                    logger.LogInformation("✅ Database ready");
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Database initialization failed");
                    throw;
                }
            }

            app.Logger.LogInformation("🎯 SkillBuilderPro API started");

            try
            {
                await app.RunAsync();
            }
            catch (Exception ex)
            {
                app.Logger.LogCritical(ex, "Application crashed");
                throw;
            }
        }
    }
}