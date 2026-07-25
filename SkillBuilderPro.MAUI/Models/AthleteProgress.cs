namespace SkillBuilderPro.MAUI.Models;

public class AthleteProgress
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int DrillId { get; set; }
    public DateTime CompletedDate { get; set; }
    public int RepetitionsCompleted { get; set; }
}