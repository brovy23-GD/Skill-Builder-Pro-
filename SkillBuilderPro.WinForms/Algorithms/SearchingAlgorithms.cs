using System;
using System.Collections.Generic;
using SkillBuilderPro.WinForms.Models;

namespace SkillBuilderPro.WinForms.Algorithms
{
    /// <summary>
    /// SearchingAlgorithms.cs - Collection of searching algorithms for drill lookup
    ///
    /// WHAT IT DOES:
    /// - Provides multiple ways to search through lists of drills
    /// - Linear Search for unsorted lists or partial text matches
    /// - Binary Search for sorted lists when speed matters
    /// - Property-based search for filtering by specific fields
    ///
    /// WHEN TO USE EACH:
    /// - LinearSearch:           small lists or unsorted data
    /// - BinarySearch:           large sorted lists where speed is important
    /// - SearchByIntProperty:    filter by a number like difficulty level
    /// - SearchByStringProperty: filter by text like category or name
    ///
    /// TIME COMPLEXITY:
    /// - LinearSearch:           O(n)
    /// - LinearSearchAll:        O(n)
    /// - BinarySearch:           O(log n)
    /// - SearchByIntProperty:    O(n)
    /// - SearchByStringProperty: O(n)
    ///
    /// SPACE COMPLEXITY: O(1) for index returns, O(k) for list returns
    /// </summary>
    public static class SearchingAlgorithms
    {
        // ═══════════════════════════════════════════════════
        // LINEAR SEARCH
        // ═══════════════════════════════════════════════════

        /// <summary>
        /// Linear Search - finds the FIRST drill that matches the search term
        ///
        /// HOW IT WORKS:
        /// 1. Start at the first drill
        /// 2. Check if it contains the search term
        /// 3. If yes return its index
        /// 4. If no move to the next drill
        /// 5. Return -1 if nothing was found
        ///
        /// TIME COMPLEXITY: O(n) - checks every drill in worst case
        /// SPACE COMPLEXITY: O(1) - only stores one index number
        /// </summary>
        public static int LinearSearch(
            List<Drill> drills,           // the list to search
            string searchTerm,            // the text to look for
            Func<Drill, string> selector) // which property to search
        {
            // Loop through every drill one by one
            for (int i = 0; i < drills.Count; i++)
            {
                // Check if this drill's property contains the search term
                // OrdinalIgnoreCase means search is not case sensitive
                if (selector(drills[i]).Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                {
                    return i; // found it - return the index
                }
            }

            return -1; // not found - return -1
        }

        /// <summary>
        /// Linear Search All - finds ALL drills that match the search term
        /// Returns a list of index positions for every match
        ///
        /// TIME COMPLEXITY: O(n) - always checks every drill
        /// SPACE COMPLEXITY: O(k) where k = number of matches
        /// </summary>
        public static List<int> LinearSearchAll(
            List<Drill> drills,           // the list to search
            string searchTerm,            // the text to look for
            Func<Drill, string> selector) // which property to search
        {
            // List to hold all matching index positions
            var matches = new List<int>();

            // Loop through every drill
            for (int i = 0; i < drills.Count; i++)
            {
                // If this drill matches add its index to results
                if (selector(drills[i]).Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                {
                    matches.Add(i); // save the matching index
                }
            }

            return matches; // return all matching indexes
        }

        // ═══════════════════════════════════════════════════
        // BINARY SEARCH
        // ═══════════════════════════════════════════════════

        /// <summary>
        /// Binary Search - finds a drill in a SORTED list much faster than linear
        ///
        /// HOW IT WORKS:
        /// 1. Start with left = 0, right = last index
        /// 2. Check the middle item
        /// 3. If match return the index
        /// 4. If target is greater move left pointer up (search right half)
        /// 5. If target is smaller move right pointer down (search left half)
        /// 6. Repeat until found or left > right
        ///
        /// REQUIRES: list must be SORTED before calling this
        ///
        /// TIME COMPLEXITY: O(log n) - cuts search space in half each step
        /// SPACE COMPLEXITY: O(1) - only stores left, right, mid pointers
        /// </summary>
        public static int BinarySearch(
            List<Drill> drills,              // the SORTED list to search
            Drill target,                    // the drill to find
            Func<Drill, Drill, int> compare) // comparison function
        {
            int left = 0;                  // start of search range
            int right = drills.Count - 1;   // end of search range

            // Keep searching while there is still a range to check
            while (left <= right)
            {
                // Calculate middle index
                // Using left + (right - left) / 2 prevents integer overflow
                int mid = left + (right - left) / 2;

                // Compare the middle drill to the target
                int cmp = compare(drills[mid], target);

                if (cmp == 0)
                {
                    return mid; // exact match found
                }
                else if (cmp < 0)
                {
                    left = mid + 1; // middle too small - search right half
                }
                else
                {
                    right = mid - 1; // middle too large - search left half
                }
            }

            return -1; // not found
        }

        // ═══════════════════════════════════════════════════
        // PROPERTY SEARCH METHODS
        // ═══════════════════════════════════════════════════

        /// <summary>
        /// Search by Integer Property - finds all drills where a number equals a value
        ///
        /// EXAMPLE USES:
        /// - Find all drills with Difficulty == 3
        /// - Find all drills with Duration == 20
        ///
        /// TIME COMPLEXITY: O(n) - checks every drill
        /// SPACE COMPLEXITY: O(k) where k = number of matches
        /// </summary>
        public static List<Drill> SearchByIntProperty(
            List<Drill> drills,        // the list to search
            int value,                 // the number to match exactly
            Func<Drill, int> selector) // which int property to check
        {
            // List to hold all matching drills
            var results = new List<Drill>();

            // Loop through every drill and check the selected property
            foreach (var drill in drills)
            {
                if (selector(drill) == value)
                {
                    results.Add(drill); // match found - add to results
                }
            }

            return results; // return all matching drills
        }

        /// <summary>
        /// Search by String Property - finds all drills where a text property
        /// contains the search term
        ///
        /// EXAMPLE USES:
        /// - Find all drills where Name contains "Agility"
        /// - Find all drills where SkillCategory contains "Speed"
        /// - Find all drills where Description contains "footwork"
        ///
        /// Search is NOT case sensitive so "speed" finds "Speed"
        ///
        /// TIME COMPLEXITY: O(n) - checks every drill
        /// SPACE COMPLEXITY: O(k) where k = number of matches
        /// </summary>
        public static List<Drill> SearchByStringProperty(
            List<Drill> drills,           // the list to search
            string searchTerm,            // the text to look for
            Func<Drill, string> selector) // which string property to check
        {
            // List to hold all matching drills
            var results = new List<Drill>();

            // Loop through every drill and check the selected property
            foreach (var drill in drills)
            {
                // Check if the property contains the search term
                // OrdinalIgnoreCase means not case sensitive
                if (selector(drill).Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                {
                    results.Add(drill); // match found - add to results
                }
            }

            return results; // return all matching drills
        }
    }
}
