using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkillBuilderPro.Core.Data;
using SkillBuilderPro.Core.Models;

namespace SkillBuilderPro.API.Data;

// LEGACY / DO NOT RUN: this historical seeder merges multiple sources and is not
// idempotent under the current import contract. It has no startup caller. Use the
// explicit `import-drills` command and DrillImportService instead.
public class DrillExcelSeeder
{
    private readonly AppDbContext _dbContext;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<DrillExcelSeeder> _logger;

    public DrillExcelSeeder(
        AppDbContext dbContext,
        IWebHostEnvironment environment,
        ILogger<DrillExcelSeeder> logger)
    {
        _dbContext = dbContext;
        _environment = environment;
        _logger = logger;
    }

    /// <summary>
    /// Merges your 28 hardcoded drills, your new massive 900 JSON drills library, and 60 fallback drills into one transaction.
    /// </summary>
    public async Task SeedAsync()
    {
        try
        {
            // STEP A: Ensure schema alignment
            try
            {
                await _dbContext.Database.ExecuteSqlRawAsync("ALTER TABLE Drills ALTER COLUMN [Description] NVARCHAR(MAX) NULL;");
                await _dbContext.Database.ExecuteSqlRawAsync("ALTER TABLE Drills ALTER COLUMN [VideoUrl] NVARCHAR(MAX) NULL;");
                await _dbContext.Database.ExecuteSqlRawAsync("DROP INDEX IF EXISTS IX_Drills_Name ON Drills;");
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Schema alignment pass completed with minor notification constraints: {Msg}", ex.Message);
            }

            // STEP B: Clear out older temporary trial entries completely
            await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM Drills;");

            // SAFETY GUARD TEMPORARILY DISABLED FOR THIS 900-DRILL PASS
            /*
            if (await _dbContext.Set<Drill>().AnyAsync())
            {
                _logger.LogInformation("Drills table already populated. Skipping file seed loop.");
                return;
            }
            */

            var masterIngestionList = new List<Drill>();
            var seenNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // POOL 1: Hardcoded drills
            var hardcodedPool = AppDbContext.GetHardcodedDrills();
            foreach (var drill in hardcodedPool)
            {
                ProcessAndPoolDrill(drill, masterIngestionList, seenNames);
            }

            // POOL 2: JSON drills (900 drills)
            string jsonPath = Path.Combine(_environment.ContentRootPath, "Resources", "drills_seed.json");

            if (File.Exists(jsonPath))
            {
                _logger.LogInformation("Located upgraded dataset container. Launching ingestion of your new 900 drills...");

                string rawTextString = await File.ReadAllTextAsync(jsonPath);
                var serializeOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var jsonPool = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(rawTextString, serializeOptions);
                if (jsonPool != null)
                {
                    foreach (var item in jsonPool)
                    {
                        string nameVal = GetValue(item, "Name", "Video Title");
                        if (string.IsNullOrWhiteSpace(nameVal) || nameVal.Equals("Video Title", StringComparison.OrdinalIgnoreCase))
                            continue;

                        string sportVal = GetValue(item, "Sport", "sport");
                        if (sportVal.Equals("Sport", StringComparison.OrdinalIgnoreCase))
                            continue;

                        string categoryVal = GetValue(item, "Category", "category");
                        string subCatVal = GetValue(item, "SubCategory", "Subcategory");
                        string videoVal = GetValue(item, "VideoUrl", "YouTube URL");
                        string descVal = GetValue(item, "Description", "Selection Note");

                        int difficultyValue = 3;
                        string rawDifficulty = GetValue(item, "Difficulty", "DifficultyLevel");
                        if (string.IsNullOrWhiteSpace(rawDifficulty))
                            rawDifficulty = GetValue(item, "Rank", "E");
                        if (int.TryParse(rawDifficulty, out int parsedDiff))
                            difficultyValue = parsedDiff;

                        var jsonDrill = new Drill
                        {
                            Sport = sportVal,
                            Category = categoryVal,
                            SubCategory = subCatVal,
                            Difficulty = difficultyValue,
                            Name = nameVal,
                            VideoUrl = videoVal,
                            Description = descVal
                        };

                        ProcessAndPoolDrill(jsonDrill, masterIngestionList, seenNames);
                    }
                }
            }
            else
            {
                _logger.LogError("ERROR: 'drills_seed.json' was not found inside your API Resources folder. Checked location: {Path}", jsonPath);
            }

            // POOL 3: Dummy drills (60)
            string[] fallbackSports = { "Basketball", "Football", "Hockey" };
            string[] fallbackCategories = { "Defense", "Offense", "Agility", "Conditioning" };

            for (int i = 1; i <= 60; i++)
            {
                string currentSport = fallbackSports[(i % fallbackSports.Length)];
                string currentCategory = fallbackCategories[(i % fallbackCategories.Length)];

                var dummyDrill = new Drill
                {
                    Sport = currentSport,
                    Category = currentCategory,
                    SubCategory = "System Integration Testing",
                    Difficulty = (i % 4) + 2,
                    Duration = $"{10 + (i % 3) * 5}:00",
                    Name = $"System Verification Drill #{i} ({currentSport})",
                    VideoUrl = "https://youtube.com",
                    Description = $"Automated systemic validation tracker placeholder row container for row item #{i}."
                };

                ProcessAndPoolDrill(dummyDrill, masterIngestionList, seenNames);
            }

            // STEP D: Commit to SQL Server
            if (masterIngestionList.Count > 0)
            {
                await _dbContext.Set<Drill>().AddRangeAsync(masterIngestionList);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("SUCCESS: Unified data pipeline synced exactly {Count} drills!", masterIngestionList.Count);
            }
            else
            {
                _logger.LogWarning("Pipeline ended safely but zero tracking entries loaded.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "A fatal transaction processing pipeline interruption occurred.");
        }
    }

    private void ProcessAndPoolDrill(Drill drill, List<Drill> masterList, HashSet<string> seenNames)
    {
        if (drill == null || string.IsNullOrWhiteSpace(drill.Name))
            return;

        string cleanName = drill.Name.Trim();
        if (seenNames.Contains(cleanName))
            return;

        drill.Name = cleanName;
        drill.DateCreated = DateTime.UtcNow;

        if (drill.Difficulty == null || drill.Difficulty <= 0)
            drill.Difficulty = 3;

        if (string.IsNullOrWhiteSpace(drill.Duration))
            drill.Duration = "15:00";

        if (string.IsNullOrWhiteSpace(drill.Sport))
            drill.Sport = "General Athletics";

        if (string.IsNullOrWhiteSpace(drill.Category))
            drill.Category = "General Training";

        if (string.IsNullOrWhiteSpace(drill.SubCategory))
            drill.SubCategory = "Practice Units";

        if (string.IsNullOrWhiteSpace(drill.Description))
            drill.Description = $"Instruction set for training asset: {cleanName}.";

        if (!string.IsNullOrWhiteSpace(drill.VideoUrl))
            drill.VideoUrl = ConvertToEmbedUrl(drill.VideoUrl);

        seenNames.Add(cleanName);
        masterList.Add(drill);
    }

    private string GetValue(Dictionary<string, object> dictionary, params string[] keys)
    {
        foreach (var key in keys)
        {
            var matchedKey = dictionary.Keys.FirstOrDefault(k => k.Equals(key, StringComparison.OrdinalIgnoreCase));
            if (matchedKey != null && dictionary[matchedKey] != null)
            {
                return dictionary[matchedKey]?.ToString() ?? string.Empty;
            }
        }
        return string.Empty;
    }

    private string ConvertToEmbedUrl(string youtubeUrl)
    {
        if (string.IsNullOrWhiteSpace(youtubeUrl))
            return string.Empty;

        youtubeUrl = youtubeUrl.Trim();

        try
        {
            string videoId = string.Empty;

            if (youtubeUrl.Contains("v=", StringComparison.OrdinalIgnoreCase))
            {
                int index = youtubeUrl.IndexOf("v=", StringComparison.OrdinalIgnoreCase);
                if (index != -1)
                {
                    videoId = youtubeUrl.Substring(index + 2);
                    int ampIdx = videoId.IndexOf('&');
                    if (ampIdx != -1)
                        videoId = videoId.Substring(0, ampIdx);
                }
            }
            else if (youtubeUrl.Contains("youtu.be/", StringComparison.OrdinalIgnoreCase))
            {
                int index = youtubeUrl.IndexOf("youtu.be/", StringComparison.OrdinalIgnoreCase);
                if (index != -1)
                {
                    videoId = youtubeUrl.Substring(index + 9);
                    int qMarkIdx = videoId.IndexOf('?');
                    if (qMarkIdx != -1)
                        videoId = videoId.Substring(0, qMarkIdx);
                }
            }
            else if (youtubeUrl.Contains("shorts/", StringComparison.OrdinalIgnoreCase))
            {
                int index = youtubeUrl.IndexOf("shorts/", StringComparison.OrdinalIgnoreCase);
                if (index != -1)
                {
                    videoId = youtubeUrl.Substring(index + 7);
                    int qMarkIdx = videoId.IndexOf('?');
                    if (qMarkIdx != -1)
                        videoId = videoId.Substring(0, qMarkIdx);
                }
            }
            else if (youtubeUrl.Contains("embed/", StringComparison.OrdinalIgnoreCase))
            {
                int index = youtubeUrl.IndexOf("embed/", StringComparison.OrdinalIgnoreCase);
                if (index != -1)
                    videoId = youtubeUrl.Substring(index + 6);
            }

            if (!string.IsNullOrWhiteSpace(videoId))
                videoId = videoId.Trim().Replace("/", "");

            if (!string.IsNullOrWhiteSpace(videoId) && videoId.Length == 11)
            {
                return $"https://www.youtube-nocookie.com/embed/{videoId}";
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Failed to extract clean YouTube ID from URL string {URL}: {Error}", youtubeUrl, ex.Message);
        }

        return youtubeUrl;
    }
}
