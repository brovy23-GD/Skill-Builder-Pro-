using System;

namespace SkillBuilderPro.WinForms.Models
{
    /// <summary>
    /// CalendarEvent.cs - Represents a scheduled event on the athlete's calendar
    ///
    /// A calendar event can be a training session, goal milestone,
    /// reminder, competition, or rest day.
    ///
    /// Properties:
    /// - Id, UserId               - identify who owns this event
    /// - Title, Description       - describe what the event is
    /// - EventType                - Training, GoalMilestone, Reminder, Competition, Rest
    /// - EventDate, EventTime     - when the event happens
    /// - DurationMinutes          - how long the event lasts
    /// - HasReminder              - whether a reminder should be sent
    /// - ReminderMinutesBefore    - how early to send the reminder
    /// - Status                   - Scheduled, InProgress, Completed, Cancelled
    /// - RelatedGoalId            - links to a goal if applicable
    /// - RelatedSessionId         - links to a training session if applicable
    ///
    /// Methods:
    /// - ShouldSendReminder()  - checks if it is time to send the reminder
    /// - CompleteEvent()       - marks event as done
    /// - GetEventDateTime()    - combines date and time into one DateTime
    /// - IsToday()             - checks if event is today
    /// - IsUpcoming()          - checks if event is within the next 7 days
    /// - CancelEvent()         - cancels the event
    /// - GetEventTypeLabel()   - returns readable event type name
    ///
    /// TIME COMPLEXITY: O(1) for all operations
    /// SPACE COMPLEXITY: O(1) - fixed size properties
    /// </summary>
    public class CalendarEvent
    {
        // ═══════════════════════════════════════════════════
        // PROPERTIES
        // ═══════════════════════════════════════════════════

        // Unique identifier for this event
        public Guid Id { get; set; }

        // The athlete this event belongs to
        public Guid UserId { get; set; }

        // Short name for the event
        public string Title { get; set; } // e.g., "Monday Morning Workout"

        // More detailed description of what happens at this event
        public string Description { get; set; }

        // ───────────────────────────────────────────────────
        // EVENT DETAILS
        // ───────────────────────────────────────────────────

        // What kind of event this is
        public EventType EventType { get; set; } // Training, GoalMilestone, Reminder, Competition, Rest

        // The date this event is scheduled for
        public DateTime EventDate { get; set; }

        // The time this event starts
        public DateTime EventTime { get; set; }

        // How long this event lasts in minutes
        public int DurationMinutes { get; set; }

        // ───────────────────────────────────────────────────
        // REMINDER SETTINGS
        // ───────────────────────────────────────────────────

        // Whether this event has a reminder turned on
        public bool HasReminder { get; set; }

        // How many minutes before the event to send the reminder
        public int ReminderMinutesBefore { get; set; } // e.g., 15, 30, 60

        // The date and time the reminder was actually sent (null if not sent yet)
        public DateTime? ReminderSentTime { get; set; }

        // ───────────────────────────────────────────────────
        // EVENT STATUS
        // ───────────────────────────────────────────────────

        // Current status of this event
        public EventStatus Status { get; set; } // Scheduled, InProgress, Completed, Cancelled

        // Links to a goal if this event is related to one (null if not linked)
        public Guid? RelatedGoalId { get; set; }

        // Links to a training session if this event is related to one (null if not linked)
        public Guid? RelatedSessionId { get; set; }

        // ───────────────────────────────────────────────────
        // METADATA
        // ───────────────────────────────────────────────────

        // When this event was created in the system
        public DateTime CreatedAt { get; set; }

        // When this event was last modified
        public DateTime LastUpdated { get; set; }

        // When this event was completed (null if not done yet)
        public DateTime? CompletedAt { get; set; }

        // ═══════════════════════════════════════════════════
        // CONSTRUCTOR
        // ═══════════════════════════════════════════════════

        /// <summary>
        /// Default constructor - sets up a new calendar event with default values
        /// TIME COMPLEXITY: O(1)
        /// SPACE COMPLEXITY: O(1)
        /// </summary>
        public CalendarEvent()
        {
            Id = Guid.NewGuid();               // generate a unique ID for this event
            CreatedAt = DateTime.Now;          // record when it was created
            LastUpdated = DateTime.Now;        // set last updated to now
            Status = EventStatus.Scheduled;    // new events start as scheduled
            HasReminder = true;                // reminders are on by default
            ReminderMinutesBefore = 30;        // default reminder is 30 minutes before
        }

        // ═══════════════════════════════════════════════════
        // METHODS
        // ═══════════════════════════════════════════════════

