using SkillBuilderPro.Core.Identity;

namespace SkillBuilderPro.API.Authentication;

public interface ITokenService
{
    Task<TokenResult> CreateAccessTokenAsync(ApplicationUser user);
}
