using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SkillBuilderPro.WinForms.Models;

namespace SkillBuilderPro.WinForms.Services
{
    /// <summary>
    /// Service for managing calendar events and reminders
    /// </summary>
    public class CalendarService
    {
        private List<CalendarEvent> events;
        private const string CALENDAR_FILE = "data/calendar.txt";

        public CalendarService()
        {
            events = new List<CalendarEvent>();
        }

        /// <summary>
        /// Loads all calendar events from file
        /// </summary>
        public void LoadCalendarEvents()
        {
            events.Clear();

            if (!File.Exists(CALENDAR_FILE))
                return;

            try
            {
                var lines = File.ReadAllLines(CALENDAR_FILE);
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var evt = ParseCalendarEvent(line);
                    if (evt != null)
                    {
                        events.Add(evt);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading calendar events: {ex.Message}");
            }
        }

        /// <summary>
        /// Saves all calendar events to file
        /// </summary>
        public void SaveCalendarEvents()
        {
            try
            {
                var lines = events.Select(e => SerializeCalendarEvent(e)).ToArray();
                File.WriteAllLines(CALENDAR_FILE, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving calendar events: {ex.Message}");
            }
        }

        /// <summary>
        /// Adds a new calendar event
        /// </summary>
        public void AddCalendarEvent(CalendarEvent evt)
        {
            if (evt == null)
                throw new ArgumentNullException(nameof(evt));

            events.Add(evt);
            SaveCalendarEvents();
        }

        /// <summary>
        /// Gets all events for a user
        /// </summary>
        public List<CalendarEvent> GetUserEvents(Guid userId)
        {
            return events.Where(e => e.UserId == userId).OrderBy(e => e.EventDate).ToList();
        }

        /// <summary>
        /// Gets today's events
        /// </summary>
        public List<CalendarEvent> GetTodayEvents(Guid userId)
        {
            return GetUserEvents(userId)
                .Where(e => e.EventDate.Date == DateTime.Now.Date)
                .OrderBy(e => e.EventTime)
                .ToList();
        }

        /// <summary>
        /// Gets upcoming events (next N days)
        /// </summary>
        public List<CalendarEvent> GetUpcomingEvents(Guid userId, int days)
        {
            var cutoffDate = DateTime.Now.AddDays(days);
            return GetUserEvents(userId)
                .Where(e => e.EventDate.Date >= DateTime.Now.Date && e.EventDate.Date <= cutoffDate.Date)
                .OrderBy(e => e.EventDate)
                .ThenBy(e => e.EventTime)
                .ToList();
        }

        /// <summary>
        /// Gets events for a specific date
        /// </summary>
        public List<CalendarEvent> GetEventsForDate(Guid userId, DateTime date)
        {
            return GetUserEvents(userId)
                .Where(e => e.EventDate.Date == date.Date)
                .OrderBy(e => e.EventTime)
                .ToList();
        }

        /// <summary>
        /// Gets events by type
        /// </summary>
        public List<CalendarEvent> GetEventsByType(Guid userId, EventType eventType)
        {
            return GetUserEvents(userId)
                .Where(e => e.EventType == eventType && e.Status != EventStatus.Cancelled)
                .ToList();
        }

        /// <summary>
        /// Gets a specific event by ID
        /// </summary>
        public CalendarEvent GetEventById(Guid id)
        {
            return events.FirstOrDefault(e => e.Id == id);
        }

        /// <summary>
        /// Updates a calendar event
        /// </summary>
        public bool UpdateCalendarEvent(CalendarEvent evt)
        {
            var existing = GetEventById(evt.Id);
            if (existing != null)
            {
                var index = events.IndexOf(existing);
                events[index] = evt;
                SaveCalendarEvents();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Deletes a calendar event
        /// </summary>
        public bool DeleteCalendarEvent(Guid id)
        {
            var evt = GetEventById(id);
            if (evt != null)
            {
                events.Remove(evt);
                SaveCalendarEvents();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Cancels an event
        /// </summary>
        public bool CancelEvent(Guid id)
        {
            var evt = GetEventById(id);
            if (evt != null && evt.Status != EventStatus.Completed)
            {
                evt.Status = EventStatus.Cancelled;
                SaveCalendarEvents();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Completes an event
        /// </summary>
        public bool CompleteEvent(Guid id)
        {
            var evt = GetEventById(id);
            if (evt != null)
            {
                evt.CompleteEvent();
                SaveCalendarEvents();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks and processes reminders that need to be sent
        /// </summary>
        public List<CalendarEvent> CheckReminders(Guid userId)
        {
            var remindersToSend = new List<CalendarEvent>();

            var userEvents = GetUserEvents(userId)
                .Where(e => e.HasReminder && e.Status == EventStatus.Scheduled)
                .ToList();

            foreach (var evt in userEvents)
            {
                if (evt.ShouldSendReminder())
                {
                    remindersToSend.Add(evt);
                    evt.ReminderSentTime = DateTime.Now;
                }
            }

            if (remindersToSend.Count > 0)
            {
                SaveCalendarEvents();
            }

            return remindersToSend;
        }

        /// <summary>
        /// Gets events that need reminders sent soon
        /// </summary>
        public List<CalendarEvent> GetEventsDueForReminder(Guid userId)
        {
            return GetUserEvents(userId)
                .Where(e => e.HasReminder && e.Status == EventStatus.Scheduled && 
                           !e.ReminderSentTime.HasValue &&
                           e.IsUpcoming())
                .OrderBy(e => e.GetEventDateTime())
                .ToList();
        }

        /// <summary>
        /// Gets this week's events
        /// </summary>
        public List<CalendarEvent> GetWeeklyEvents(Guid userId)
        {
            return GetUpcomingEvents(userId, 7);
        }

        /// <summary>
        /// Gets this month's events
        /// </summary>
        public List<CalendarEvent> GetMonthlyEvents(Guid userId)
        {
            int daysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            return GetUpcomingEvents(userId, daysInMonth);
        }

        /// <summary>
        /// Counts training sessions scheduled
        /// </summary>
        public int GetScheduledTrainingCount(Guid userId, int days)
        {
            return GetUpcomingEvents(userId, days)
                .Count(e => e.EventType == EventType.Training);
        }

        /// <summary>
        /// Searches events by title
        /// </summary>
        public List<CalendarEvent> SearchByTitle(Guid userId, string searchTerm)
        {
            return GetUserEvents(userId)
                .Where(e => e.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// Serializes calendar event to pipe-delimited string
        /// </summary>
        private string SerializeCalendarEvent(CalendarEvent evt)
        {
            return $"{evt.Id}|{evt.UserId}|{evt.Title}|{evt.Description}|{(int)evt.EventType}|{evt.EventDate:O}|{evt.EventTime:HH:mm}|{evt.DurationMinutes}|{evt.HasReminder}|{evt.ReminderMinutesBefore}|{(evt.ReminderSentTime.HasValue ? evt.ReminderSentTime.Value.ToString("O") : "")}|{(int)evt.Status}|{(evt.RelatedGoalId.HasValue ? evt.RelatedGoalId.ToString() : "")}|{(evt.RelatedSessionId.HasValue ? evt.RelatedSessionId.ToString() : "")}|{evt.CreatedAt:O}|{(evt.CompletedAt.HasValue ? evt.CompletedAt.Value.ToString("O") : "")}";
        }

        /// <summary>
        /// Deserializes calendar event from pipe-delimited string
        /// </summary>
        private CalendarEvent ParseCalendarEvent(string line)
        {
            try
            {
                var parts = line.Split('|');
                if (parts.Length < 16)
                    return null;

                var evt = new CalendarEvent
                {
                    Id = Guid.Parse(parts[0]),
                    UserId = Guid.Parse(parts[1]),
                    Title = parts[2],
                    Description = parts[3],
                    EventType = (EventType)int.Parse(parts[4]),
                    EventDate = DateTime.Parse(parts[5]),
                    EventTime = DateTime.Parse($"2000-01-01 {parts[6]}"),
                    DurationMinutes = int.Parse(parts[7]),
                    HasReminder = bool.Parse(parts[8]),
                    ReminderMinutesBefore = int.Parse(parts[9]),
                    ReminderSentTime = string.IsNullOrWhiteSpace(parts[10]) ? null : DateTime.Parse(parts[10]),
                    Status = (EventStatus)int.Parse(parts[11]),
                    RelatedGoalId = string.IsNullOrWhiteSpace(parts[12]) ? null : Guid.Parse(parts[12]),
                    RelatedSessionId = string.IsNullOrWhiteSpace(parts[13]) ? null : Guid.Parse(parts[13]),
                    CreatedAt = DateTime.Parse(parts[14]),
                    CompletedAt = string.IsNullOrWhiteSpace(parts[15]) ? null : DateTime.Parse(parts[15])
                };

                return evt;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets calendar event count
        /// </summary>
        public int GetEventCount()
        {
            return events.Count;
        }
    }
}

