using Microsoft.EntityFrameworkCore;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.API.Data;

public class AppDbContext : DbContext
{


    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Drill> Drills => Set<Drill>();
    public DbSet<ProgressLog> ProgressLogs => Set<ProgressLog>();
    public DbSet<TrainingSchedule> Schedules => Set<TrainingSchedule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Sport).IsRequired().HasMaxLength(30);
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Drill configuration
        modelBuilder.Entity<Drill>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Sport).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.VideoUrl).HasMaxLength(500);
            entity.Property(e => e.DifficultyLevel).IsRequired();
            entity.HasIndex(e => e.Sport);
        });

        // ProgressLog configuration
        modelBuilder.Entity<ProgressLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DrillId).IsRequired();
            entity.Property(e => e.LogDate).IsRequired();
            entity.Property(e => e.Rating).IsRequired();
            entity.Property(e => e.Notes).HasMaxLength(300);
            entity.HasIndex(e => new { e.DrillId, e.LogDate });
        });

        // TrainingSchedule configuration
        modelBuilder.Entity<TrainingSchedule>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DrillId).IsRequired();
        });

        // Seed 60 drills
        modelBuilder.Entity<Drill>().HasData(
            // Basketball (Ids 1-10)
            new Drill { Id = 1, Name = "Ball Handling Drills", Sport = "Basketball", Category = "Dribbling", Description = "5-minute dribbling workout", DifficultyLevel = 2 },
            new Drill { Id = 2, Name = "Shooting Form Basics", Sport = "Basketball", Category = "Shooting", Description = "Find your perfect shooting form", DifficultyLevel = 2 },
            new Drill { Id = 3, Name = "Defensive Footwork", Sport = "Basketball", Category = "Defense", Description = "Three defense drills", DifficultyLevel = 2 },
            new Drill { Id = 4, Name = "Rebounding Techniques", Sport = "Basketball", Category = "Rebounding", Description = "Basketball rebounding drills", DifficultyLevel = 1 },
            new Drill { Id = 5, Name = "Passing Accuracy", Sport = "Basketball", Category = "Passing", Description = "Drills to become better at passing", DifficultyLevel = 1 },
            new Drill { Id = 6, Name = "Fast Break Drills", Sport = "Basketball", Category = "Offense", Description = "Transition offense drills", DifficultyLevel = 3 },
            new Drill { Id = 7, Name = "Post Moves", Sport = "Basketball", Category = "Post", Description = "Low post fundamentals", DifficultyLevel = 2 },
            new Drill { Id = 8, Name = "Three Point Shooting", Sport = "Basketball", Category = "Shooting", Description = "Long range shooting drills", DifficultyLevel = 3 },
            new Drill { Id = 9, Name = "Crossover Dribble", Sport = "Basketball", Category = "Dribbling", Description = "Advanced ball handling", DifficultyLevel = 3 },
            new Drill { Id = 10, Name = "Bounce Pass Drills", Sport = "Basketball", Category = "Passing", Description = "Passing technique drills", DifficultyLevel = 1 },

            // Football (Ids 11-20)
            new Drill { Id = 11, Name = "Passing Technique", Sport = "Football", Category = "Passing", Description = "Throwing mechanics", DifficultyLevel = 2 },
            new Drill { Id = 12, Name = "Catching Skills", Sport = "Football", Category = "Receiving", Description = "WR drills", DifficultyLevel = 2 },
            new Drill { Id = 13, Name = "Route Running", Sport = "Football", Category = "Receiving", Description = "Creating separation", DifficultyLevel = 2 },
            new Drill { Id = 14, Name = "Blocking Fundamentals", Sport = "Football", Category = "Blocking", Description = "Run and pass blocking", DifficultyLevel = 2 },
            new Drill { Id = 15, Name = "Speed and Agility", Sport = "Football", Category = "Conditioning", Description = "Ladder drills", DifficultyLevel = 3 },
            new Drill { Id = 16, Name = "Footwork Drills", Sport = "Football", Category = "Footwork", Description = "QB footwork", DifficultyLevel = 2 },
            new Drill { Id = 17, Name = "Edge Rusher Technique", Sport = "Football", Category = "Defense", Description = "Pass rush drills", DifficultyLevel = 3 },
            new Drill { Id = 18, Name = "Coverage Drills", Sport = "Football", Category = "Defense", Description = "Defensive back drills", DifficultyLevel = 2 },
            new Drill { Id = 19, Name = "Tackling Drills", Sport = "Football", Category = "Defense", Description = "Proper tackling form", DifficultyLevel = 2 },
            new Drill { Id = 20, Name = "Cone Drills", Sport = "Football", Category = "Agility", Description = "Agility cone drills", DifficultyLevel = 1 },

            // Softball (Ids 21-30)
            new Drill { Id = 21, Name = "Hitting Drills", Sport = "Softball", Category = "Hitting", Description = "Batting practice", DifficultyLevel = 1 },
            new Drill { Id = 22, Name = "Pitching Mechanics", Sport = "Softball", Category = "Pitching", Description = "Windmill pitch", DifficultyLevel = 2 },
            new Drill { Id = 23, Name = "Infield Drills", Sport = "Softball", Category = "Defense", Description = "Ground ball drills", DifficultyLevel = 1 },
            new Drill { Id = 24, Name = "Outfield Skills", Sport = "Softball", Category = "Defense", Description = "Fly ball drills", DifficultyLevel = 2 },
            new Drill { Id = 25, Name = "Catcher Fundamentals", Sport = "Softball", Category = "Catching", Description = "Catching drills", DifficultyLevel = 3 },
            new Drill { Id = 26, Name = "Bunting Techniques", Sport = "Softball", Category = "Hitting", Description = "Bunting drills", DifficultyLevel = 2 },
            new Drill { Id = 27, Name = "Base Running", Sport = "Softball", Category = "Running", Description = "Running drills", DifficultyLevel = 1 },
            new Drill { Id = 28, Name = "Drop Ball Drill", Sport = "Softball", Category = "Pitching", Description = "Pitcher drills", DifficultyLevel = 2 },
            new Drill { Id = 29, Name = "Relay Throws", Sport = "Softball", Category = "Throwing", Description = "Throwing accuracy", DifficultyLevel = 2 },
            new Drill { Id = 30, Name = "Sliding Drills", Sport = "Softball", Category = "Running", Description = "Base sliding", DifficultyLevel = 1 },

            // Baseball (Ids 31-40)
            new Drill { Id = 31, Name = "Hitting Drills", Sport = "Baseball", Category = "Hitting", Description = "Batting practice", DifficultyLevel = 1 },
            new Drill { Id = 32, Name = "Pitching Drills", Sport = "Baseball", Category = "Pitching", Description = "Youth pitching drills", DifficultyLevel = 2 },
            new Drill { Id = 33, Name = "Infield Drills", Sport = "Baseball", Category = "Defense", Description = "Ground ball drills", DifficultyLevel = 1 },
            new Drill { Id = 34, Name = "Outfield Drills", Sport = "Baseball", Category = "Defense", Description = "Fly ball drills", DifficultyLevel = 2 },
            new Drill { Id = 35, Name = "Catcher Training", Sport = "Baseball", Category = "Catching", Description = "Catcher drills", DifficultyLevel = 3 },
            new Drill { Id = 36, Name = "Base Running", Sport = "Baseball", Category = "Running", Description = "Running drills", DifficultyLevel = 1 },
            new Drill { Id = 37, Name = "Curveball Practice", Sport = "Baseball", Category = "Pitching", Description = "Breaking ball drills", DifficultyLevel = 3 },
            new Drill { Id = 38, Name = "Cut-off Throws", Sport = "Baseball", Category = "Throwing", Description = "Relay throw drills", DifficultyLevel = 2 },
            new Drill { Id = 39, Name = "Double Play Drills", Sport = "Baseball", Category = "Defense", Description = "Infield coordination", DifficultyLevel = 2 },
            new Drill { Id = 40, Name = "Slide Drills", Sport = "Baseball", Category = "Running", Description = "Base sliding", DifficultyLevel = 1 },

            // Hockey (Ids 41-50)
            new Drill { Id = 41, Name = "Edge-Work Skating", Sport = "Hockey", Category = "Skating", Description = "Skating drills", DifficultyLevel = 2 },
            new Drill { Id = 42, Name = "Stickhandling Routine", Sport = "Hockey", Category = "Stickhandling", Description = "Dribbling drills", DifficultyLevel = 2 },
            new Drill { Id = 43, Name = "Shooting Drills", Sport = "Hockey", Category = "Shooting", Description = "Shot accuracy", DifficultyLevel = 2 },
            new Drill { Id = 44, Name = "Passing Technique", Sport = "Hockey", Category = "Passing", Description = "Passing drills", DifficultyLevel = 1 },
            new Drill { Id = 45, Name = "Defensive Positioning", Sport = "Hockey", Category = "Defense", Description = "Defense drills", DifficultyLevel = 2 },
            new Drill { Id = 46, Name = "Crossover Drill", Sport = "Hockey", Category = "Skating", Description = "Skating technique", DifficultyLevel = 2 },
            new Drill { Id = 47, Name = "One-Timer Practice", Sport = "Hockey", Category = "Shooting", Description = "Shooting on the move", DifficultyLevel = 3 },
            new Drill { Id = 48, Name = "Checking Drills", Sport = "Hockey", Category = "Defense", Description = "Body checking drills", DifficultyLevel = 2 },
            new Drill { Id = 49, Name = "Goalie Drills", Sport = "Hockey", Category = "Goaltending", Description = "Goaltending drills", DifficultyLevel = 3 },
            new Drill { Id = 50, Name = "Conditioning Skate", Sport = "Hockey", Category = "Conditioning", Description = "Endurance skating", DifficultyLevel = 2 },

            // Soccer (Ids 51-60)
            new Drill { Id = 51, Name = "Dribbling Drills", Sport = "Soccer", Category = "Dribbling", Description = "Ball control drills", DifficultyLevel = 1 },
            new Drill { Id = 52, Name = "Finishing Exercises", Sport = "Soccer", Category = "Shooting", Description = "Shooting drills", DifficultyLevel = 2 },
            new Drill { Id = 53, Name = "Passing Drills", Sport = "Soccer", Category = "Passing", Description = "Passing technique", DifficultyLevel = 1 },
            new Drill { Id = 54, Name = "Defensive Fundamentals", Sport = "Soccer", Category = "Defense", Description = "Defense drills", DifficultyLevel = 2 },
            new Drill { Id = 55, Name = "Speed and Agility", Sport = "Soccer", Category = "Conditioning", Description = "Speed drills", DifficultyLevel = 3 },
            new Drill { Id = 56, Name = "Crossing Drills", Sport = "Soccer", Category = "Crossing", Description = "Wing play drills", DifficultyLevel = 2 },
            new Drill { Id = 57, Name = "Free Kick Practice", Sport = "Soccer", Category = "Set Pieces", Description = "Free kick drills", DifficultyLevel = 3 },
            new Drill { Id = 58, Name = "Heading Drills", Sport = "Soccer", Category = "Heading", Description = "Heading technique", DifficultyLevel = 2 },
            new Drill { Id = 59, Name = "First Touch Drills", Sport = "Soccer", Category = "Control", Description = "Ball control drills", DifficultyLevel = 1 },
            new Drill { Id = 60, Name = "Tactical Drills", Sport = "Soccer", Category = "Tactics", Description = "Formation drills", DifficultyLevel = 2 }
        );
    }
}
