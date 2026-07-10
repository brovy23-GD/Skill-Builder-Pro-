using System;
using System.Collections.Generic;
using SkillBuilderPro.WinForms.Models;

namespace SkillBuilderPro.WinForms.Models
{
    /// <summary>
    /// TrainingSession.cs - Represents a single training session containing multiple drills
    ///
    /// A training session groups drills together under one scheduled event.
    /// Example: "Monday Morning Workout" with 3 drills totaling 45 minutes
    ///
    /// Properties:
    /// - Id, UserId       - identify who owns this session
    /// - Name, Notes      - describe what the session is
    /// - Date, Duration   - when it happens and how long it runs
    /// - Drills           - the list of drills included in this session
    /// - IsCompleted      - tracks whether the session was finished
    ///
    /// Methods:
    /// - GetTotalDuration()  - adds up all drill durations
    /// - GetDrillCount()     - returns how many drills are in the session
    /// - AddDrill()          - adds a drill to the session
    /// - RemoveDrill()       - removes a drill from the session
    /// - CompleteSession()   - marks the session as done
    ///
    /// TIME COMPLEXITY:
    /// - GetTotalDuration: O(n) - loops through all drills
    /// - GetDrillCount:    O(1) - uses .Count property directly
    /// - AddDrill:         O(1) - adds to end of list
    /// - RemoveDrill:      O(n) - searches list for the drill to remove
    ///
    /// SPACE COMPLEXITY: O(n) - stores n drills in memory
    /// </summary>
    public class TrainingSession
    {
        // ═══════════════════════════════════════════════════
        // PROPERTIES
        // ═══════════════════════════════════════════════════

        // Unique identifier for this session
        public Guid Id { get; set; }

        // The athlete this session belongs to
        public Guid UserId { get; set; }

        // Name of this training session
        public string Name { get; set; } // e.g., "Monday Morning Workout"

        // Optional notes about this session (what to focus on, reminders, etc.)
        public string Notes { get; set; }

        // The date and time this session is scheduled for
        public DateTime Date { get; set; }

        // The list of drills included in this session
        public List<Drill> Drills { get; set; }

        // Whether this session has been completed by the athlete
        public bool IsCompleted { get; set; }

        // The date the session was actually completed (null if not done yet)
        public DateTime? CompletedDate { get; set; }

        // ───────────────────────────────────────────────────
        // METADATA
        // ───────────────────────────────────────────────────

        // When this session was first created in the system
        public DateTime DateCreated { get; set; }

        // When this session was last modified
        public DateTime LastUpdated { get; set; }

        // ═══════════════════════════════════════════════════
        // CONSTRUCTOR
        // ═══════════════════════════════════════════════════

        /// <summary>
        /// Default constructor - sets up a new training session with default values
        /// TIME COMPLEXITY: O(1)
        /// SPACE COMPLEXITY: O(1)
        /// </summary>
        public TrainingSession()
        {
            Id = Guid.NewGuid();               // generate a unique ID for this session
            Drills = new List<Drill>();         // start with an empty drill list
            IsCompleted = false;               // new sessions start as not completed
            DateCreated = DateTime.Now;        // record when it was created
            LastUpdated = DateTime.Now;        // set last updated to now
        }

        // ═══════════════════════════════════════════════════
        // METHODS
        // ═══════════════════════════════════════════════════

        /// <summary>
        /// Calculates the total duration of the session by adding all drill durations
        /// Returns 0 if there are no drills
        ///
        /// TIME COMPLEXITY: O(n) - must loop through every drill
        /// SPACE COMPLEXITY: O(1) - only stores a running total
        /// </summary>
        public int GetTotalDuration()
        {
            int total = 0; // running total of minutes

            // Add each drill's duration to the total
            foreach (var drill in Drills)
            {
                total += drill.Duration; // add this drill's minutes to the total
            }

            return total; // return the final total in minutes
        }

        /// <summary>
        /// Returns how many drills are in this session
        ///
        /// TIME COMPLEXITY: O(1) - List.Count is instant
        /// SPACE COMPLEXITY: O(1)
        /// </summary>
        public int GetDrillCount()
        {
            return Drills.Count; // return the number of drills in the list
        }

        /// <summary>
        /// Adds a drill to this training session
        /// Throws an error if a null drill is passed in
        ///
        /// TIME COMPLEXITY: O(1) - adding to end of list is instant
        /// SPACE COMPLEXITY: O(1)
        /// </summary>
        public void AddDrill(Drill drill)
        {
            // Make sure a valid drill was passed in
            if (drill == null)
                throw new ArgumentNullException(nameof(drill), "Drill cannot be null");

            Drills.Add(drill);           // add the drill to the list
            LastUpdated = DateTime.Now;  // record that the session was updated
        }

        /// <summary>
        /// Removes a drill from this training session by its ID
        /// Returns true if the drill was found and removed
        /// Returns false if the drill was not found
        ///
        /// TIME COMPLEXITY: O(n) - must search through the list to find the drill
        /// SPACE COMPLEXITY: O(1)
        /// </summary>
        public bool RemoveDrill(Guid drillId)
        {
            // Loop through every drill to find the one with matching ID
            foreach (var drill in Drills)
            {
                if (drill.Id == drillId)
                {
                    Drills.Remove(drill);        // remove the drill from the list
                    LastUpdated = DateTime.Now;  // record that the session was updated
                    return true;                 // drill was found and removed
                }
            }

            return false; // drill was not found in this session
        }

        /// <summary>
        /// Marks this session as completed and records the completion date
        ///
        /// TIME COMPLEXITY: O(1)
        /// SPACE COMPLEXITY: O(1)
        /// </summary>
        public void CompleteSession()
        {
            IsCompleted = true;                // mark session as done
            CompletedDate = DateTime.Now;      // record when it was completed
            LastUpdated = DateTime.Now;        // record the update time
        }

        /// <summary>
        /// Calculates how many days until this session is scheduled
        /// Returns 0 if the session is already completed or in the past
        ///
        /// TIME COMPLEXITY: O(1)
        /// SPACE COMPLEXITY: O(1)
        /// </summary>
        public int DaysUntilSession()
        {
            // If already completed, no days remaining
            if (IsCompleted)
                return 0;

            // Calculate days from today to the session date
            int daysLeft = (int)(Date - DateTime.Now).TotalDays;

            // Return 0 if the date has already passed
            return daysLeft > 0 ? daysLeft : 0;
        }

        // ═══════════════════════════════════════════════════
        // TOSTRING
        // ═══════════════════════════════════════════════════

        /// <summary>
        /// Returns a readable summary of this training session
        /// Useful for displaying in lists or debug output
        ///
        /// TIME COMPLEXITY: O(n) - calls GetTotalDuration which loops drills
        /// SPACE COMPLEXITY: O(1)
        /// </summary>
        public override string ToString()
        {
            // Pick a status label based on whether the session is done
            string status = IsCompleted ? "Completed" : "Scheduled";

            // Return a formatted summary string
            return $"{Name} ({Date:MM/dd/yyyy}) - {GetDrillCount()} drills - {GetTotalDuration()} min - {status}";
        }
    }
}
