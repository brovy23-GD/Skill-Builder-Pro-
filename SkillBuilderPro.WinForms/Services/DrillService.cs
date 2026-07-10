using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SkillBuilderPro.WinForms.Algorithms;
using SkillBuilderPro.WinForms.Models;

namespace SkillBuilderPro.WinForms.Services
{
    /// <summary>
    /// DrillService.cs - Manages drills including storage, retrieval, sorting, and searching
    ///
    /// WHAT IT DOES:
    /// - Stores drills in memory while the app is running
    /// - Saves drills to a text file so data persists between app restarts
    /// - Loads drills back from the file when the app starts
    /// - Provides CRUD operations: Create, Read, Update, Delete
    /// - Sorts drills by difficulty, duration, and skill category using QuickSort
    /// - Searches drills by name, category, and difficulty using SearchingAlgorithms
    ///
    /// FILE FORMAT:
    /// Each drill is stored as one line in drills.txt
    /// Format: Id|Name|SkillCategory|Difficulty|Duration|Description|DateCreated
    /// Example: a1b2c3...|Agility Ladder|Speed|2|20|Run through ladder...|2026-05-26T10:00:00
    ///
    /// DEPENDS ON:
    /// - SortingAlgorithms   - for QuickSort on drill lists
    /// - SearchingAlgorithms - for linear search on drill lists
    ///
    /// TIME COMPLEXITY:
    /// - AddDrill:            O(n) for file save
    /// - GetAllDrills:        O(n) for list copy
    /// - GetDrillById:        O(n) - searches through list
    /// - DeleteDrill:         O(n) - searches then saves
    /// - UpdateDrill:         O(n) - finds by ID then saves
    /// - SortByDifficulty:    O(n log n) - QuickSort
    /// - SortByDuration:      O(n log n) - QuickSort
    /// - SortBySkillCategory: O(n log n) - QuickSort
    /// - SearchByName:        O(n) - linear search
    /// - SearchByDifficulty:  O(n) - linear search
    /// - LoadDrills:          O(n) - reads and parses each line
    /// - SaveDrills:          O(n) - serializes each drill
    ///
    /// SPACE COMPLEXITY: O(n) where n = number of drills stored
    /// </summary>
    public class DrillService
    {
        // ═══════════════════════════════════════════════════
        // FIELDS
        // ═══════════════════════════════════════════════════

        // In-memory list that holds all drills while the app is running
        private List<Drill> _drills;

        // Name of the folder where data files are stored
        private const string DATA_FOLDER = "data";

        // Path to the file where drills are saved
        private const string DRILLS_FILE = "data/drills.txt";

        // ═══════════════════════════════════════════════════
        // CONSTRUCTOR
        // ═══════════════════════════════════════════════════

        /// <summary>
        /// Creates a new DrillService with an empty drill list
        /// Also creates the data folder if it does not exist yet
        /// Call LoadDrills() after creating to load saved drills from file
        ///
        /// TIME COMPLEXITY: O(1)
        /// SPACE COMPLEXITY: O(1)
        /// </summary>
        public DrillService()
        {
            _drills = new List<Drill>(); // start with an empty list
            CreateDataFolder();          // make sure the data folder exists
        }

        // ═══════════════════════════════════════════════════
        // SETUP
        // ═══════════════════════════════════════════════════

        /// <summary>
        /// Creates the data folder if it does not already exist
        /// Called in the constructor to ensure the folder is ready before saving
        ///
        /// TIME COMPLEXITY: O(1)
        /// SPACE COMPLEXITY: O(1)
        /// </summary>
        private void CreateDataFolder()
        {
            // Only create the folder if it does not already exist
            if (!Directory.Exists(DATA_FOLDER))
            {
                Directory.CreateDirectory(DATA_FOLDER); // create the data folder
            }
        }

        // ═══════════════════════════════════════════════════
        // FILE OPERATIONS
        // ═══════════════════════════════════════════════════

