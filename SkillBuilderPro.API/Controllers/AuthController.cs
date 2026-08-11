using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillBuilderPro.API.Authentication;
using SkillBuilderPro.API.Contracts.Authentication;
using SkillBuilderPro.Core.Data;
using SkillBuilderPro.Core.Identity;
using SkillBuilderPro.Core.Models;
using ApiLoginRequest = SkillBuilderPro.API.Contracts.Authentication.LoginRequest;
using ApiRegisterRequest = SkillBuilderPro.API.Contracts.Authentication.RegisterRequest;

namespace SkillBuilderPro.API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private const string InvalidCredentialsMessage = "Invalid email or password.";

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly AppDbContext _dbContext;
    private readonly ITokenService _tokenService;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole<int>> roleManager,
        AppDbContext dbContext,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _dbContext = dbContext;
        _tokenService = tokenService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<AuthenticationResponse>> Register(
        ApiRegisterRequest request)
    {
        var role = GetPublicRegistrationRole(request.Role);
        if (role is null)
        {
            ModelState.AddModelError(
                nameof(request.Role),
                "Public registration is available only for Athlete or Parent accounts.");
            return ValidationProblem(ModelState);
        }

        if (!await _roleManager.RoleExistsAsync(role))
        {
            return Problem(
                title: "Account registration is temporarily unavailable.",
                statusCode: StatusCodes.Status503ServiceUnavailable);
        }

        var email = request.Email.Trim();
        var user = new ApplicationUser
        {
            Email = email,
            UserName = email
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            AddIdentityErrors(createResult);
            return ValidationProblem(ModelState);
        }

        var roleResult = await _userManager.AddToRoleAsync(user, role);
        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);
            AddIdentityErrors(roleResult);
            return ValidationProblem(ModelState);
        }

        var profile = new UserProfile
        {
            UserId = user.Id,
            FullName = request.FullName.Trim(),
            IsActive = true
        };

        try
        {
            _dbContext.UserProfiles.Add(profile);
            await _dbContext.SaveChangesAsync();
        }
        catch
        {
            await _userManager.DeleteAsync(user);
            throw;
        }

        return Ok(await CreateAuthenticationResponseAsync(user, profile));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthenticationResponse>> Login(ApiLoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email.Trim());
        if (user is null)
        {
            return Unauthorized(new { Message = InvalidCredentialsMessage });
        }

        var signInResult = await _signInManager.CheckPasswordSignInAsync(
            user,
            request.Password,
            lockoutOnFailure: true);

        if (!signInResult.Succeeded)
        {
            return Unauthorized(new { Message = InvalidCredentialsMessage });
        }

        var profile = await _dbContext.UserProfiles
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.UserId == user.Id);

        if (profile is null || !profile.IsActive)
        {
            return Unauthorized(new { Message = InvalidCredentialsMessage });
        }

        return Ok(await CreateAuthenticationResponseAsync(user, profile));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<CurrentUserResponse>> Me()
    {
        var subject = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (!int.TryParse(subject, out var userId))
        {
            return Unauthorized();
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return Unauthorized();
        }

        var profile = await _dbContext.UserProfiles
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.UserId == user.Id);

        if (profile is null || !profile.IsActive)
        {
            return Unauthorized();
        }

        return Ok(await CreateCurrentUserResponseAsync(user, profile));
    }

    private async Task<AuthenticationResponse> CreateAuthenticationResponseAsync(
        ApplicationUser user,
        UserProfile profile)
    {
        var token = await _tokenService.CreateAccessTokenAsync(user);
        var currentUser = await CreateCurrentUserResponseAsync(user, profile);

        return new AuthenticationResponse(
            token.AccessToken,
            token.ExpiresAtUtc,
            currentUser);
    }

    private async Task<CurrentUserResponse> CreateCurrentUserResponseAsync(
        ApplicationUser user,
        UserProfile profile)
    {
        var roles = await _userManager.GetRolesAsync(user);

        return new CurrentUserResponse(
            user.Id,
            user.Email ?? string.Empty,
            profile.FullName,
            roles.ToArray(),
            profile.Phone,
            profile.Sport,
            profile.TargetArea,
            profile.ExperienceLevel,
            profile.IsActive);
    }

    private void AddIdentityErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("Identity", error.Description);
        }
    }

    private static string? GetPublicRegistrationRole(string requestedRole)
    {
        if (string.Equals(
                requestedRole,
                ApplicationRoles.Athlete,
                StringComparison.OrdinalIgnoreCase))
        {
            return ApplicationRoles.Athlete;
        }

        if (string.Equals(
                requestedRole,
                ApplicationRoles.Parent,
                StringComparison.OrdinalIgnoreCase))
        {
            return ApplicationRoles.Parent;
        }

        return null;
    }
}
