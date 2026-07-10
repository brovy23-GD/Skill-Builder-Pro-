using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SkillBuilderPro.WinForms.Models;
using SkillBuilderPro.WinForms.Algorithms;

namespace SkillBuilderPro.WinForms.Services
{
    /// <summary>
    /// Service for managing progress logs and tracking improvements
    /// </summary>
    public class ProgressService
    {
        private List<ProgressLog> logs;
        private const string PROGRESS_FILE = "data/progress.txt";

        public ProgressService()
        {
            logs = new List<ProgressLog>();
        }

        /// <summary>
        /// Loads all progress logs from file
        /// </summary>
        public void LoadProgressLogs()
        {
            logs.Clear();

            if (!File.Exists(PROGRESS_FILE))
                return;

            try
            {
                var lines = File.ReadAllLines(PROGRESS_FILE);
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var log = ParseProgressLog(line);
                    if (log != null)
                    {
                        logs.Add(log);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading progress logs: {ex.Message}");
            }
        }

        /// <summary>
        /// Saves all progress logs to file
        /// </summary>
        public void SaveProgressLogs()
        {
            try
            {
                var lines = logs.Select(l => SerializeProgressLog(l)).ToArray();
                File.WriteAllLines(PROGRESS_FILE, lines);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving progress logs: {ex.Message}");
            }
        }

        /// <summary>
        /// Logs a new progress entry
        /// </summary>
        public void LogProgress(ProgressLog log)
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));