        /// <summary>
        /// Loads all saved drills from the text file into memory
        /// Called once when the application starts up
        /// Skips any lines that cannot be parsed without crashing the app
        ///
        /// TIME COMPLEXITY: O(n) where n = number of lines in the file
        /// SPACE COMPLEXITY: O(n) for the loaded drills list
        /// </summary>
        public void LoadDrills()
        {
            // Clear any existing drills before loading fresh from file
            _drills.Clear();

            // If the file does not exist yet there is nothing to load
            if (!File.Exists(DRILLS_FILE))
                return;

            try
            {
                // Read all lines from the drills file at once
                var lines = File.ReadAllLines(DRILLS_FILE);

                // Loop through each line and try to parse it into a drill object
                foreach (var line in lines)
                {
                    // Skip blank lines so they do not cause parse errors
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    // Try to convert this line into a Drill object
                    var drill = ParseDrill(line);

                    // Only add to our list if the parse was successful
                    if (drill != null)
                    {
                        _drills.Add(drill);
                    }
                }
            }
            catch (Exception ex)
            {
                // If reading the file fails print the error but do not crash the app
                Console.WriteLine($"Error loading drills: {ex.Message}");
            }
        }

        /// <summary>
        /// Saves all drills in memory to the text file
        /// Called automatically after any add, update, or delete operation
        ///
        /// TIME COMPLEXITY: O(n) - serializes every drill
        /// SPACE COMPLEXITY: O(n) for the lines array
        /// </summary>
        public void SaveDrills()
        {
            try
            {
                // Convert every drill in our list to a text line
                // TIME: O(n) - processes each drill once
                var lines = _drills.Select(d => SerializeDrill(d)).ToArray();

                // Write all lines to the file at once (overwrites previous content)
                File.WriteAllLines(DRILLS_FILE, lines);
            }
            catch (Exception ex)
            {
                // If saving fails print the error but do not crash the app
                Console.WriteLine($"Error saving drills: {ex.Message}");
            }
        }

        // ═══════════════════════════════════════════════════
        // CRUD OPERATIONS
        // ═══════════════════════════════════════════════════

        /// <summary>
        /// Adds a new drill to the collection and saves to file
        /// Throws an error if a null drill is passed in
        ///
        /// TIME COMPLEXITY: O(n) for file save
        /// SPACE COMPLEXITY: O(1)
        /// </summary>
        public void AddDrill(Drill drill)
        {
            // Make sure a valid drill was passed in
            if (drill == null)
                throw new ArgumentNullException(nameof(drill), "Drill cannot be null");

            _drills.Add(drill); // add drill to the in-memory list
            SaveDrills();       // save updated list to file
        }

        /// <summary>
        /// Returns a copy of all drills in the list
        /// Returns a copy so the original list cannot be modified from outside
        ///
        /// TIME COMPLEXITY: O(n) for list copy
        /// SPACE COMPLEXITY: O(n) for the copy
        /// </summary>
        public List<Drill> GetAllDrills()
        {
            // Return a new copy so the caller cannot accidentally modify our internal list
            return new List<Drill>(_drills);
        }

        /// <summary>
        /// Finds and returns a single drill by its unique ID
        /// Returns null if no drill with that ID is found
        ///
        /// TIME COMPLEXITY: O(n) - searches through the list
        /// SPACE COMPLEXITY: O(1)
        /// </summary>
        public Drill GetDrillById(Guid id)
        {
            // Search the list for the first drill whose ID matches
            // Returns null automatically if nothing is found
            return _drills.FirstOrDefault(d => d.Id == id);
        }

        /// <summary>
        /// Deletes a drill from the collection by its ID
        /// Returns true if the drill was found and deleted
        /// Returns false if no drill with that ID was found
        ///
        /// TIME COMPLEXITY: O(n) - searches then saves
        /// SPACE COMPLEXITY: O(1)
        /// </summary>
        public bool DeleteDrill(Guid id)
        {
            // First find the drill we want to delete
            var drill = GetDrillById(id);

            if (drill != null)
            {
                _drills.Remove(drill); // remove from in-memory list
                SaveDrills();          // save updated list to file
                return true;           // drill was found and deleted
            }

            return false; // drill was not found
        }

        /// <summary>
        /// Updates an existing drill by replacing it with the new version
        /// Matches on the drill's ID to find which one to replace
        /// Returns true if the drill was found and updated
        /// Returns false if no drill with that ID was found
        ///
        /// TIME COMPLEXITY: O(n) - finds by ID then saves
        /// SPACE COMPLEXITY: O(1)
        /// </summary>
        public bool UpdateDrill(Drill drill)
        {
            // Find the existing drill with the same ID
            var existing = GetDrillById(drill.Id);

            if (existing != null)
            {
                // Find the index of the existing drill in the list
                var index = _drills.IndexOf(existing);

                // Replace the old drill with the updated version at the same index
                _drills[index] = drill;

                SaveDrills(); // save updated list to file
                return true;  // drill was found and updated
            }

            return false; // drill was not found
        }

