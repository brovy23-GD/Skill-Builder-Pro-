using Microsoft.EntityFrameworkCore;
using SkillBuilderPro.Core.Data;
using SkillBuilderPro.Core.Models;
using SkillBuilderPro.Core.Repositories;

namespace SkillBuilderPro.Tests.Persistence;

public sealed class RepositoryTests
{
    [Fact]
    public void DrillRepository_WhenContextIsNull_Throws() =>
        Assert.Throws<ArgumentNullException>(() => new DrillRepository(null!));

    [Fact]
    public async Task DrillRepository_AddSaveAndGetById_RoundTripsDrill()
    {
        await using var context = CreateContext();
        var repository = new DrillRepository(context);
        await repository.AddAsync(new Drill { Id = 1, Name = "Footwork", Sport = "Basketball", Category = "Defense" });
        await repository.SaveAsync();
        Assert.Equal("Footwork", (await repository.GetByIdAsync(1))?.Name);
    }

    [Fact]
    public async Task DrillRepository_GetAll_ReturnsUntrackedEntities()
    {
        await using var context = CreateContext();
        context.Drills.AddRange(new Drill { Id = 1, Name = "A" }, new Drill { Id = 2, Name = "B" });
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        Assert.Equal(2, (await new DrillRepository(context).GetAllAsync()).Count());
        Assert.Empty(context.ChangeTracker.Entries<Drill>());
    }

    [Fact]
    public async Task DrillRepository_FindAsync_WhenPredicateMatches_ReturnsDrill()
    {
        await using var context = CreateContext();
        context.Drills.Add(new Drill { Id = 1, Name = "Footwork", Sport = "Basketball" });
        await context.SaveChangesAsync();
        Assert.Equal("Footwork", (await new DrillRepository(context).FindAsync(item => item.Sport == "Basketball"))?.Name);
    }

    [Fact]
    public async Task DrillRepository_GetDrillRangeAsync_ReturnsInclusiveRange()
    {
        await using var context = CreateContext();
        context.Drills.AddRange(Enumerable.Range(1, 5).Select(id => new Drill { Id = id, Name = $"Drill {id}" }));
        await context.SaveChangesAsync();
        var ids = (await new DrillRepository(context).GetDrillRangeAsync(2, 4)).Select(item => item.Id).Order().ToArray();
        Assert.Equal([2, 3, 4], ids);
    }

    [Fact]
    public async Task DrillRepository_GetDrillRangeAsync_WhenStartExceedsEnd_ReturnsEmpty()
    {
        await using var context = CreateContext();
        Assert.Empty(await new DrillRepository(context).GetDrillRangeAsync(5, 2));
    }

    [Fact]
    public async Task DrillRepository_UpdateAndDelete_PersistStateTransitions()
    {
        await using var context = CreateContext();
        var repository = new DrillRepository(context);
        var drill = new Drill { Id = 1, Name = "Original" };
        await repository.AddAsync(drill);
        await repository.SaveAsync();
        drill.Name = "Updated";
        await repository.UpdateAsync(drill);
        await repository.SaveAsync();
        Assert.Equal("Updated", (await repository.GetByIdAsync(1))?.Name);
        await repository.DeleteAsync(drill);
        await repository.SaveAsync();
        Assert.Null(await repository.GetByIdAsync(1));
    }

    [Fact]
    public async Task UserRepository_CRUDAndFind_PersistExpectedUserState()
    {
        await using var context = CreateContext();
        var repository = new UserRepository(context);
        var user = ValidUser(1, "athlete@example.com");
        await repository.AddAsync(user);
        await repository.SaveAsync();
        Assert.Equal(user.Email, (await repository.GetByIdAsync(1))?.Email);
        Assert.Single(await repository.GetAllAsync());
        Assert.Equal(1, (await repository.FindAsync(item => item.Email == user.Email))?.Id);
        user.FullName = "Updated Athlete";
        await repository.UpdateAsync(user);
        await repository.SaveAsync();
        Assert.Equal("Updated Athlete", (await repository.GetByIdAsync(1))?.FullName);
        await repository.DeleteAsync(user);
        await repository.SaveAsync();
        Assert.Empty(await repository.GetAllAsync());
    }

    [Fact]
    public async Task ProgressRepository_CRUDAndFind_PersistExpectedProgressState()
    {
        await using var context = CreateContext();
        context.Drills.Add(new Drill { Id = 1, Name = "Footwork" });
        await context.SaveChangesAsync();
        var repository = new ProgressRepository(context);
        var progress = new ProgressLog { Id = 1, DrillId = 1, Rating = 3, Notes = "Started" };
        await repository.AddAsync(progress);
        await repository.SaveAsync();
        Assert.Equal(3, (await repository.GetByIdAsync(1))?.Rating);
        Assert.Single(await repository.GetAllAsync());
        Assert.Equal(1, (await repository.FindAsync(item => item.Notes == "Started"))?.Id);
        progress.Rating = 5;
        await repository.UpdateAsync(progress);
        await repository.SaveAsync();
        Assert.Equal(5, (await repository.GetByIdAsync(1))?.Rating);
        await repository.DeleteAsync(progress);
        await repository.SaveAsync();
        Assert.Empty(await repository.GetAllAsync());
    }

    private static AppDbContext CreateContext() => new(
        new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options);

    private static User ValidUser(int id, string email) => new()
    {
        Id = id, FullName = "Athlete", Email = email, PasswordHash = "hash", Sport = "Basketball"
    };
}
