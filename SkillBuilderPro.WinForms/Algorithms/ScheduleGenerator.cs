using System;
using System.Collections.Generic;
using System.Linq;
using SkillBuilderPro.WinForms.Models;

namespace SkillBuilderPro.WinForms.Algorithms
{
    /// <summary>
    /// ScheduleGenerator.cs - Generates customized training schedules for athletes
    ///
    /// WHAT IT DOES:
    /// - Creates a personalized weekly training plan based on the athlete's sport and level
    /// - Selects appropriate drills that fit within the athlete's available time
    /// - Gradually increases difficulty as the athlete progresses over weeks
    ///
    /// HOW IT WORKS:
    /// - Filter drills to match the athlete's sport and target area
    /// - Calculate how many days per week to train based on available hours
    /// - Use a greedy algorithm to select the best drills within the time budget
    /// - Sort drills by the athlete's experience level for the best fit
    ///
    /// ALGORITHMS USED:
    /// - Greedy Algorithm  - picks the best drill at each step (fast and practical)
    /// - QuickSort         - sorts drills by how well they match the user
    ///
    /// TIME COMPLEXITY:
    /// - GenerateWeeklySchedule:        O(n log n) sorting + O(d x m) selection
    /// - GenerateDailySchedule:         O(n log n) sorting
    /// - GenerateProgressionSchedule:   O(w x s x n)
    /// - SelectOptimalDrills:           O(n log n) sorting
    /// - CalculateScheduleEffectiveness: O(m) where m = drills in session
    ///
    /// SPACE COMPLEXITY:
    /// - O(n) for filtered drills list
    /// - O(d x m) for output schedule
    /// </summary>
    public static class ScheduleGenerator
    {
        // ============================================================
        // EXPERIENCE LEVEL MAPPING (string → numeric difficulty)
        // ============================================================
        private static int GetExperienceDifficulty(string level)
        {
            return level switch
            {
                "Beginner" => 1,
                "Intermediate" => 2,
                "Advanced" => 3,
                _ => 2
            };
        }

        // ============================================================
        // WEEKLY SCHEDULE
        // ============================================================
        public static List<TrainingSession> GenerateWeeklySchedule(
            User user,
            List<Drill> availableDrills,
            int hoursPerWeek)
        {
            var schedule = new List<TrainingSession>();

            var relevantDrills = FilterDrillsForUser(availableDrills, user);

            if (relevantDrills.Count == 0)
            {
                Console.WriteLine("Warning: No drills found for your sport and target area.");
                return schedule;
            }

            int trainingDays = CalculateTrainingDays(hoursPerWeek);
            int minutesPerDay = (hoursPerWeek * 60) / trainingDays;

            for (int day = 0; day < trainingDays; day++)
            {
                var session = new TrainingSession
                {
                    Name = $"Training Session - Day {day + 1}",
                    Date = DateTime.Now.AddDays(day),
                    Drills = new List<Drill>()
                };

                var selectedDrills = SelectOptimalDrills(relevantDrills, user, minutesPerDay);
                session.Drills.AddRange(selectedDrills);

                if (session.Drills.Count > 0)
                    schedule.Add(session);
            }

            return schedule;
        }

        // ============================================================
        // DAILY SCHEDULE
        // ============================================================
        public static TrainingSession GenerateDailySchedule(
            User user,
            List<Drill> availableDrills,
            int minutesAvailable)
        {
            var session = new TrainingSession
            {
                Name = $"Today's Training - {DateTime.Now:MM/dd/yyyy}",
                Date = DateTime.Now,
                Drills = new List<Drill>()
            };

            var relevantDrills = FilterDrillsForUser(availableDrills, user);

            if (relevantDrills.Count == 0)
            {
                Console.WriteLine("Warning: No drills found for your sport and target area.");
                return session;
            }

            var selectedDrills = SelectOptimalDrills(relevantDrills, user, minutesAvailable);
            session.Drills.AddRange(selectedDrills);

            return session;
        }