        // ═══════════════════════════════════════════════════
        // SORT METHODS
        // ═══════════════════════════════════════════════════

        /// <summary>
        /// Sorts drills by difficulty level (1=Easy, 2=Medium, 3=Hard)
        /// Uses QuickSort from SortingAlgorithms for efficiency
        /// Can sort ascending (Easy first) or descending (Hard first)
        ///
        /// TIME COMPLEXITY: O(n log n) average - QuickSort
        /// SPACE COMPLEXITY: O(n) for the copy + O(log n) recursion stack
        /// </summary>
        public List<Drill> SortByDifficulty(List<Drill> drillsToSort, bool ascending = true)
        {
            // Make a copy so we do not modify the original list
            var sorted = new List<Drill>(drillsToSort);

            // Build a compare function that compares two drills by difficulty
            // If ascending is true: easy drills come first (1 before 3)
            // If ascending is false: hard drills come first (3 before 1)
            var compare = new Func<Drill, Drill, int>((d1, d2) =>
            {
                int result = d1.Difficulty.CompareTo(d2.Difficulty); // compare difficulty values
                return ascending ? result : -result;                  // flip sign for descending
            });

            // Sort the copy using QuickSort from our algorithms class
            SortingAlgorithms.QuickSort(sorted, compare);
            return sorted;
        }

        /// <summary>
        /// Sorts drills by duration in minutes
        /// Uses QuickSort from SortingAlgorithms for efficiency
        /// Can sort ascending (shortest first) or descending (longest first)
        ///
        /// TIME COMPLEXITY: O(n log n) average - QuickSort
        /// SPACE COMPLEXITY: O(n) for the copy + O(log n) recursion stack
        /// </summary>
        public List<Drill> SortByDuration(List<Drill> drillsToSort, bool ascending = true)
        {
            // Make a copy so we do not modify the original list
            var sorted = new List<Drill>(drillsToSort);

            // Build a compare function that compares two drills by duration
            // If ascending is true: shortest drills come first
            // If ascending is false: longest drills come first
            var compare = new Func<Drill, Drill, int>((d1, d2) =>
            {
                int result = d1.Duration.CompareTo(d2.Duration); // compare duration values
                return ascending ? result : -result;              // flip sign for descending
            });

            // Sort the copy using QuickSort from our algorithms class
            SortingAlgorithms.QuickSort(sorted, compare);
            return sorted;
        }

        /// <summary>
        /// Sorts drills alphabetically by skill category (A to Z)
        /// Uses QuickSort from SortingAlgorithms for efficiency
        ///
        /// Example: Defense comes before Speed which comes before Strength
        ///
        /// TIME COMPLEXITY: O(n log n) average - QuickSort
        /// SPACE COMPLEXITY: O(n) for the copy + O(log n) recursion stack
        /// </summary>
        public List<Drill> SortBySkillCategory(List<Drill> drillsToSort)
        {
            // Make a copy so we do not modify the original list
            var sorted = new List<Drill>(drillsToSort);

            // Build a compare function that compares two drills by skill category alphabetically
            var compare = new Func<Drill, Drill, int>((d1, d2) =>
            {
                // string.CompareTo returns negative, zero, or positive
                // just like our other compare functions
                return d1.SkillCategory.CompareTo(d2.SkillCategory);
            });

            // Sort the copy using QuickSort from our algorithms class
            SortingAlgorithms.QuickSort(sorted, compare);
            return sorted;
        }

        // ═══════════════════════════════════════════════════
        // SEARCH METHODS
        // ═══════════════════════════════════════════════════

        /// <summary>
        /// Searches for drills whose name contains the search term
        /// Uses linear search from SearchingAlgorithms
        /// Search is not case sensitive so "agility" finds "Agility Ladder"
        ///
        /// TIME COMPLEXITY: O(n) - linear search checks every drill
        /// SPACE COMPLEXITY: O(k) where k = number of matching drills
        /// </summary>
        public List<Drill> SearchByName(string searchTerm)
        {
            // Use SearchingAlgorithms to search through the name property of each drill
            return SearchingAlgorithms.SearchByStringProperty(
                _drills,
                searchTerm,
                d => d.Name); // tells the search method to look at the Name property
        }

        /// <summary>
        /// Searches for drills whose skill category contains the search term
        /// Uses linear search from SearchingAlgorithms
        /// Search is not case sensitive so "speed" finds "Speed"
        ///
        /// TIME COMPLEXITY: O(n) - linear search checks every drill
        /// SPACE COMPLEXITY: O(k) where k = number of matching drills
        /// </summary>
        public List<Drill> SearchBySkillCategory(string category)
        {
            // Use SearchingAlgorithms to search through the SkillCategory property
            return SearchingAlgorithms.SearchByStringProperty(
                _drills,
                category,
                d => d.SkillCategory); // tells the search to look at SkillCategory
        }

