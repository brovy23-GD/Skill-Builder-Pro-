using System;

namespace SkillBuilderPro.WinForms.Models
{
    /// <summary>
    /// Represents a single training drill assigned to a specific sport.
    /// The Sport property is critical — it drives all drill filtering in the app.
    /// </summary>
    public class Drill
    {
        /// <summary>Unique identifier for the drill</summary>
        public Guid Id { get; set; }

        /// <summary>Display name of the drill (e.g. "Crossover Dribble")</summary>
        public string Name { get; set; }

        /// <summary>Short explanation of what the drill involves</summary>
        public string Description { get; set; }

        /// <summary>
        /// Step-by-step instructions for performing the drill.
        /// Used in DrillService.cs and the drill detail view.
        /// </summary>
        public string Instructions { get; set; }

        /// <summary>
        /// Optional URL linking to a video demonstration of the drill.
        /// Used in DrillService.cs drill detail view.
        /// </summary>
        public string VideoUrl { get; set; }

        /// <summary>Skill area the drill trains (e.g. "Dribbling", "Hitting", "Defense")</summary>
        public string SkillCategory { get; set; }

        /// <summary>
        /// The sport this drill belongs to.
        /// Must match exactly: "Basketball", "Baseball", "Softball", "Football", "Soccer"
        /// This is what GetDrillsBySport() filters on.
        /// </summary>
        public string Sport { get; set; }

        /// <summary>Difficulty rating: 1 = Easy, 2 = Medium, 3 = Hard</summary>
        public int Difficulty { get; set; }

        /// <summary>Estimated drill duration in minutes</summary>
        public int Duration { get; set; }

        /// <summary>Whether this drill is currently active/available</summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Date the drill was added to the database</summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Returns the drill name when displayed in a CheckedListBox or ComboBox
        /// </summary>
        public override string ToString() => Name;
    }
}
