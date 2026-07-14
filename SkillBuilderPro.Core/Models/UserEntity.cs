using System.ComponentModel.DataAnnotations;

namespace SkillBuilderPro.Core.Models
{
    /// <summary>
    /// Database entity for an athlete. PasswordHash never leaves the API —
    /// clients receive UserDto instead.
    /// </summary>
    public class UserEntity
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required, MaxLength(30)]
        public string Sport { get; set; } = string.Empty;

        [MaxLength(50)]
        public string TargetArea { get; set; } = string.Empty;

        [MaxLength(20)]
        public string ExperienceLevel { get; set; } = "Beginner";

        [MaxLength(20)]
        public string Role { get; set; } = "Athlete";

        public bool IsActive { get; set; } = true;
        public string PhotoPath { get; set; } = string.Empty;

        public int Age { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }

        [MaxLength(60)]
        public string Team { get; set; } = string.Empty;

        [MaxLength(300)]
        public string Bio { get; set; } = string.Empty;

        public int JerseyNumber { get; set; }

        [MaxLength(100)]
        public string Goal { get; set; } = string.Empty;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}