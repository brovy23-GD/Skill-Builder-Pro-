using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SkillBuilderPro.Core.Identity;

namespace SkillBuilderPro.API.Security;

public sealed class HttpCurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpCurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? UserId
    {
        get
        {
            var subject = _httpContextAccessor.HttpContext?.User
                .FindFirstValue(JwtRegisteredClaimNames.Sub);

            return int.TryParse(subject, out var userId) ? userId : null;
        }
    }

    public bool IsAdministrator =>
        _httpContextAccessor.HttpContext?.User
            .IsInRole(ApplicationRoles.Administrator) == true;
}
