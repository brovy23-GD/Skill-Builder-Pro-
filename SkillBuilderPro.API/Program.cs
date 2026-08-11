using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SkillBuilderPro.API.Authentication;
using SkillBuilderPro.API.Data;
using SkillBuilderPro.API.Middleware;
using SkillBuilderPro.API.Services;
using SkillBuilderPro.Core.Data;
using SkillBuilderPro.Core.Identity;
using SkillBuilderPro.Core.Interfaces;
using SkillBuilderPro.Core.Models;
using SkillBuilderPro.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SkillBuilderPro.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var connectionString =
            builder.Configuration.GetConnectionString("SkillBuilderDb")
            ?? throw new InvalidOperationException(
                "Connection string 'SkillBuilderDb' was not found.");

        var isDevelopment = builder.Environment.IsDevelopment();

        // ==========================================
        // 1. SERVICES & SERIALIZATION
        // ==========================================

        builder.Services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.WriteIndented = isDevelopment;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(
                "v1",
                new Microsoft.OpenApi.OpenApiInfo
                {
                    Title = "SkillBuilderPro API",
                    Version = "v1.0",
                    Description = "Full-stack athletic development platform",
                    Contact = new Microsoft.OpenApi.OpenApiContact
                    {
                        Name = "Bobby Rovy",
                        Email = "brovy23@gmail.com",
                        Url = new Uri("https://github.com/brovy23-GD")
                    }
                });

            c.AddSecurityDefinition(
                "Bearer",
                new Microsoft.OpenApi.OpenApiSecurityScheme
                {
                    Type = Microsoft.OpenApi.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Enter the JWT access token."
                });

            c.AddSecurityRequirement(document =>
                new Microsoft.OpenApi.OpenApiSecurityRequirement
                {
                    [new Microsoft.OpenApi.OpenApiSecuritySchemeReference(
                        "Bearer",
                        document,
                        null)] = []
                });
        });

        // ==========================================
        // 2. DATABASE INFRASTRUCTURE
        // ==========================================

        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(
                connectionString,
                sqlOptions =>
                {
                    sqlOptions.CommandTimeout(30);

                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3);
                });
        });

        builder.Services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole<int>>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        builder.Services
            .AddOptions<JwtOptions>()
            .Bind(builder.Configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var jwtOptions = builder.Configuration
            .GetSection(JwtOptions.SectionName)
            .Get<JwtOptions>() ?? new JwtOptions();

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1),
                    NameClaimType = "email",
                    RoleClaimType = "role"
                };
            });

        builder.Services.AddScoped<ITokenService, JwtTokenService>();

        // ==========================================
        // 3. CORE DEPENDENCY INJECTION REGISTRATION
        // ==========================================

        builder.Services.AddScoped<IDrillService, DrillApiService>();

        builder.Services.AddScoped<IRepository<Drill>, DrillRepository>();

        builder.Services.AddScoped<IDrillRepository, DrillRepository>();

        // Seeder remains registered so it can be used later
        // for an explicit, validated import operation.
        // It is NOT automatically executed at application startup.
        builder.Services.AddScoped<DrillExcelSeeder>();

        builder.Services.AddScoped<IScheduleService, ScheduleService>();

        builder.Services.AddScoped<
            IRepository<ProgressLog>,
            ProgressRepository>();

        builder.Services.AddScoped<IProgressService, ProgressService>();

        // ==========================================
        // 4. CROSS-ORIGIN RESOURCE SHARING (CORS)
        // ==========================================

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(
                "AllowAll",
                policy =>
                {
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
        });

        // ==========================================
        // 5. DIAGNOSTIC LOGGING PROVIDERS
        // ==========================================

        builder.Logging.ClearProviders();

        builder.Logging.AddConsole();

        builder.Logging.AddDebug();

        if (isDevelopment)
        {
            builder.Logging.SetMinimumLevel(LogLevel.Debug);
        }

        // ==========================================
        // 6. PIPELINE ORCHESTRATION BUILD
        // ==========================================

        var app = builder.Build();

        // ==========================================
        // 7. MIDDLEWARE PIPELINE
        // ==========================================

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        if (isDevelopment)
        {
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(
                    "/swagger/v1/swagger.json",
                    "SkillBuilderPro API v1.0");

                c.RoutePrefix = string.Empty;
            });
        }
        else
        {
            app.UseHttpsRedirection();
        }

        // ==========================================
        // REQUEST DIAGNOSTIC TRACKER
        // ==========================================

        app.Use(async (context, next) =>
        {
            var logger =
                context.RequestServices
                    .GetRequiredService<ILogger<Program>>();

            var stopwatch =
                System.Diagnostics.Stopwatch.StartNew();

            try
            {
                await next();

                stopwatch.Stop();

                logger.LogDebug(
                    "{Method} {Path} completed with {StatusCode} in {ElapsedMilliseconds} ms.",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                logger.LogError(
                    ex,
                    "Request failed while processing {Method} {Path}.",
                    context.Request.Method,
                    context.Request.Path);

                throw;
            }
        });

        app.UseCors("AllowAll");

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        // ==========================================
        // DIAGNOSTIC ENDPOINTS
        // ==========================================

        app.MapGet(
            "/health",
            () => Results.Ok(
                new
                {
                    status = "healthy"
                }));

        app.MapGet(
            "/api/info",
            () => Results.Ok(
                new
                {
                    name = "SkillBuilderPro API",
                    version = "1.0"
                }));

        // ==========================================
        // 8. DATABASE STARTUP VERIFICATION
        // ==========================================
        //
        // IMPORTANT:
        //
        // Automatic drill seeding is intentionally disabled.
        //
        // The previous startup pipeline:
        //   - deleted all rows from Drills
        //   - loaded the legacy 900-drill JSON
        //   - loaded hardcoded drills
        //   - generated dummy drills
        //
        // That legacy JSON contains contaminated cross-sport data
        // and must NOT be automatically imported.
        //
        // Future drill imports must be explicitly initiated and
        // validated before DrillExcelSeeder.SeedAsync() is used.
        // ==========================================

        _ = Task.Run(async () =>
        {
            using var scope = app.Services.CreateScope();

            var logger =
                scope.ServiceProvider
                    .GetRequiredService<ILogger<Program>>();

            try
            {
                var dbContext =
                    scope.ServiceProvider
                        .GetRequiredService<AppDbContext>();

                logger.LogInformation(
                    "Verifying SkillBuilderPro database infrastructure...");

                var pendingMigrations =
                    (await dbContext.Database.GetPendingMigrationsAsync())
                    .ToArray();

                if (pendingMigrations.Length > 0)
                {
                    logger.LogWarning(
                        "Database initialization is paused because {PendingMigrationCount} migration(s) are pending: {PendingMigrations}. Apply reviewed migrations explicitly before starting Identity role initialization.",
                        pendingMigrations.Length,
                        string.Join(", ", pendingMigrations));

                    return;
                }

                await IdentityRoleInitializer.InitializeAsync(
                    scope.ServiceProvider);

                // ==================================================
                // AUTOMATIC DRILL SEEDING IS DISABLED
                // ==================================================
                //
                // DO NOT uncomment this until drills_seed.json has
                // been replaced with the validated production dataset.
                //
                // var drillSeeder =
                //     scope.ServiceProvider
                //         .GetRequiredService<DrillExcelSeeder>();
                //
                // await drillSeeder.SeedAsync();
                //
                // ==================================================

                logger.LogInformation(
                    "SkillBuilderPro database infrastructure verified and ready.");
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Database background initialization sequence faulted.");
            }
        });

        app.Logger.LogInformation(
            "SkillBuilderPro API started.");

        try
        {
            await app.RunAsync();
        }
        catch (Exception ex)
        {
            app.Logger.LogCritical(
                ex,
                "Application host crashed unexpectedly.");

            throw;
        }
    }
}
