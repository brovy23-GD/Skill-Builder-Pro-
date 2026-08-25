using SkillBuilderPro.Core.Security;

namespace SkillBuilderPro.Tests.Security;

public sealed class AthleteAccessScopeTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WhenActorIdIsNotPositive_Throws(int actorUserId) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new AthleteAccessScope(actorUserId, false, []));

    [Fact]
    public void Constructor_WhenAthleteIdsRepeat_DeduplicatesScope()
    {
        var scope = new AthleteAccessScope(10, false, [20, 20, 21]);
        Assert.Equal(2, scope.AthleteUserIds.Count);
    }

    [Fact]
    public void CanAccessAthlete_ForScopedAthlete_ReturnsTrue()
    {
        var scope = new AthleteAccessScope(10, false, [20]);
        Assert.True(scope.CanAccessAthlete(20));
    }

    [Fact]
    public void CanAccessAthlete_ForUnscopedAthlete_ReturnsFalse()
    {
        var scope = new AthleteAccessScope(10, false, [20]);
        Assert.False(scope.CanAccessAthlete(21));
    }

    [Fact]
    public void CanAccessAthlete_ForAdministrator_ReturnsTrueWithoutExplicitScope()
    {
        var scope = new AthleteAccessScope(10, true, []);
        Assert.True(scope.CanAccessAthlete(999));
    }
}
