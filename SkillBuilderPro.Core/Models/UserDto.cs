namespace SkillBuilderPro.Core.Models
{
    /// <summary>What the API returns to clients — no password hash, ever.</summary>
    public class UserDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Sport { get; set; } = string.Empty;
        public string TargetArea { get; set; } = string.Empty;
        public string ExperienceLevel { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int Age { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public string Team { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public int JerseyNumber { get; set; }
        public string Goal { get; set; } = string.Empty;
    }

    /// <summary>Sign-up request body.</summary>
    public class RegisterRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Sport { get; set; } = string.Empty;
        public string TargetArea { get; set; } = string.Empty;
        public int JerseyNumber { get; set; }
        public string Goal { get; set; } = string.Empty;
    }

    /// <summary>Login request body.</summary>
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}