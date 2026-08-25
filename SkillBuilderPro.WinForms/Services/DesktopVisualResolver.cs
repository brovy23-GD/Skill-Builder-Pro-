using SkillBuilderPro.WinForms.Properties;

namespace SkillBuilderPro.WinForms.Services;

public interface IDesktopVisualResolver
{
    Image GetChooseExperienceBackground();
    Image GetLoginBackground();
    Image GetAthleteHomeBackground();
    Image GetTrainingPageBackground(string? sport);
    Image GetTrainingBuilderBackground(string? sport);
    Image GetGoalsBackground();
    Image GetTrophyRoomBackground();
    Image GetLockerRoomBackground();
    Image GetCoachBackground();
    Image GetParentBackground();
    Image GetAdministratorBackground();
}

public sealed class DesktopVisualResolver : IDesktopVisualResolver
{
    public static DesktopVisualResolver Current { get; } = new();

    private DesktopVisualResolver()
    {
    }

    public Image GetChooseExperienceBackground() => Load("choose_role_desktop.png", Resource1.choose_role_desktop);

    public Image GetLoginBackground() => new Bitmap(Resource1.weight_room);

    public Image GetAthleteHomeBackground() => Load("home_athlete_desktop.png", Resource1.weight_room);

    public Image GetTrainingPageBackground(string? sport) => NormalizeSport(sport) switch
    {
        "basketball" => Load("training_basketball_chicago_desktop.png", Resource1.Chicago_Basketball),
        "football" => Load("training_football_chicago_desktop.png", Resource1.Chicago_Football),
        "baseball" => Load("training_baseball_chicago_desktop.png", Resource1.Chicago_Baseball),
        "softball" => Load("training_softball_chicago_desktop.png", Resource1.softball_field),
        "soccer" => Load("training_soccer_chicago_desktop.png", Resource1.Chicago_Soccer),
        "hockey" => Load("training_hockey_chicago_desktop.png", Resource1.Chicago_Hockey),
        _ => GetAthleteHomeBackground()
    };

    public Image GetTrainingBuilderBackground(string? sport) => NormalizeSport(sport) switch
    {
        "basketball" => Load("training_builder_basketball_desktop.png", Resource1.training_builder_basketball_desktop),
        "football" => Load("training_builder_football_desktop.png", Resource1.training_builder_football_desktop),
        "baseball" => Load("training_builder_baseball_desktop.png", Resource1.training_builder_baseball_desktop),
        "softball" => Load("training_builder_softball_desktop.png", Resource1.training_builder_softball_desktop),
        "soccer" => Load("training_builder_soccer_desktop.png", Resource1.training_builder_soccer_desktop),
        "hockey" => Load("training_builder_hockey_desktop.png", Resource1.training_builder_hockey_desktop),
        _ => GetAthleteHomeBackground()
    };

    public Image GetGoalsBackground() => Load("goals_background_approved.png", Resource1.strength_training);

    public Image GetTrophyRoomBackground() => Load("trophy_room_background_approved.png", Resource1.strength_training);

    public Image GetLockerRoomBackground() => Load("locker_room_background_approved.png", Resource1.LockerRoom);

    public Image GetCoachBackground() => new Bitmap(Resource1.CoachOffice);

    public Image GetParentBackground() => new Bitmap(Resource1.parentsbackground);

    public Image GetAdministratorBackground() => new Bitmap(Resource1.AdminDashApproved);

    private static string NormalizeSport(string? sport) => (sport ?? string.Empty).Trim().ToLowerInvariant();

    private static Image Load(string fileName, Image fallback)
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Resources", fileName);
        if (!File.Exists(path))
            return new Bitmap(fallback);

        using Image source = Image.FromFile(path);
        return new Bitmap(source);
    }
}
