using Microsoft.EntityFrameworkCore;
using SkillBuilderPro.API.Data;
using SkillBuilderPro.Core.Interfaces;
using SkillBuilderPro.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database — SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SkillBuilderDb")));

// Dependency injection for services
builder.Services.AddScoped<IDrillService, DrillService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();
builder.Services.AddScoped<IProgressService, ProgressService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();   // interactive UI at /swagger
}

app.UseAuthorization();
app.MapControllers();
app.Run();