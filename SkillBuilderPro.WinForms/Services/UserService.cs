using System;
using System.Collections.Generic;
using SkillBuilderPro.WinForms.Models;

namespace SkillBuilderPro.WinForms.Services
{
    public class UserService
    {
        private AuthenticationService _authService;

        public UserService(AuthenticationService authService)
        {
            _authService = authService;
        }

        public bool UpdateCurrentUserProfile(
            string fullName,
            string email,
            string phone,
            string sport,
            string targetArea,
            string experienceLevel,
            string role,
            string team,
            string bio,
            int age,
            double height,
            double weight,
            string photoPath)
        {
            var user = _authService.GetCurrentUser();
            if (user == null)
                return false;

            user.FullName = fullName;
            user.Email = email;
            user.Phone = phone;
            user.Sport = sport;
            user.TargetArea = targetArea;
            user.ExperienceLevel = experienceLevel;
            user.Role = role;
            user.Team = team;
            user.Bio = bio;
            user.Age = age;
            user.Height = height;
            user.Weight = weight;
            user.PhotoPath = photoPath;

            return _authService.UpdateUserProfile(user);
        }

        public string GetProfileSummary(User user)
        {
            if (user == null)
                return "User not found";

            return $@"
Name: {user.FullName}
Email: {user.Email}
Phone: {user.Phone}
Sport: {user.Sport}
Target Area: {user.TargetArea}
Experience: {user.ExperienceLevel}
Role: {user.Role}
Team: {user.Team}
Bio: {user.Bio}
";
        }

        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public bool IsValidPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return true;

            var cleaned = System.Text.RegularExpressions.Regex.Replace(phone, @"[^\d]", "");
            return cleaned.Length >= 10;
        }
    }
}

