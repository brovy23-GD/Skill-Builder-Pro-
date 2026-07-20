using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SkillBuilderPro.WinForms.Models;          // WinForms Drill
using CoreDrill = SkillBuilderPro.Core.Models.Drill;

namespace SkillBuilderPro.WinForms.Services
{
    /// <summary>
    /// Single entry point for drills. Prefers the Web API; falls back to the
    /// offline DrillDatabase when the API is unreachable. Callers get WinForms
    /// Drills either way and never know which source answered.
    /// </summary>
    public static class DrillProvider
    {
        private static readonly DrillApiService _api = new DrillApiService();

        /// <summary>Which source answered the last request: "API" or "Offline".</summary>
        public static string LastSource { get; private set; } = "—";

        public static async Task<List<Drill>> GetBySportAsync(string sport)
        {
            try
            {
                List<CoreDrill> apiDrills = await _api.GetAllAsync(sport);
                if (apiDrills != null && apiDrills.Count > 0)
                {
                    LastSource = "API";
                    return apiDrills.Select(MapToWinForms).ToList();
                }
            }
            catch
            {
                // API down, wrong port, no network — fall through to offline
            }

            LastSource = "Offline";
            return DrillDatabase.GetDrillsBySport(sport);
        }

        /// <summary>
        /// The one place the two Drill models meet. Core uses Category/DifficultyLevel
        /// and an int Id; WinForms uses SkillCategory/Difficulty and a Guid Id.
        /// </summary>
        private static Drill MapToWinForms(CoreDrill c) => new Drill
        {
            Id = Guid.NewGuid(),
            Name = c.Name,
            Description = c.Description,
            SkillCategory = c.SubCategory,
            Sport = c.Sport,
            Difficulty = c.DifficultyLevel,
            Duration = c.Duration,
            DateCreated = DateTime.Now
        };
    }
}