        // ============================================================
        // PROGRESSION SCHEDULE
        // ============================================================
        public static List<TrainingSession> GenerateProgressionSchedule(
            User user,
            List<Drill> availableDrills,
            int weeksToProgress)
        {
            var schedule = new List<TrainingSession>();

            var baseSchedule = GenerateWeeklySchedule(user, availableDrills, 5);

            for (int week = 0; week < weeksToProgress; week++)
            {
                foreach (var baseSession in baseSchedule)
                {
                    var progressedSession = new TrainingSession
                    {
                        Name = $"{baseSession.Name} - Week {week + 1}",
                        Date = DateTime.Now.AddDays(week * 7 + (baseSession.Date - DateTime.Now).Days),
                        Drills = new List<Drill>(baseSession.Drills)
                    };

                    if (week >= 2 && week % 2 == 0)
                    {
                        progressedSession.Drills = IncreaseSessionDifficulty(
                            progressedSession.Drills,
                            availableDrills);
                    }

                    schedule.Add(progressedSession);
                }
            }

            return schedule;
        }

        // ============================================================
        // EFFECTIVENESS SCORE
        // ============================================================
        public static int CalculateScheduleEffectiveness(TrainingSession session, User user)
        {
            if (session.Drills.Count == 0)
                return 0;

            int score = 50;

            var uniqueCategories = session.Drills
                .Select(d => d.SkillCategory)
                .Distinct()
                .Count();

            score += uniqueCategories * 10;

            double avgDifficulty = session.Drills.Average(d => d.Difficulty);
            int userDifficulty = GetExperienceDifficulty(user.ExperienceLevel);

            if (Math.Abs(avgDifficulty - userDifficulty) <= 1)
                score += 20;

            int totalDuration = session.Drills.Sum(d => d.Duration);

            if (totalDuration >= 30 && totalDuration <= 90)
                score += 15;

            return Math.Min(score, 100);
        }

        // ============================================================
        // FILTER DRILLS
        // ============================================================
        private static List<Drill> FilterDrillsForUser(List<Drill> drills, User user)
        {
            var filtered = new List<Drill>();

            foreach (var drill in drills)
            {
                if (user.TargetArea.Equals("All-Around", StringComparison.OrdinalIgnoreCase) ||
                    drill.SkillCategory.Equals(user.TargetArea, StringComparison.OrdinalIgnoreCase))
                {
                    filtered.Add(drill);
                }
            }

            return filtered.Count > 0 ? filtered : drills;
        }

        // ============================================================
        // SELECT OPTIMAL DRILLS (GREEDY)
        // ============================================================
        private static List<Drill> SelectOptimalDrills(
            List<Drill> drills,
            User user,
            int minutesAvailable)
        {
            var selected = new List<Drill>();
            int totalMinutes = 0;

            var sortedDrills = SortDrillsByUserFit(drills, user);

            foreach (var drill in sortedDrills)
            {
                if (totalMinutes + drill.Duration <= minutesAvailable)
                {
                    selected.Add(drill);
                    totalMinutes += drill.Duration;
                }
            }

            return selected;
        }

        // ============================================================
        // SORT DRILLS BY USER FIT
        // ============================================================
        private static List<Drill> SortDrillsByUserFit(List<Drill> drills, User user)
        {
            int userDifficulty = GetExperienceDifficulty(user.ExperienceLevel);

            return drills
                .OrderByDescending(d => d.SkillCategory.Equals(user.TargetArea, StringComparison.OrdinalIgnoreCase))
                .ThenBy(d => Math.Abs(d.Difficulty - userDifficulty))
                .ThenBy(d => d.Duration)
                .ToList();
        }

        // ============================================================
        // INCREASE DIFFICULTY
        // ============================================================
        private static List<Drill> IncreaseSessionDifficulty(
            List<Drill> currentDrills,
            List<Drill> allDrills)
        {
            var upgraded = new List<Drill>();

            foreach (var drill in currentDrills)
            {
                var harderOptions = allDrills
                    .Where(d =>
                        d.SkillCategory.Equals(drill.SkillCategory, StringComparison.OrdinalIgnoreCase) &&
                        d.Difficulty > drill.Difficulty)
                    .OrderBy(d => d.Difficulty)
                    .ToList();

                upgraded.Add(harderOptions.Count > 0 ? harderOptions.First() : drill);
            }

            return upgraded;
        }

        // ============================================================
        // TRAINING DAYS
        // ============================================================
        private static int CalculateTrainingDays(int hoursPerWeek)
        {
            if (hoursPerWeek <= 2) return 2;
            if (hoursPerWeek <= 4) return 3;
            if (hoursPerWeek <= 6) return 4;
            if (hoursPerWeek <= 8) return 5;
            return 6;
        }
    }
}

