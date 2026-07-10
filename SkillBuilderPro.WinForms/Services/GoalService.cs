using SkillBuilderPro.WinForms.Models;
using System;
using System.Collections.Generic;

namespace SkillBuilderPro.WinForms.Services
{
    /// <summary>
    /// GoalService.cs - Manages goal creation, retrieval, updates, and sorting
    ///
    /// Provides:
    /// - Create, read, update, complete, and abandon goals
    /// - Sort goals by progress, priority, and title
    /// - Filter goals by athlete, status, and priority
    /// - Calculate goal statistics
    ///
    /// TIME COMPLEXITY:
    /// - GetGoalsByAthlete:   O(n)
    /// - GetGoalsByStatus:    O(n)
    /// - SortGoalsByProgress: O(n log n) - QuickSort
    /// - SortGoalsByPriority: O(n)       - Counting Sort (only 3 priority levels)
    /// - SortGoalsByTitle:    O(n log n) - QuickSort
    ///
    /// SPACE COMPLEXITY: O(n) for storage
    /// </summary>
    public class GoalService
    {
        // Private list that stores all goals in memory
        private List<Goal> _goals;

        // Constructor: sets up the goal list and loads sample data
        public GoalService()
        {
            _goals = new List<Goal>();
            InitializeSampleGoals();
        }

        // ---------------------------------------------------
        // SAMPLE DATA
        // ---------------------------------------------------

        /// <summary>
        /// Loads sample goals into the list for testing purposes
        /// TIME COMPLEXITY: O(1) - fixed number of items added
        /// </summary>
        private void InitializeSampleGoals()
        {
            _goals = new List<Goal>
            {
                new Goal
                {
                    Id = new Guid("a1111111-1111-1111-1111-111111111111"),
                    UserId = new Guid("11111111-1111-1111-1111-111111111111"),
                    Title = "Improve Shooting Accuracy",
                    Description = "Increase free throw percentage to 90%",
                    TargetMetric = "90% free throw accuracy",
                    ProgressPercentage = 75,
                    Priority = 1,
                    Status = GoalStatus.Active,
                    StartDate = new DateTime(2026, 4, 1),
                    TargetDate = new DateTime(2026, 7, 1),
                    DateCreated = new DateTime(2026, 4, 1),
                    LastUpdated = new DateTime(2026, 4, 1)
                },
                new Goal
                {
                    Id = new Guid("a2222222-2222-2222-2222-222222222222"),
                    UserId = new Guid("11111111-1111-1111-1111-111111111111"),
                    Title = "Increase Court Vision",
                    Description = "More assists per game",
                    TargetMetric = "8 assists per game",
                    ProgressPercentage = 60,
                    Priority = 2,
                    Status = GoalStatus.Active,
                    StartDate = new DateTime(2026, 4, 15),
                    TargetDate = new DateTime(2026, 8, 1),
                    DateCreated = new DateTime(2026, 4, 15),
                    LastUpdated = new DateTime(2026, 4, 15)
                },
                new Goal
                {
                    Id = new Guid("a3333333-3333-3333-3333-333333333333"),
                    UserId = new Guid("11111111-1111-1111-1111-111111111111"),
                    Title = "Perfect Defense",
                    Description = "Master defensive positioning",
                    TargetMetric = "100% positioning score",
                    ProgressPercentage = 85,
                    Priority = 1,
                    Status = GoalStatus.Active,
                    StartDate = new DateTime(2026, 3, 1),
                    TargetDate = new DateTime(2026, 6, 1),
                    DateCreated = new DateTime(2026, 3, 1),
                    LastUpdated = new DateTime(2026, 3, 1)
                },
                new Goal
                {
                    Id = new Guid("a4444444-4444-4444-4444-444444444444"),
                    UserId = new Guid("11111111-1111-1111-1111-111111111111"),
                    Title = "Build Strength",
                    Description = "Increase bench press by 50 lbs",
                    TargetMetric = "250 lb bench press",
                    ProgressPercentage = 80,
                    Priority = 2,
                    Status = GoalStatus.Active,
                    StartDate = new DateTime(2026, 5, 1),
                    TargetDate = new DateTime(2026, 9, 1),
                    DateCreated = new DateTime(2026, 5, 1),
                    LastUpdated = new DateTime(2026, 5, 1)
                },
                new Goal
                {
                    Id = new Guid("a5555555-5555-5555-5555-555555555555"),
                    UserId = new Guid("11111111-1111-1111-1111-111111111111"),
                    Title = "Speed Training",
                    Description = "Improve vertical jump by 6 inches",
                    TargetMetric = "36 inch vertical jump",
                    ProgressPercentage = 100,
                    Priority = 3,
                    Status = GoalStatus.Completed,
                    StartDate = new DateTime(2026, 2, 1),
                    TargetDate = new DateTime(2026, 5, 1),
                    CompletedDate = new DateTime(2026, 5, 1),
                    DateCreated = new DateTime(2026, 2, 1),
                    LastUpdated = new DateTime(2026, 5, 1)
                }
            };
        }

        // ---------------------------------------------------
        // GET / FILTER METHODS
        // ---------------------------------------------------

        /// <summary>
        /// Returns a copy of all goals in the list
        /// TIME COMPLEXITY: O(1)
        /// SPACE COMPLEXITY: O(n) for the copy
        /// </summary>
        public List<Goal> GetAllGoals()
        {
            // Return a new copy so the original list stays protected
            return new List<Goal>(_goals);
        }

        /// <summary>
        /// Returns all goals that belong to a specific athlete
        /// TIME COMPLEXITY: O(n) - must check every goal
        /// SPACE COMPLEXITY: O(k) where k = number of matching goals
        /// </summary>
        public List<Goal> GetGoalsByAthlete(Guid userId)
        {
            var filtered = new List<Goal>();

            // Loop through every goal and keep the ones that match the userId
            foreach (var goal in _goals)
            {
                if (goal.UserId == userId)
                    filtered.Add(goal); // add matching goal to result list
            }

            return filtered;
        }

        /// <summary>
        /// Returns all goals that match a specific status (Active, Completed, Abandoned)
        /// TIME COMPLEXITY: O(n) - must check every goal
        /// SPACE COMPLEXITY: O(k) where k = number of matching goals
        /// </summary>
        public List<Goal> GetGoalsByStatus(GoalStatus status)
        {
            var filtered = new List<Goal>();

            // Loop through every goal and keep the ones that match the status
            foreach (var goal in _goals)
            {
                if (goal.Status == status)
                    filtered.Add(goal); // add matching goal to result list
            }

            return filtered;
        }

        /// <summary>
        /// Returns all goals that match a specific priority level (1, 2, or 3)
        /// TIME COMPLEXITY: O(n) - must check every goal
        /// SPACE COMPLEXITY: O(k) where k = number of matching goals
        /// </summary>
        public List<Goal> GetGoalsByPriority(int priority)
        {
            var filtered = new List<Goal>();

            // Loop through every goal and keep the ones that match the priority
            foreach (var goal in _goals)
            {
                if (goal.Priority == priority)
                    filtered.Add(goal); // add matching goal to result list
            }

            return filtered;

        }
    }

}
        // ---------------------------------------------------
        // SORT METHODS
        // ---------------------------------------------------

/// <summary>
        ///
