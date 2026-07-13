namespace SkillBuilderPro.WinForms.Models
{
    public class User
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string Phone { get; set; }

        public string Sport { get; set; }
        public string TargetArea { get; set; }
        public string ExperienceLevel { get; set; }   // Beginner / Intermediate / Advanced
        public string Role { get; set; }              // Athlete / Coach / Parent

        public bool IsActive { get; set; }

        public string PhotoPath { get; set; }
        public int Age { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public string Team { get; set; }
        public string Bio { get; set; }

        public int JerseyNumber { get; set; }
        public string Goal { get; set; }
    }
}