using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SkillBuilderPro.WinForms.Models;

namespace SkillBuilderPro.WinForms.Services
{
    /// <summary>
    /// SessionService.cs - Manages training sessions including storage and retrieval
    ///
    /// WHAT IT DOES:
    /// - Stores training sessions in memory while the app is running
    /// - Saves sessions to a text file so data persists between app restarts
    /// - Loads sessions back from the file when the app starts
    /// - Provides methods to add, get, delete, and filter sessions
    ///
    /// FILE FORMAT:
    /// Each session is stored as one line in sessions.txt
    /// Format: Id|Name|Date|DrillCount|DrillIds|DateCreated
    /// Example: a1b2...|Morning Workout|2026-05-26T08:00:00|3|d1,d2,d3|2026-05-01T10:00:00
    ///
    /// TIME COMPLEXITY:
    /// - AddSession:        O(n) for file save
    /// - GetAllSessions:    O(n) for list copy
    /// - GetSessionById:    O(n) - searches through list
    /// - DeleteSession:     O(n) - searches then saves
    /// - GetSessionsByDate: O(n) - filters through list
    /// - LoadSessions:      O(n) - reads and parses each line
    /// - SaveSessions:      O(n) - serializes each session
    ///
    /// SPACE COMPLEXITY: O(n) where n = number of sessions stored
    /// </summary>
    public class SessionService
    {
        // ═══════════════════════════════════════════════════
        // FIELDS
        // ═══════════════════════════════════════════════════

        // In-memory list that holds all sessions while the app is running
        private List<TrainingSession> _sessions;

        // Name of the folder where data files are stored
        private const string DATA_FOLDER = "data";

        // Path to the file where sessions are saved
        private const string SESSIONS_FILE = "data/sessions.txt";

        // ═══════════════════════════════════════════════════
        // CONSTRUCTOR
        // ═══════════════════════════════════════════════════

        /// <summary>
        /// Creates a new SessionService with an empty session list
        /// Call LoadSessions() after creating to load saved sessions from file
        ///
        /// TIME COMPLEXITY: O(1)
        /// SPACE COMPLEXITY: O(1)
        /// </summary>
        public SessionService()
        {
            _sessions = new List<TrainingSession>(); // start with an empty list
            CreateDataFolder();                       // make sure data folder exists
        }

        // ═══════════════════════════════════════════════════
        // SETUP
        // ═══════════════════════════════════════════════════

        /// <summary>
        /// Creates the data folder if it does not already exist
        ///
        /// TIME COMPLEXITY: O(1)
        /// SPACE COMPLEXITY: O(1)
        /// </summary>
        private void CreateDataFolder()
        {
            if (!Directory.Exists(DATA_FOLDER))
            {
                Directory.CreateDirectory(DATA_FOLDER); // create the data folder
            }
        }

        // ═══════════════════════════════════════════════════
        // FILE OPERATIONS
        // ═══════════════════════════════════════════════════

        /// <summary>
        /// Loads all saved sessions from the text file into memory
        /// Called once when the application starts up
        /// Skips any lines that cannot be parsed without crashing
        ///
        /// TIME COMPLEXITY: O(n) where n = number of lines in the file
        /// SPACE COMPLEXITY: O(n) for the loaded sessions list
        /// </summary>
        public void LoadSessions()
        {
            // Clear any existing sessions before loading fresh from file
            _sessions.Clear();

            // If the file does not exist yet there is nothing to load
            if (!File.Exists(SESSIONS_FILE))
                return;

            try
            {
                // Read all lines from the sessions file at once
                var lines = File.ReadAllLines(SESSIONS_FILE);

                // Loop through each line and try to parse it into a session object
                foreach (var line in lines)
                {
                    // Skip blank lines
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    // Try to convert this line into a TrainingSession object
                    var session = ParseSession(line);

                    // Only add to our list if the parse was successful
                    if (session != null)
                    {
                        _sessions.Add(session);
                    }
                }
            }
            catch (Exception ex)
            {
                // If reading fails print the error but do not crash the app
                Console.WriteLine($"Error loading sessions: {ex.Message}");
            }
        }

        /// <summary>
        /// Saves all sessions in memory to the text file
        /// Called automatically after any add or delete operation
        ///
        /// TIME COMPLEXITY: O(n) - serializes every session
        /// SPACE COMPLEXITY: O(n) for the lines array
        /// </summary>
        public void SaveSessions()
        {
            try
            {
                // Convert every session in our list to a text line
                var lines = _sessions.Select(s => SerializeSession(s)).ToArray();

                // Write all lines to the file at once
                File.WriteAllLines(SESSIONS_FILE, lines);
            }
            catch (Exception ex)
            {
                // If saving fails print the error but do not crash
                Console.WriteLine($"Error saving sessions: {ex.Message}");
            }
        }

        // ═══════════════════════════════════════════════════
        // CRUD OPERATIONS
        // ═══════════════════════════════════════════════════

        /// <summary>
        /// Adds a new training session to the collection and saves to file
        /// Throws an error if a null session is passed in
        ///
        /// TIME COMPLEXITY: O(n) for file save
        /// SPACE COMPLEXITY: O(1)
        /// </summary>
        public void AddSession(TrainingSession session)
        {
            // Make sure a valid session was passed in
            if (session == null)
                throw new ArgumentNullException(nameof(session), "Session cannot be null");

            _sessions.Add(session); // add session to the in-memory list
            SaveSessions();         // save updated list to file
        }

