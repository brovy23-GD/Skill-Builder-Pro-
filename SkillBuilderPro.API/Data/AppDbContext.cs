using Microsoft.EntityFrameworkCore;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<Drill> Drills => Set<Drill>();
    public DbSet<TrainingSchedule> Schedules => Set<TrainingSchedule>();
    public DbSet<ProgressLog> ProgressLogs => Set<ProgressLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Emails unique at the DB level
        modelBuilder.Entity<UserEntity>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<TrainingSchedule>()
            .HasOne(s => s.Drill)
            .WithMany(d => d.Schedules)
            .HasForeignKey(s => s.DrillId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProgressLog>()
            .HasOne(p => p.Drill)
            .WithMany(d => d.ProgressLogs)
            .HasForeignKey(p => p.DrillId)
            .OnDelete(DeleteBehavior.Cascade);

        // Map Difficulty property to Difficulty column
        modelBuilder.Entity<Drill>()
            .Property(d => d.Difficulty)
            .HasColumnName("Difficulty");

        // Seed data — 30 drills (5 per sport × 6 sports)
        modelBuilder.Entity<Drill>().HasData(
            // Basketball (Ids 1-5)
            new Drill { Id = 1, Name = "Ball Handling Drills", Sport = "Basketball", Category = "Dribbling", Description = "5-minute dribbling workout that changes your game.", VideoUrl = "https://www.youtube.com/watch?v=oADaM2L1YLc", Difficulty = 2 },
            new Drill { Id = 2, Name = "Shooting Form Basics", Sport = "Basketball", Category = "Shooting", Description = "Find your perfect shooting form.", VideoUrl = "https://www.youtube.com/watch?v=x7anDE7OEww", Difficulty = 2 },
            new Drill { Id = 3, Name = "Defensive Footwork", Sport = "Basketball", Category = "Defense", Description = "Three defense drills to make your team better.", VideoUrl = "https://www.youtube.com/watch?v=lFY__uSOJIY", Difficulty = 2 },
            new Drill { Id = 4, Name = "Rebounding Techniques", Sport = "Basketball", Category = "Rebounding", Description = "Three best basketball rebounding drills that win games.", VideoUrl = "https://www.youtube.com/watch?v=pFRlEOeWpKY", Difficulty = 1 },
            new Drill { Id = 5, Name = "Passing Accuracy", Sport = "Basketball", Category = "Passing", Description = "Three basketball drills to become better at passing.", VideoUrl = "https://www.youtube.com/watch?v=OUskjh1r4Aw", Difficulty = 1 },

            // Football (Ids 6-10)
            new Drill { Id = 6, Name = "Passing Technique", Sport = "Football", Category = "Passing", Description = "How to throw a football with Tom Brady.", VideoUrl = "https://www.youtube.com/watch?v=lv5p2Xqkxyk", Difficulty = 2 },
            new Drill { Id = 7, Name = "Catching Skills", Sport = "Football", Category = "Catching", Description = "WR drills with Odell Beckham Jr.", VideoUrl = "https://www.youtube.com/watch?v=4n-Js1SwC2c", Difficulty = 2 },
            new Drill { Id = 8, Name = "Route Running", Sport = "Football", Category = "Route Running", Description = "Cooper Kupp's WR drills for creating separation.", VideoUrl = "https://www.youtube.com/watch?v=b8Y-BrxoGQc", Difficulty = 2 },
            new Drill { Id = 9, Name = "Blocking Fundamentals", Sport = "Football", Category = "Blocking", Description = "Proper technique for run and pass blocking.", VideoUrl = "https://www.youtube.com/watch?v=hHyjR__k3XA", Difficulty = 2 },
            new Drill { Id = 10, Name = "Speed and Agility", Sport = "Football", Category = "Conditioning", Description = "Ten speed and agility ladder drills.", VideoUrl = "https://www.youtube.com/watch?v=9ZTRUVLjGzI", Difficulty = 3 },

            // Softball (Ids 11-15)
            new Drill { Id = 11, Name = "Hitting Drills", Sport = "Softball", Category = "Hitting", Description = "Ten best softball hitting drills for kids.", VideoUrl = "https://www.youtube.com/watch?v=g-yDDzQL6eE", Difficulty = 1 },
            new Drill { Id = 12, Name = "Pitching Mechanics", Sport = "Softball", Category = "Pitching", Description = "Basic five steps for a beginner pitcher.", VideoUrl = "https://www.youtube.com/watch?v=mIx9CvpGXsU", Difficulty = 2 },
            new Drill { Id = 13, Name = "Infield Drills", Sport = "Softball", Category = "Fielding", Description = "Three infield drills for youth players.", VideoUrl = "https://www.youtube.com/watch?v=6z0cpY5nGMA", Difficulty = 1 },
            new Drill { Id = 14, Name = "Outfield Skills", Sport = "Softball", Category = "Fielding", Description = "Must-do outfield drills with Gold Glover AJ Andrews.", VideoUrl = "https://www.youtube.com/watch?v=QREFQP72W0U", Difficulty = 2 },
            new Drill { Id = 15, Name = "Catcher Fundamentals", Sport = "Softball", Category = "Catching", Description = "How to improve as a softball catcher.", VideoUrl = "https://www.youtube.com/watch?v=qwdeRteH3es", Difficulty = 3 },

            // Baseball (Ids 16-20)
            new Drill { Id = 16, Name = "Hitting Drills", Sport = "Baseball", Category = "Hitting", Description = "Ten best baseball hitting drills for kids.", VideoUrl = "https://www.youtube.com/watch?v=gOE484Meo_o", Difficulty = 1 },
            new Drill { Id = 17, Name = "Pitching Drills", Sport = "Baseball", Category = "Pitching", Description = "Must-do youth baseball pitching drills.", VideoUrl = "https://www.youtube.com/watch?v=McHb2hXrTrE", Difficulty = 2 },
            new Drill { Id = 18, Name = "Infield Drills", Sport = "Baseball", Category = "Fielding", Description = "The top four infield drills.", VideoUrl = "https://www.youtube.com/watch?v=Uj5lw17XvuI", Difficulty = 1 },
            new Drill { Id = 19, Name = "Outfield Drills", Sport = "Baseball", Category = "Fielding", Description = "Baseball outfield drills you must be doing.", VideoUrl = "https://www.youtube.com/watch?v=WUIM8NqNETg", Difficulty = 2 },
            new Drill { Id = 20, Name = "Catcher Training", Sport = "Baseball", Category = "Catching", Description = "How to become a better baseball catcher.", VideoUrl = "https://www.youtube.com/watch?v=KJZHdoPxvW0", Difficulty = 3 },

            // Hockey (Ids 21-25)
            new Drill { Id = 21, Name = "Edge-Work Skating", Sport = "Hockey", Category = "Skating", Description = "Edge-work drills from level 1 to 100.", VideoUrl = "https://www.youtube.com/watch?v=pp0Y3BDDp4A", Difficulty = 2 },
            new Drill { Id = 22, Name = "Stickhandling Routine", Sport = "Hockey", Category = "Dribbling", Description = "Five-minute daily stickhandling routine.", VideoUrl = "https://www.youtube.com/watch?v=7HluVwbAv3w", Difficulty = 2 },
            new Drill { Id = 23, Name = "Shooting Drills", Sport = "Hockey", Category = "Shooting", Description = "Fifteen hockey shooting drills.", VideoUrl = "https://www.youtube.com/watch?v=RrYFNdTNvkc", Difficulty = 2 },
            new Drill { Id = 24, Name = "Passing Technique", Sport = "Hockey", Category = "Passing", Description = "How to catch and receive passes.", VideoUrl = "https://www.youtube.com/watch?v=BFI7jzMgu6Q", Difficulty = 1 },
            new Drill { Id = 25, Name = "Defensive Positioning", Sport = "Hockey", Category = "Defense", Description = "How to play better defense in hockey.", VideoUrl = "https://www.youtube.com/watch?v=HkNAK40ugkw", Difficulty = 2 },

            // Soccer (Ids 26-30)
            new Drill { Id = 26, Name = "Dribbling Drills", Sport = "Soccer", Category = "Dribbling", Description = "Five essential dribbling drills.", VideoUrl = "https://www.youtube.com/watch?v=jwIHc9rz7yo", Difficulty = 1 },
            new Drill { Id = 27, Name = "Finishing Exercises", Sport = "Soccer", Category = "Shooting", Description = "Ten finishing exercises to become clinical.", VideoUrl = "https://www.youtube.com/watch?v=0u8kPwXXsLA", Difficulty = 2 },
            new Drill { Id = 28, Name = "Passing Drills", Sport = "Soccer", Category = "Passing", Description = "Ten best soccer passing drills.", VideoUrl = "https://www.youtube.com/watch?v=Kb58F3r_TQM", Difficulty = 1 },
            new Drill { Id = 29, Name = "Defensive Fundamentals", Sport = "Soccer", Category = "Defense", Description = "Stop getting beaten in one-on-one situations.", VideoUrl = "https://www.youtube.com/watch?v=aadebgx5nz4", Difficulty = 2 },
            new Drill { Id = 30, Name = "Speed and Agility", Sport = "Soccer", Category = "Conditioning", Description = "Eight exercises to improve speed, agility and power.", VideoUrl = "https://www.youtube.com/watch?v=cCZSTGeSuHM", Difficulty = 3 }
        );
    }
}