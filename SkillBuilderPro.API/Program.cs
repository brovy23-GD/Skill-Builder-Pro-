using Microsoft.EntityFrameworkCore;
using SkillBuilderPro.API.Data;
using SkillBuilderPro.Core.Interfaces;
using SkillBuilderPro.API.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SkillBuilderDb")));

// Skip migration validation — prevents freezing on startup
using (var scope = builder.Services.BuildServiceProvider().CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        // Don't call Migrate() — just verify connection exists
        db.Database.CanConnect();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database warning (non-fatal): {ex.Message}");
    }
}


// Dependency injection for services
builder.Services.AddScoped<IDrillService, DrillService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IProgressService, ProgressService>();

builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:5001");
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();   // interactive UI at /swagger
}

app.UseAuthorization();
app.MapControllers();
app.Run();