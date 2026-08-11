using Microsoft.EntityFrameworkCore;
using SkillBuilderPro.Core.Models;
using System.Collections.Generic;

namespace SkillBuilderPro.Core.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Drill> Drills { get; set; } = null!;
    public DbSet<ProgressLog> ProgressLogs { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<TrainingSchedule> Schedules { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

    public static List<Drill> GetHardcodedDrills()
    {
        return new List<Drill>();
    }
}