        /// <summary>
        /// Checks whether it is time to send the reminder for this event
        /// Returns true only if:
        /// - Reminder is turned on
        /// - Event is still scheduled
        /// - We are within the reminder window
        /// - Reminder has not already been sent
        ///
        /// TIME COMPLEXITY: O(1)
        /// SPACE COMPLEXITY: O(1)
        /// </summary>
        public bool ShouldSendReminder()
        {
            // No reminder needed if turned off or event is not scheduled
            if (!HasReminder || Status != EventStatus.Scheduled)
                return false;

            // Combine date and time into one DateTime for comparison
            DateTime eventDateTime = EventDate.Add(EventTime.TimeOfDay);

            // How many minutes until the event starts
            TimeSpan timeUntilEvent = eventDateTime - DateTime.Now;

            // Send reminder if:
            // - We are within the reminder window (e.g., 30 minutes before)
            // - The event has not already started (timeUntilEvent > 0)
            // - We have not already sent the reminder
            return timeUntilEvent.TotalMinutes <= ReminderMinutesBefore &&
                   timeUntilEvent.TotalMinutes > 0 &&
                   ReminderSentTime == null;
        }

        /// <summary>
        /// Marks this event as completed and records the completion time
        /// TIME COMPLEXITY: O(1)
        /// SPACE COMPLEXITY: O(1)
        /// </summary>
        public void CompleteEvent()
        {
            Status = EventStatus.Completed;    // update status to completed
            CompletedAt = DateTime.Now;        // record when it was completed
            LastUpdated = DateTime.Now;        // record the update time
        }

        /// <summary>
        /// Cancels this event
        /// TIME COMPLEXITY: O(1)
        /// SPACE COMPLEXITY: O(1)
        /// </summary>
        public void CancelEvent()
        {
            Status = EventStatus.Cancelled;    // update status to cancelled
            LastUpdated = DateTime.Now;        // record the update time
        }

        /// <summary>
        /// Combines the EventDate and EventTime into a single DateTime object
        /// Useful for comparisons and sorting
        ///
        /// TIME COMPLEXITY: O(1)
        /// SPACE COMPLEXITY: O(1)
        /// </summary>
        public DateTime GetEventDateTime()
        {
            // Add the time portion from EventTime to the date from EventDate
            return EventDate.Add(EventTime.TimeOfDay);
        }

        /// <summary>
        /// Returns true if this event is scheduled for today
        /// Compares only the date part, not the time
        ///
        /// TIME COMPLEXITY: O(1)
        /// SPACE COMPLEXITY: O(1)
        /// </summary>
        public bool IsToday()
        {
            // Compare just the date part (ignore time)
            return EventDate.Date == DateTime.Now.Date;
        }

        /// <summary>
        /// Returns true if this event is happening within the next 7 days
        /// Useful for showing upcoming events on the dashboard
        ///
        /// TIME COMPLEXITY: O(1)
        /// SPACE COMPLEXITY: O(1)
        /// </summary>
        public bool IsUpcoming()
        {
            DateTime eventDateTime = GetEventDateTime();

            // Event must be in the future AND within 7 days from now
            return eventDateTime >= DateTime.Now &&
                   eventDateTime <= DateTime.Now.AddDays(7);
        }

        /// <summary>
        /// Returns a readable label for the event type
        /// TIME COMPLEXITY: O(1)
        /// SPACE COMPLEXITY: O(1)
        /// </summary>
        public string GetEventTypeLabel()
        {
            // Return a readable string based on the event type
            if (EventType == EventType.Training)
                return "Training Session";
            else if (EventType == EventType.GoalMilestone)
                return "Goal Milestone";
            else if (EventType == EventType.Reminder)
                return "Reminder";
            else if (EventType == EventType.Competition)
                return "Competition";
            else if (EventType == EventType.Rest)
                return "Rest Day";
            else
                return "Unknown"; // catch any invalid values
        }

        // ═══════════════════════════════════════════════════
        // TOSTRING
        // ═══════════════════════════════════════════════════

        /// <summary>
        /// Returns a readable summary of this calendar event
        /// TIME COMPLEXITY: O(1)
        /// SPACE COMPLEXITY: O(1)
        /// </summary>
        public override string ToString()
        {
            // Return a formatted summary with title, date, time, and duration
            return $"{Title} - {EventDate:MM/dd/yyyy} at {EventTime:HH:mm} " +
                   $"({DurationMinutes} min | {GetEventTypeLabel()} | {Status})";
        }
    }

    // ═══════════════════════════════════════════════════
    // ENUMS
    // ═══════════════════════════════════════════════════

    /// <summary>
    /// The types of events that can be added to the calendar
    /// </summary>
    public enum EventType
    {
        Training,       // a scheduled training session
        GoalMilestone,  // a goal checkpoint or deadline
        Reminder,       // a general reminder
        Competition,    // a game, match, or competition
        Rest            // a scheduled rest or recovery day
    }

    /// <summary>
    /// The possible states a calendar event can be in
    /// </summary>
    public enum EventStatus
    {
        Scheduled,   // event is planned but has not started
        InProgress,  // event is currently happening
        Completed,   // event has finished
        Cancelled    // event was cancelled
    }
}
