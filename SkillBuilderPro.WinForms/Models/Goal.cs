using System;

namespace SkillBuilderPro.WinForms.Models
{
    /// <summary>
    /// Goal.cs - Represents a training goal set by a user
    ///
    /// Properties:
    /// - Id, UserId - identify who owns this goal
    /// - Title, Description, TargetMetric - describe what the goal is
    /// - StartDate, TargetDate, CompletedDate - track the timeline
    /// - ProgressPercentage - tracks how far along the goal is (0-100)
    /// - Status - Active, Completed, or Abandoned
    /// - Priority - 1=High, 2=Medium, 3=Low
    ///
    /// Methods:
    /// - DaysRemaining() - how many days until the target date
    /// - CompleteGoal()  - marks the goal as done
    /// - UpdateProgress() - updates the progress percentage
    /// </summary>
    public class Goal
    {
        // ═══════════════════════════════════════════════════
        // PROPERTIES
        // ═══════════════════════════════════════════════════

        // Unique identifier for this goal
        public Guid Id { get; set; }

        // The athlete this goal belongs to
        public Guid UserId { get; set; }

        // What the goal is called
        public string Title { get; set; } // e.g., "Improve Shooting Accuracy"

        // A longer description of the goal
        public string Description { get; set; }

        // The specific metric being targeted
        public string TargetMetric { get; set; } // e.g., "Increase accuracy to 85%"

        // ───────────────────────────────────────────────────
        // TIMELINE
        // ───────────────────────────────────────────────────

        // When the athlete started working on this goal
        public DateTime StartDate { get; set; }

        // The deadline for completing this goal
        public DateTime TargetDate { get; set; }

        // The date the goal was actually completed (null if not done yet)
        public DateTime? CompletedDate { get; set; }

        // ───────────────────────────────────────────────────
        // PROGRESS
        // ───────────────────────────────────────────────────

        // How far along the goal is - value between 0 and 100
        public int ProgressPercentage { get; set; }

        // Current status of the goal
        public GoalStatus Status { get; set; } // Active, Completed, Abandoned

        // How important this goal is: 1=High, 2=Medium, 3=Low
        public int Priority { get; set; }

        // ───────────────────────────────────────────────────
        // METADATA
        // ───────────────────────────────────────────────────

        // When this goal was first created
        public DateTime DateCreated { get; set; }

        // When this goal was last changed
        public DateTime LastUpdated { get; set; }

        // ═══════════════════════════════════════════════════
        // CONSTRUCTOR
        // ═══════════════════════════════════════════════════

        /// <summary>
        /// Default constructor - sets up a new goal with default values
        /// TIME COMPLEXITY: O(1)
        /// </summary>
        public Goal()
        {
            Id = Guid.NewGuid();              // generate a unique ID
            DateCreated = DateTime.Now;        // record when it was created
            LastUpdated = DateTime.Now;        // set last updated to now
            Status = GoalStatus.Active;        // new goals start as active
            Priority = 2;                      // default to medium priority
            ProgressPercentage = 0;            // starts at 0% progress
        }

        // ═══════════════════════════════════════════════════
        // METHODS
        // ═══════════════════════════════════════════════════

        /// <summary>
        /// Calculates how many days are left until the target date
        /// Returns 0 if the goal is already completed
        /// TIME COMPLEXITY: O(1)
        /// </summary>
        public int DaysRemaining()
        {
            // If the goal is done, no days remaining
            if (Status == GoalStatus.Completed || CompletedDate.HasValue)
                return 0;

            // Subtract today from the target date and return as whole days
            return (int)(TargetDate - DateTime.Now).TotalDays;
        }

        /// <summary>
        /// Marks this goal as completed and records the completion date
        /// TIME COMPLEXITY: O(1)
        /// </summary>
        public void CompleteGoal()
        {
            Status = GoalStatus.Completed;     // update status
            CompletedDate = DateTime.Now;      // record when it was completed
            ProgressPercentage = 100;          // progress is now 100%
            LastUpdated = DateTime.Now;        // record the update time
        }

        /// <summary>
        /// Updates the progress percentage for this goal
        /// Automatically completes the goal if progress reaches 100
        /// TIME COMPLEXITY: O(1)
        /// </summary>
        public void UpdateProgress(int percentComplete)
        {
            // Make sure the value is between 0 and 100
            if (percentComplete < 0 || percentComplete > 100)
                throw new ArgumentException("Progress must be between 0 and 100");

            ProgressPercentage = percentComplete; // set the new progress
            LastUpdated = DateTime.Now;           // record the update time

            // If progress hit 100, auto-complete the goal
            if (percentComplete == 100)
            {
                CompleteGoal();
            }
        }

        /// <summary>
        /// Returns a short readable summary of this goal
        /// TIME COMPLEXITY: O(1)
        /// </summary>
        public override string ToString()
        {
            // Pick a label based on the current status
            string statusLabel = Status == GoalStatus.Active ? "Active" :
                                 Status == GoalStatus.Completed ? "Completed" : "Abandoned";

            return $"{Title} - {ProgressPercentage}% - {statusLabel}";
        }
    }

    // ═══════════════════════════════════════════════════
    // GOAL STATUS ENUM
    // ═══════════════════════════════════════════════════

    /// <summary>
    /// The three possible states a goal can be in
    /// </summary>
    public enum GoalStatus
    {
        Active,     // goal is in progress
        Completed,  // goal was achieved
        Abandoned   // goal was given up on
    }
}
