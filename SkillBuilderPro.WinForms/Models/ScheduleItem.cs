namespace SkillBuilderPro.WinForms.Models
{
    /// <summary>
    /// Represents a single row in the training schedule grid.
    /// Each ScheduleItem is one drill assigned to a specific day.
    /// These are the items looped over in MainForm.cs line 724:
    ///     foreach (var item in currentSchedule.Items)
    /// </summary>
    public class ScheduleItem
    {
        /// <summary>Day of the week this drill is scheduled (e.g. "Monday")</summary>
        public string Day { get; set; }

        /// <summary>Name of the drill — maps to scheduleGrid column "Drill Name"</summary>
        public string DrillName { get; set; }

        /// <summary>Skill category — maps to scheduleGrid column "Category"</summary>
        public string Category { get; set; }

        /// <summary>Duration in minutes — displayed as "X min" in scheduleGrid</summary>
        public int Duration { get; set; }

        /// <summary>Drill description used as notes — maps to scheduleGrid column "Notes"</summary>
        public string Notes { get; set; }
    }
}
