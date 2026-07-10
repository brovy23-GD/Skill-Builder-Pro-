using System;

namespace SkillBuilderPro.WinForms.Models
{
    /// <summary>
    /// ProgressLog.cs - Represents a single progress log entry for tracking athlete improvements
    ///
    /// A progress log is created after each training session to record
    /// how the athlete performed, how they felt, and what they accomplished.
    ///
    /// Properties:
    /// - Id, UserId, SessionId    - identify who this log belongs to
    /// - LogDate                  - when the session took place
    /// - MetricType, Value, Unit  - what was measured and the result
    /// - SessionDuration          - how long the session lasted
    /// - DrillsCompleted          - which drills were done
    /// - DifficultyLevel          - how hard the session was (1-3)
    /// - Notes, SatisfactionScore, EnergyLevel - athlete feedback
    /// - PreviousMetricValue      - used to calculate improvement
    ///
    /// Methods:
    /// - GetImprovementPercentage() - calculates how much the metric improved
    /// - IsImprovement()            - returns true if metric went up
    /// - GetDifficultyLabel()       - converts 1/2/3 to Easy/Medium/Hard
    ///
    /// TIME COMPLEXITY: O(1) for all operations
    /// SPACE COMPLEXITY: O(1) - fixed size properties
    /// </summary>
    public class ProgressLog
    {
        // ═══════════════════════════════════════════════════
        // PROPERTIES
        // ═══════════════════════════════════════════════════

        // Unique identifier for this log entry
        public Guid Id { get; set; }

        // The athlete this log belongs to
        public Guid UserId { get; set; }

        // The training session this log was recorded for
        public Guid SessionId { get; set; }

        // The date and time this log was recorded
        public DateTime LogDate { get; set; }

        // ───────────────────────────────────────────────────
        // PERFORMANCE METRICS
        // ───────────────────────────────────────────────────

        // What type of metric was measured
        public string MetricType { get; set; } // e.g., "Accuracy", "Speed", "Endurance", "Strength"

        // The actual measured value
        public double MetricValue { get; set; } // e.g., 85.5

        // The unit the metric is measured in
        public string MetricUnit { get; set; } // e.g., "%", "seconds", "reps", "lbs"

        // The previous metric value - used to calculate improvement over time
        public double PreviousMetricValue { get; set; } // e.g., 80.0 (what it was before)

        // ───────────────────────────────────────────────────
        // SESSION DETAILS
        // ───────────────────────────────────────────────────

        // How long the session lasted in minutes
        public int SessionDuration { get; set; }

        // The names of drills completed stored as comma separated text
        public string DrillsCompleted { get; set; } // e.g., "Drop Step, Agility Ladder, Reaction Training"

        // How hard the session was: 1=Easy, 2=Medium, 3=Hard
        public int DifficultyLevel { get; set; }

        // ───────────────────────────────────────────────────
        // ATHLETE FEEDBACK
        // ───────────────────────────────────────────────────

        // Any notes the athlete or coach wrote about the session
        public string Notes { get; set; }

        // How satisfied the athlete was with their performance: 1-5
        public int SatisfactionScore { get; set; }

        // How much energy the athlete had during the session: 1-5
        public int EnergyLevel { get; set; }

        // ───────────────────────────────────────────────────
        // METADATA
        // ───────────────────────────────────────────────────

        // When this log entry was created in the system
        public DateTime CreatedAt { get; set; }

        // When this log entry was last modified
        public DateTime LastUpdated { get; set; }

        // ═══════════════════════════════════════════════════
        // CONSTRUCTOR
        // ═══════════════════════════════════════════════════

        /// <summary>
        /// Default constructor - sets up a new progress log with default values
        /// TIME COMPLEXITY: O(1)
        /// SPACE COMPLEXITY: O(1)
        /// </summary>
        public ProgressLog()
        {
            Id = Guid.NewGuid();           // generate a unique ID for this log
            LogDate = DateTime.Now;        // default log date to right now
            CreatedAt = DateTime.Now;      // record when it was created
            LastUpdated = DateTime.Now;    // set last updated to now
            SatisfactionScore = 3;         // default to middle rating (3 out of 5)
            EnergyLevel = 3;               // default to middle energy level (3 out of 5)
            DifficultyLevel = 1;           // default to easy
            PreviousMetricValue = 0;       // no previous value by default
        }

        // ═══════════════════════════════════════════════════
        // METHODS
        // ═══════════════════════════════════════════════════

        /// <summary>
        /// Calculates how much the metric improved compared to the previous value
        /// Returns 0 if there is no previous value to compare against
        ///
        /// Example: previous = 80, current = 85 → improvement = 6.25%
        ///
        /// TIME COMPLEXITY: O(1)
        /// SPACE COMPLEXITY: O(1)
        /// </summary>
        public double GetImprovementPercentage()
        {
            // Cannot calculate improvement if there is no previous value
            if (PreviousMetricValue == 0)
                return 0;

            // Formula: ((current - previous) / previous) * 100
            return ((MetricValue - PreviousMetricValue) / PreviousMetricValue) * 100;
        }

        /// <summary>
        /// Returns true if the current metric value is higher than the previous one
        /// Returns false if performance stayed the same or got worse
        ///
        /// TIME COMPLEXITY: O(1)
        /// SPACE COMPLEXITY: O(1)
        /// </summary>
        public bool IsImprovement()
        {
            // Compare current value to previous value
            return MetricValue > PreviousMetricValue;
        }

        /// <summary>
        /// Converts the difficulty level number to a readable label
        /// 1 = Easy, 2 = Medium, 3 = Hard
        ///
        /// TIME COMPLEXITY: O(1)
        /// SPACE COMPLEXITY: O(1)
        /// </summary>
        public string GetDifficultyLabel()
        {
            // Return a readable label based on the difficulty number
            if (DifficultyLevel == 1)
                return "Easy";
            else if (DifficultyLevel == 2)
                return "Medium";
            else if (DifficultyLevel == 3)
                return "Hard";
            else
                return "Unknown"; // catch any invalid values
        }

        /// <summary>
        /// Converts satisfaction score number to a readable label
        /// 1 = Poor, 2 = Fair, 3 = Good, 4 = Great, 5 = Excellent
        ///
        /// TIME COMPLEXITY: O(1)
        /// SPACE COMPLEXITY: O(1)
        /// </summary>
        public string GetSatisfactionLabel()
        {
            // Return a readable label based on the satisfaction score
            if (SatisfactionScore == 1)
                return "Poor";
            else if (SatisfactionScore == 2)
                return "Fair";
            else if (SatisfactionScore == 3)
                return "Good";
            else if (SatisfactionScore == 4)
                return "Great";
            else if (SatisfactionScore == 5)
                return "Excellent";
            else
                return "Unknown"; // catch any invalid values
        }

        // ═══════════════════════════════════════════════════
        // TOSTRING
        // ═══════════════════════════════════════════════════

        /// <summary>
        /// Returns a readable summary of this progress log entry
        /// Useful for displaying in lists or debug output
        ///
        /// TIME COMPLEXITY: O(1)
        /// SPACE COMPLEXITY: O(1)
        /// </summary>
        public override string ToString()
        {
            // Build a readable summary with date, metric, value, and session length
            return $"{LogDate:MM/dd/yyyy} - {MetricType}: {MetricValue}{MetricUnit} " +
                   $"(Session: {SessionDuration} min | Difficulty: {GetDifficultyLabel()} | " +
                   $"Satisfaction: {GetSatisfactionLabel()})";
        }
    }
}
