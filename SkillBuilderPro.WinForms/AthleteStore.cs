using System.Collections.Generic;
using System.Linq;
using SkillBuilderPro.WinForms.Models;

namespace SkillBuilderPro.WinForms
{
    /// <summary>
    /// In-memory store for athletes created during the session.
    /// Swaps out for the Web API later — same shape.
    /// </summary>
    public static class AthleteStore
    {
        private static readonly List<User> _created = new List<User>();

        public static void Add(User athlete) => _created.Add(athlete);

        /// <summary>Dummy roster plus anything created this session.</summary>
        public static List<User> AllUsers()
        {
            var all = DummyUsers.GetAllDummyUsers();
            all.AddRange(_created);
            return all;
        }

        public static List<User> AthletesFor(string guardianEmail) =>
            AllUsers()
                .Where(u => u.Role == "Athlete" && u.GuardianEmail == guardianEmail)
                .ToList();

        public static List<User> AthletesBySport(string sport) =>
            AllUsers()
                .Where(u => u.Role == "Athlete" &&
                            string.Equals(u.Sport, sport, System.StringComparison.OrdinalIgnoreCase))
                .ToList();
    }
}