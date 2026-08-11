// Location: SkillBuilderPro.WinForms/Services/DrillService.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SkillBuilderPro.Core.Interfaces; // 🟢 Implements your elite shared contract
using SkillBuilderPro.Core.Models;     // 🟢 Uses the verified global domain Drill model
using SkillBuilderPro.WinForms.Algorithms;

namespace SkillBuilderPro.WinForms.Services;

/// <summary>
/// WinForms Presentation Desktop Service Adapter.
/// Satisfies IDrillService while managing legacy text-file storage and custom sorting algorithms.
/// </summary>
public class DrillService : IDrillService
{
    private readonly List<Drill> _drills;
    private const string DATA_FOLDER = "data";
    private const string DRILLS_FILE = "data/drills.txt";

    public DrillService()
    {
        _drills = new List<Drill>();
        CreateDataFolder();
        LoadDrills();
    }

    private void CreateDataFolder()
    {
        if (!Directory.Exists(DATA_FOLDER))
        {
            Directory.CreateDirectory(DATA_FOLDER);
        }
    }

    public void LoadDrills()
    {
        _drills.Clear();
        if (!File.Exists(DRILLS_FILE)) return;

        try
        {
            var lines = File.ReadAllLines(DRILLS_FILE);
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var drill = ParseDrill(line);
                if (drill != null)
                {
                    _drills.Add(drill);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading drills: {ex.Message}");
        }
    }

    public void SaveDrills()
    {
        try
        {
            var lines = _drills.Select(SerializeDrill).ToArray();
            File.WriteAllLines(DRILLS_FILE, lines);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving drills: {ex.Message}");
        }
    }

    // ==================================================================
    // 🟢 IDRILLSERVICE INTERFACE IMPLEMENTATIONS (CLEARS COMPILER ERRORS)
    // ==================================================================

    /// <summary>
    /// Implements the two-parameter async interface contract.
    /// </summary>
    public async Task<IEnumerable<Drill>> GetAllAsync(string? sport, string? category)
    {
        IEnumerable<Drill> query = _drills;

        if (!string.IsNullOrWhiteSpace(sport))
            query = query.Where(d => d.Sport.Equals(sport, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(d => d.Category.Equals(category, StringComparison.OrdinalIgnoreCase));

        return await Task.FromResult(query.ToList().AsEnumerable());
    }

    /// <summary>
    /// Implements the single-parameter async interface contract precisely.
    /// Materializes the return query directly into a concrete List to satisfy explicit type binding.
    /// </summary>
    public async Task<List<Drill>> GetAllAsync(string? sport)
    {
        IEnumerable<Drill> query = _drills;

        if (!string.IsNullOrWhiteSpace(sport))
            query = query.Where(d => d.Sport.Equals(sport, StringComparison.OrdinalIgnoreCase));

        return await Task.FromResult(query.ToList());
    }

    public async Task<Drill?> GetByIdAsync(int id)
    {
        var drill = _drills.FirstOrDefault(d => d.Id == id);
        return await Task.FromResult(drill);
    }

    /// <summary>
    /// Asynchronously submits a new athletic drill instance to local text storage.
    /// </summary>
    public async Task<Drill> CreateAsync(Drill drill)
    {
        if (drill == null)
        {
            throw new ArgumentNullException(nameof(drill), "Drill data package cannot be null.");
        }

        if (drill.Id == 0)
        {
            drill.Id = _drills.Any() ? _drills.Max(d => d.Id) + 1 : 1;
        }

        _drills.Add(drill);
        SaveDrills();
        return await Task.FromResult(drill);
    }

    public async Task UpdateAsync(int id, Drill drill)
    {
        if (drill == null) throw new ArgumentNullException(nameof(drill));

        var existing = _drills.FirstOrDefault(d => d.Id == id);
        if (existing != null)
        {
            var index = _drills.IndexOf(existing);
            drill.Id = id;
            _drills[index] = drill;
            SaveDrills();
        }
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var drill = _drills.FirstOrDefault(d => d.Id == id);
        if (drill != null)
        {
            _drills.Remove(drill);
            SaveDrills();
        }
        await Task.CompletedTask;
    }

    public async Task<IEnumerable<Drill>> GetDrillRangeAsync(int startId, int endId)
    {
        var range = _drills.Where(d => d.Id >= startId && d.Id <= endId).ToList();
        return await Task.FromResult(range.AsEnumerable());
    }

    // ==================================================================
    // 🔵 UTILITIES & CUSTOM ALGORITHMS MAPPINGS
    // ==================================================================

    public List<Drill> GetAllDrills() => new(_drills);

    public int GetDrillCount() => _drills.Count;

    public List<Drill> SortByDifficulty(List<Drill> drillsToSort, bool ascending = true)
    {

        var sorted = new List<Drill>(drillsToSort);

        sorted.Sort((d1, d2) =>
        {
            var duration1 = GetDurationSeconds(d1.Duration);
            var duration2 = GetDurationSeconds(d2.Duration);

            return ascending
                ? duration1.CompareTo(duration2)
                : duration2.CompareTo(duration1);
        });

        return sorted;
    }

public List<Drill> SearchByName(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return new List<Drill>(_drills);

        return _drills
            .Where(d => d.Name.Contains(
                searchTerm,
                StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    private static int GetDurationSeconds(string? duration)
    {
        if (string.IsNullOrWhiteSpace(duration))
            return 0;

        var parts = duration.Split(':');

        if (parts.Length == 2 &&
            int.TryParse(parts[0], out var minutes) &&
            int.TryParse(parts[1], out var seconds))
        {
            return (minutes * 60) + seconds;
        }

        if (parts.Length == 3 &&
            int.TryParse(parts[0], out var hours) &&
            int.TryParse(parts[1], out var mins) &&
            int.TryParse(parts[2], out var secs))
        {
            return (hours * 3600) + (mins * 60) + secs;
        }

        return 0;
    }

    private string SerializeDrill(Drill d)
    {
        return $"{d.Id}|{d.Name}|{d.Sport}|{d.Category}|{d.DrillGroup}|{d.SubCategory}|{d.Duration}|{d.Difficulty}|{d.Description}|{d.VideoUrl}|{d.DateCreated:O}";
    }

    private Drill? ParseDrill(string line)
    {
        try
        {
            var parts = line.Split('|');

            // New 4-level format
            if (parts.Length >= 11)
            {
                return new Drill
                {
                    Id = int.Parse(parts[0]),
                    Name = parts[1],
                    Sport = parts[2],
                    Category = parts[3],
                    DrillGroup = parts[4],
                    SubCategory = parts[5],
                    Duration = parts[6],
                    Difficulty = int.TryParse(parts[7], out var difficulty)
                        ? difficulty
                        : null,
                    Description = parts[8],
                    VideoUrl = parts[9],
                    DateCreated = DateTime.TryParse(parts[10], out var dateCreated)
                        ? dateCreated
                        : null
                };
            }

            // Backward compatibility with old saved drills
            if (parts.Length >= 10)
            {
                return new Drill
                {
                    Id = int.Parse(parts[0]),
                    Name = parts[1],
                    Sport = parts[2],
                    Category = parts[3],
                    DrillGroup = null,
                    SubCategory = parts[4],
                    Duration = parts[5],
                    Difficulty = int.TryParse(parts[6], out var difficulty)
                        ? difficulty
                        : null,
                    Description = parts[7],
                    VideoUrl = parts[8],
                    DateCreated = DateTime.TryParse(parts[9], out var dateCreated)
                        ? dateCreated
                        : null
                };
            }

            return null;
        }
        catch
        {
            return null;
        }
    }
}
