using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using SkillBuilderPro.WinForms.Models;

namespace SkillBuilderPro.WinForms.Services
{
    public class AuthenticationService
    {
        private readonly List<User> users = new List<User>();

        private static readonly string USERS_FILE =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "users.txt");

        private User? currentUser;

        public AuthenticationService()
        {
            LoadUsers();
        }

        public void LoadUsers()
        {
            users.Clear();

            if (!File.Exists(USERS_FILE))
                return;

            foreach (var line in File.ReadAllLines(USERS_FILE))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var user = ParseUser(line);
                if (user != null)
                    users.Add(user);
            }
        }

        public void SaveUsers()
        {
            var lines = users.Select(u => SerializeUser(u)).ToArray();

            string? dir = Path.GetDirectoryName(USERS_FILE);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            File.WriteAllLines(USERS_FILE, lines);
        }

        public (bool success, string message) SignUp(
            string email,
            string password,
            string fullName,
            string role,
            string sport,
            string targetArea,
            string experienceLevel,
            string phone,
            int jerseyNumber,
            string goal)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                return (false, "Invalid email");

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                return (false, "Password must be at least 6 characters");

            if (users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
                return (false, "Email already exists");

            var user = new User
            {
                Email = email,
                Password = HashPassword(password),
                FullName = fullName,
                Role = role,
                Sport = sport,
                TargetArea = targetArea,
                ExperienceLevel = experienceLevel,
                Phone = phone,
                JerseyNumber = jerseyNumber,
                Goal = goal,
                IsActive = true
            };

            users.Add(user);
            SaveUsers();

            return (true, $"Account created successfully! Welcome, {fullName}!");
        }

        public (bool success, User? user, string message) LogIn(string email, string password)
        {
            var user = users.FirstOrDefault(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (user == null)
                return (false, null, "Email not found");

            if (!VerifyPassword(password, user.Password))
                return (false, null, "Incorrect password");

            if (!user.IsActive)
                return (false, null, "Account is deactivated");

            currentUser = user;
            return (true, user, $"Welcome back, {user.FullName}!");
        }

        public User? GetCurrentUser()
        {
            return currentUser;
        }

        public void LogOut()
        {
            currentUser = null;
        }

        public bool UpdateUserProfile(User updated)
        {
            var user = users.FirstOrDefault(u =>
                u.Email.Equals(updated.Email, StringComparison.OrdinalIgnoreCase));

            if (user == null)
                return false;

            user.FullName = updated.FullName;
            user.Phone = updated.Phone;
            user.Sport = updated.Sport;
            user.TargetArea = updated.TargetArea;
            user.ExperienceLevel = updated.ExperienceLevel;
            user.Role = updated.Role;
            user.PhotoPath = updated.PhotoPath;
            user.Team = updated.Team;
            user.Bio = updated.Bio;
            user.Height = updated.Height;
            user.Weight = updated.Weight;
            user.Age = updated.Age;
            user.JerseyNumber = updated.JerseyNumber;
            user.Goal = updated.Goal;

            if (!string.IsNullOrWhiteSpace(updated.Password))
            {
                user.Password = updated.Password;
            }

            user.IsActive = updated.IsActive;

            SaveUsers();
            return true;
        }

        private string HashPassword(string password)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password).Equals(hash);
        }

        private string SerializeUser(User u)
        {
            return string.Join("|", new[]
            {
                u.Email ?? "",
                u.Password ?? "",
                u.FullName ?? "",
                u.Role ?? "",
                u.Sport ?? "",
                u.TargetArea ?? "",
                u.ExperienceLevel ?? "",
                u.Phone ?? "",
                u.PhotoPath ?? "",
                u.Age.ToString(),
                u.Height.ToString(),
                u.Weight.ToString(),
                u.Team ?? "",
                u.Bio ?? "",
                u.IsActive.ToString(),
                u.JerseyNumber.ToString(),
                u.Goal ?? ""
            });
        }

        private User? ParseUser(string line)
        {
            var p = line.Split('|');
            if (p.Length < 17)
                return null;

            return new User
            {
                Email = p[0],
                Password = p[1],
                FullName = p[2],
                Role = p[3],
                Sport = p[4],
                TargetArea = p[5],
                ExperienceLevel = p[6],
                Phone = p[7],
                PhotoPath = p[8],
                Age = int.TryParse(p[9], out var age) ? age : 0,
                Height = double.TryParse(p[10], out var height) ? height : 0,
                Weight = double.TryParse(p[11], out var weight) ? weight : 0,
                Team = p[12],
                Bio = p[13],
                IsActive = bool.TryParse(p[14], out var active) && active,
                JerseyNumber = int.TryParse(p[15], out var jerseyNumber) ? jerseyNumber : 0,
                Goal = p[16]
            };
        }
    }
}