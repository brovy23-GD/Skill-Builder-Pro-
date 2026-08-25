using SkillBuilderPro.Core.Identity;
using SkillBuilderPro.Core.Interfaces;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.Tests.Domain;

public sealed class DomainContractTests
{
    [Fact]
    public void ApplicationRoles_All_ContainsCanonicalRolesWithoutAdminAlias()
    {
        Assert.Equal(["Athlete", "Parent", "Coach", "Administrator"], ApplicationRoles.All);
        Assert.DoesNotContain("Admin", ApplicationRoles.All);
    }

    [Fact]
    public void DrillAssignmentStatuses_All_IsOrdinalAndComplete()
    {
        Assert.Equal(4, DrillAssignmentStatuses.All.Count);
        Assert.Contains(DrillAssignmentStatuses.Scheduled, DrillAssignmentStatuses.All);
        Assert.DoesNotContain("scheduled", DrillAssignmentStatuses.All);
    }

    [Fact]
    public void DrillAssignmentRecipientStatuses_All_IsComplete()
    {
        Assert.Equal(5, DrillAssignmentRecipientStatuses.All.Count);
        Assert.Contains(DrillAssignmentRecipientStatuses.Completed, DrillAssignmentRecipientStatuses.All);
    }

    [Fact]
    public void AssignmentOperationResult_Success_PreservesValue()
    {
        var result = AssignmentOperationResult<string>.Success("assignment");
        Assert.Equal(AssignmentOperationStatus.Success, result.Status);
        Assert.Equal("assignment", result.Value);
        Assert.Null(result.Error);
    }

    [Fact]
    public void AssignmentOperationResult_Created_PreservesValue()
    {
        var result = AssignmentOperationResult<int>.Created(42);
        Assert.Equal(AssignmentOperationStatus.Created, result.Status);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void AssignmentOperationResult_Validation_PreservesErrorWithoutValue()
    {
        var result = AssignmentOperationResult<string>.Validation("Drill is required.");
        Assert.Equal(AssignmentOperationStatus.ValidationError, result.Status);
        Assert.Null(result.Value);
        Assert.Equal("Drill is required.", result.Error);
    }

    [Fact]
    public void AssignmentOperationResult_NotFound_HasNoPayload()
    {
        var result = AssignmentOperationResult<string>.NotFound();
        Assert.Equal(AssignmentOperationStatus.NotFound, result.Status);
        Assert.Null(result.Value);
        Assert.Null(result.Error);
    }

    [Fact]
    public void AssignmentOperationResult_Conflict_PreservesErrorWithoutValue()
    {
        var result = AssignmentOperationResult<string>.Conflict("Already completed.");
        Assert.Equal(AssignmentOperationStatus.Conflict, result.Status);
        Assert.Null(result.Value);
        Assert.Equal("Already completed.", result.Error);
    }
}