            logs.Add(log);
            SaveProgressLogs();
        }

        /// <summary>
        /// Gets all progress logs for a user
        /// </summary>
        public List<ProgressLog> GetUserProgress(Guid userId)
        {
            return logs.Where(l => l.UserId == userId).OrderByDescending(l => l.LogDate).ToList();
        }

        /// <summary>
        /// Gets progress logs for a specific metric
        /// </summary>
        public List<ProgressLog> GetProgressByMetric(Guid userId, string metricType)
        {
            return logs
                .Where(l => l.UserId == userId && l.MetricType.Equals(metricType, StringComparison.OrdinalIgnoreCase))
                .OrderBy(l => l.LogDate)
                .ToList();
        }

        /// <summary>
        /// Gets progress logs for a date range
        /// </summary>
        public List<ProgressLog> GetProgressInDateRange(Guid userId, DateTime startDate, DateTime endDate)
        {
            return logs
                .Where(l => l.UserId == userId && l.LogDate >= startDate && l.LogDate <= endDate)
                .OrderBy(l => l.LogDate)
                .ToList();
        }

        /// <summary>
        /// Gets recent progress (last N days)
        /// </summary>
        public List<ProgressLog> GetRecentProgress(Guid userId, int days)
        {
            var cutoffDate = DateTime.Now.AddDays(-days);
            return GetProgressInDateRange(userId, cutoffDate, DateTime.Now);
        }

        /// <summary>
        /// Gets a specific progress log by ID
        /// </summary>
        public ProgressLog GetProgressLogById(Guid id)
        {
            return logs.FirstOrDefault(l => l.Id == id);
        }

        /// <summary>
        /// Calculates average metric value for user
        /// </summary>
        public double CalculateAverageMetric(Guid userId, string metricType)
        {
            var metricLogs = GetProgressByMetric(userId, metricType);
            if (metricLogs.Count == 0)
                return 0;

            return metricLogs.Average(l => l.MetricValue);
        }

        /// <summary>
        /// Calculates progress trend (improvement rate)
        /// Returns percentage improvement over period
        /// </summary>
        public double CalculateProgressTrend(Guid userId, string metricType, int days)
        {
            var recentLogs = GetRecentProgress(userId, days)
                .Where(l => l.MetricType.Equals(metricType, StringComparison.OrdinalIgnoreCase))
                .OrderBy(l => l.LogDate)
                .ToList();

            if (recentLogs.Count < 2)
                return 0;

            double oldValue = recentLogs.First().MetricValue;
            double newValue = recentLogs.Last().MetricValue;

            if (oldValue == 0)
                return 0;

            return ((newValue - oldValue) / oldValue) * 100;
        }

        /// <summary>
        /// Gets average satisfaction score
        /// </summary>
        public double GetAverageSatisfaction(Guid userId)
        {
            var userLogs = GetUserProgress(userId);
            if (userLogs.Count == 0)
                return 0;

            return userLogs.Average(l => l.SatisfactionScore);
        }

        /// <summary>
        /// Gets average energy level
        /// </summary>
        public double GetAverageEnergyLevel(Guid userId)
        {
            var userLogs = GetUserProgress(userId);
            if (userLogs.Count == 0)
                return 0;

            return userLogs.Average(l => l.EnergyLevel);
        }

        /// <summary>
        /// Gets most improved metric
        /// </summary>
        public string GetMostImprovedMetric(Guid userId, int days)
        {
            var recentLogs = GetRecentProgress(userId, days);
            var metricGroups = recentLogs.GroupBy(l => l.MetricType);

            string bestMetric = null;
            double maxImprovement = 0;

            foreach (var group in metricGroups)
            {
                var logs = group.OrderBy(l => l.LogDate).ToList();
                if (logs.Count >= 2)
                {
                    double trend = Math.Abs(CalculateProgressTrend(userId, group.Key, days));
                    if (trend > maxImprovement)
                    {
                        maxImprovement = trend;
                        bestMetric = group.Key;
                    }
                }
            }

            return bestMetric ?? "N/A";
        }

        /// <summary>
        /// Gets total sessions logged
        /// </summary>
        public int GetTotalSessionsLogged(Guid userId)
        {
            return GetUserProgress(userId).Count;
        }

        /// <summary>
        /// Gets total training minutes
        /// </summary>
        public int GetTotalTrainingMinutes(Guid userId)
        {
            return GetUserProgress(userId).Sum(l => l.SessionDuration);
        }

        /// <summary>
        /// Searches progress logs by note content
        /// </summary>
        public List<ProgressLog> SearchByNotes(Guid userId, string searchTerm)
        {
            return GetUserProgress(userId)
                .Where(l => l.Notes.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// Deletes a progress log
        /// </summary>
        public bool DeleteProgressLog(Guid id)
        {
            var log = GetProgressLogById(id);
            if (log != null)
            {
                logs.Remove(log);
                SaveProgressLogs();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Serializes progress log to pipe-delimited string
        /// </summary>
        private string SerializeProgressLog(ProgressLog log)
        {
            return $"{log.Id}|{log.UserId}|{log.SessionId}|{log.LogDate:O}|{log.MetricType}|{log.MetricValue}|{log.MetricUnit}|{log.SessionDuration}|{log.DrillsCompleted}|{log.DifficultyLevel}|{log.Notes}|{log.SatisfactionScore}|{log.EnergyLevel}|{log.CreatedAt:O}";
        }

        /// <summary>
        /// Deserializes progress log from pipe-delimited string
        /// </summary>
        private ProgressLog ParseProgressLog(string line)
        {
            try
            {
                var parts = line.Split('|');
                if (parts.Length < 14)
                    return null;

                return new ProgressLog
                {
                    Id = Guid.Parse(parts[0]),
                    UserId = Guid.Parse(parts[1]),
                    SessionId = Guid.Parse(parts[2]),
                    LogDate = DateTime.Parse(parts[3]),
                    MetricType = parts[4],
                    MetricValue = double.Parse(parts[5]),
                    MetricUnit = parts[6],
                    SessionDuration = int.Parse(parts[7]),
                    DrillsCompleted = parts[8],
                    DifficultyLevel = int.Parse(parts[9]),
                    Notes = parts[10],
                    SatisfactionScore = int.Parse(parts[11]),
                    EnergyLevel = int.Parse(parts[12]),
                    CreatedAt = DateTime.Parse(parts[13])
                };
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets progress log count
        /// </summary>
        public int GetProgressLogCount()
        {
            return logs.Count;
        }
    }
}