        /// <summary>
        /// Searches for drills that match a specific difficulty level exactly
        /// Uses linear search from SearchingAlgorithms
        ///
        /// Example: SearchByDifficulty(2) returns all Medium difficulty drills
        ///
        /// TIME COMPLEXITY: O(n) - linear search checks every drill
        /// SPACE COMPLEXITY: O(k) where k = number of matching drills
        /// </summary>
        public List<Drill> SearchByDifficulty(int difficulty)
        {
            // Use SearchingAlgorithms to search through the Difficulty property
            return SearchingAlgorithms.SearchByIntProperty(
                _drills,
                difficulty,
                d => d.Difficulty); // tells the search to look at the Difficulty property
        }

        /// <summary>
        /// Searches for all active drills only
        /// Returns only drills where IsActive is true
        ///
        /// TIME COMPLEXITY: O(n) - checks every drill
        /// SPACE COMPLEXITY: O(k) where k = number of active drills
        /// </summary>
        public List<Drill> GetActiveDrills()
        {
            // Create an empty list to hold only active drills
            var activeDrills = new List<Drill>();

            // Loop through every drill and keep the ones that are active
            foreach (var drill in _drills)
            {
                if (drill.IsActive)
                    activeDrills.Add(drill); // only add drills that are active
            }

            return activeDrills;
        }

        /// <summary>
        /// Returns the total number of drills in the collection
        ///
        /// TIME COMPLEXITY: O(1) - List.Count is instant
        /// SPACE COMPLEXITY: O(1)
        /// </summary>
        public int GetDrillCount()
        {
            return _drills.Count; // return the total number of drills stored
        }

        // ═══════════════════════════════════════════════════
        // SERIALIZATION HELPERS
        // ═══════════════════════════════════════════════════

        /// <summary>
        /// Converts a Drill object into a single pipe-delimited text line for file storage
        ///
        /// OUTPUT FORMAT:
        /// Id|Name|SkillCategory|Difficulty|Duration|Description|DateCreated
        ///
        /// EXAMPLE:
        /// a1b2c3...|Agility Ladder|Speed|2|20|Run through the ladder...|2026-05-26T10:00:00
        ///
        /// TIME COMPLEXITY: O(1) - simple string formatting
        /// SPACE COMPLEXITY: O(1)
        /// </summary>
        private string SerializeDrill(Drill drill)
        {
            // Format: Id|Name|SkillCategory|Difficulty|Duration|Description|DateCreated
            // Using :O format for DateTime gives us a full ISO 8601 string that parses back perfectly
            return $"{drill.Id}|" +
                   $"{drill.Name}|" +
                   $"{drill.SkillCategory}|" +
                   $"{drill.Difficulty}|" +
                   $"{drill.Duration}|" +
                   $"{drill.Description}|" +
                   $"{drill.DateCreated:O}";
        }

        /// <summary>
        /// Converts a pipe-delimited text line back into a Drill object
        /// Returns null if the line cannot be parsed so bad data does not crash the app
        ///
        /// EXPECTED FORMAT:
        /// Id|Name|SkillCategory|Difficulty|Duration|Description|DateCreated
        ///
        /// TIME COMPLEXITY: O(1) - fixed number of fields to parse
        /// SPACE COMPLEXITY: O(1)
        /// </summary>
        private Drill ParseDrill(string line)
        {
            try
            {
                // Split the line by the pipe character to get each field
                var parts = line.Split('|');

                // Make sure we have all 7 required fields
                // If the line is malformed skip it
                if (parts.Length < 7)
                    return null;

                // Build and return a new Drill object from the parsed fields
                return new Drill
                {
                    Id = Guid.Parse(parts[0]),        // unique ID
                    Name = parts[1],                     // drill name
                    SkillCategory = parts[2],                     // skill category
                    Difficulty = int.Parse(parts[3]),          // difficulty level 1-3
                    Duration = int.Parse(parts[4]),          // duration in minutes
                    Description = parts[5],                     // description text
                    DateCreated = DateTime.Parse(parts[6])      // original creation date
                };
            }
            catch
            {
                // If anything goes wrong parsing just return null
                // The caller will skip null drills so the app keeps running
                return null;
            }
        }
    }
}
