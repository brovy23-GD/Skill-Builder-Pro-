using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SkillBuilderPro.Core.Models
{
    public class Drill
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("sport")]
        public string Sport { get; set; } = string.Empty;

        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("videoUrl")]
        public string VideoUrl { get; set; } = string.Empty;

        [JsonPropertyName("difficulty")]
        public int Difficulty { get; set; }

        [JsonPropertyName("schedules")]
        public ICollection<TrainingSchedule> Schedules { get; set; } = new List<TrainingSchedule>();

        [JsonPropertyName("progressLogs")]
        public ICollection<ProgressLog> ProgressLogs { get; set; } = new List<ProgressLog>();
    }
}