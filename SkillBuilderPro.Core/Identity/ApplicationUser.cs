using Microsoft.AspNetCore.Identity;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.Core.Identity;

public class ApplicationUser : IdentityUser<int>
{
    public UserProfile? Profile { get; set; }
}