        /// <summary>
        /// Returns a copy of all sessions in the list
        /// Returns a copy so the original list cannot be modified from outside
        ///
        /// TIME COMPLEXITY: O(n) for list copy
        /// SPACE COMPLEXITY: O(n) for the copy
        /// </summary>
        public List<TrainingSession> GetAllSessions()
        {
            // Return a new copy so the caller cannot modify our internal list
            return new List<TrainingSession>(_sessions);
        }

        /// <summary>
        /// Finds and returns a single session by its unique ID
        /// Returns null if no session with that ID is found
        ///
        /// TIME COMPLEXITY: O(n) - searches through the list
        /// SPACE COMPLEXITY: O(1)
        /// </summary>
        public TrainingSession GetSessionById(Guid id)
        {
            // Search for the first session whose ID matches
            // Returns null automatically if nothing is found
            return _sessions.FirstOrDefault(s => s.Id == id);
        }

        /// <summary>
        /// Deletes a session from the collection by its ID
        /// Returns true if found and deleted, false if not found
        ///
        /// TIME COMPLEXITY: O(n) - searches then saves
        /// SPACE COMPLEXITY: O(1)
        /// </summary>
        public bool DeleteSession(Guid id)
        {
            // First find the session we want to delete
            var session = GetSessionById(id);

            if (session != null)
            {
                _sessions.Remove(session); // remove from in-memory list
                SaveSessions();            // save updated list to file
                return true;               // session was found and deleted
            }

            return false; // session was not found
        }

        /// <summary>
        /// Returns all sessions that are scheduled on a specific date
        /// Compares only the date part and ignores the time
        ///
        /// TIME COMPLEXITY: O(n) - checks every session
        /// SPACE COMPLEXITY: O(k) where k = number of matching sessions
        /// </summary>
        public List<TrainingSession> GetSessionsByDate(DateTime date)
        {
            // Create an empty list for sessions on this date
            var result = new List<TrainingSession>();

            // Loop through every session and keep the ones on the matching date
            foreach (var session in _sessions)
            {
                // Compare only the date part - ignore the time portion
                if (session.Date.Date == date.Date)
                    result.Add(session);
            }

            return result;
        }

        /// <summary>
        /// Returns the total number of sessions in the collection
        ///
        /// TIME COMPLEXITY: O(1)
        /// SPACE COMPLEXITY: O(1)
        /// </summary>
        public int GetSessionCount()
        {
            return _sessions.Count; // return the total number of sessions stored
        }

        // ═══════════════════════════════════════════════════
        // SERIALIZATION HELPERS
        // ═══════════════════════════════════════════════════

        /// <summary>
        /// Converts a TrainingSession object into a pipe-delimited text line for file storage
        ///
        /// OUTPUT FORMAT:
        /// Id|Name|Date|DrillCount|DrillIds|DateCreated
        ///
        /// EXAMPLE:
        /// a1b2...|Morning Workout|2026-05-26T08:00:00|3|d1id,d2id,d3id|2026-05-01T10:00:00
        ///
        /// TIME COMPLEXITY: O(m) where m = number of drills in the session
        /// SPACE COMPLEXITY: O(m) for the drill ID string
        /// </summary>
        private string SerializeSession(TrainingSession session)
        {
            // Join all drill IDs into one comma separated string
            // Example: "d1id,d2id,d3id"
            var drillIds = string.Join(",", session.Drills.Select(d => d.Id));

            // Build the full pipe delimited line
            // Using :O for DateTime gives us a full ISO 8601 string that parses back perfectly
            return $"{session.Id}|" +
                   $"{session.Name}|" +
                   $"{session.Date:O}|" +
                   $"{session.Drills.Count}|" +
                   $"{drillIds}|" +
                   $"{session.DateCreated:O}";
        }

        /// <summary>
        /// Converts a pipe-delimited text line back into a TrainingSession object
        /// Returns null if the line cannot be parsed so bad data does not crash the app
        ///
        /// NOTE:
        /// In this version drill objects are not fully reconstructed from the file
        /// because DrillService manages drills separately
        /// Drill IDs are stored so they can be linked in a future update
        ///
        /// EXPECTED FORMAT:
        /// Id|Name|Date|DrillCount|DrillIds|DateCreated
        ///
        /// TIME COMPLEXITY: O(1) - fixed number of fields to parse
        /// SPACE COMPLEXITY: O(1)
        /// </summary>
        private TrainingSession ParseSession(string line)
        {
            try
            {
                // Split the line by the pipe character to get each field
                var parts = line.Split('|');

                // Make sure we have all 6 required fields
                // If the line is malformed skip it and return null
                if (parts.Length < 6)
                    return null;

                // Build and return a new TrainingSession from the parsed fields
                var session = new TrainingSession
                {
                    Id = Guid.Parse(parts[0]),     // unique session ID
                    Name = parts[1],                  // session name
                    Date = DateTime.Parse(parts[2]),  // scheduled date
                    Drills = new List<Drill>(),         // empty for now - drills managed separately
                    DateCreated = DateTime.Parse(parts[5])   // original creation date
                };

                // Note: parts[3] = drill count and parts[4] = drill IDs
                // These are stored for reference but drill objects are not
                // reconstructed here because DrillService manages drill data
                // In a future update you could link drills here using DrillService

                return session;
            }
            catch
            {
                // If anything goes wrong parsing just return null
                // The caller will skip null sessions so the app keeps running
                return null;
            }
        }
    }
}
