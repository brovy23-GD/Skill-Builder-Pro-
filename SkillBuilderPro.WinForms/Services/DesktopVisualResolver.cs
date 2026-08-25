namespace SkillBuilderPro.WinForms.Services;

public interface IDesktopVisualResolver
{
    Image GetChooseExperienceBackground();
    Image GetLoginBackground();
    Image GetAthleteHomeBackground();
    Image GetTrainingPageBackground(string? sport);
    Image GetTrainingBuilderBackground(string? sport);
    Image GetDrillLibraryBackground();
    Image GetGoalsBackground();
    Image GetTrophyRoomBackground();
    Image GetLockerRoomBackground();
    Image GetCoachBackground();
    Image GetParentBackground();
    Image GetAdministratorBackground();
    Image GetBrandLogo();
}

public sealed class DesktopVisualResolver : IDesktopVisualResolver
{
    public static DesktopVisualResolver Current { get; } = new();

    private DesktopVisualResolver()
    {
    }

    public Image GetChooseExperienceBackground() => Load("choose_role_desktop.png");

    public Image GetLoginBackground() => Load("login_desktop.png");

    public Image GetAthleteHomeBackground() => Load("home_athlete_desktop.png");

    public Image GetTrainingPageBackground(string? sport) => NormalizeSport(sport) switch
    {
        "basketball" => Load("training_basketball_chicago_desktop.png"),
        "football" => Load("training_football_chicago_desktop.png"),
        "baseball" => Load("training_baseball_chicago_desktop.png"),
        "softball" => Load("training_softball_chicago_desktop.png"),
        "soccer" => Load("training_soccer_chicago_desktop.png"),
        "hockey" => Load("training_hockey_chicago_desktop.png"),
        _ => GetAthleteHomeBackground()
    };

    public Image GetTrainingBuilderBackground(string? sport) => NormalizeSport(sport) switch
    {
        "basketball" => Load("training_builder_basketball_desktop.png"),
        "football" => Load("training_builder_football_desktop.png"),
        "baseball" => Load("training_builder_baseball_desktop.png"),
        "softball" => Load("training_builder_softball_desktop.png"),
        "soccer" => Load("training_builder_soccer_desktop.png"),
        "hockey" => Load("training_builder_hockey_desktop.png"),
        _ => GetAthleteHomeBackground()
    };

    public Image GetDrillLibraryBackground() => Load("drill_library_desktop.png");

    public Image GetGoalsBackground() => Load("goals_desktop.png");

    public Image GetTrophyRoomBackground() => Load("trophy_desktop.png");

    public Image GetLockerRoomBackground() => Load("profile_desktop.png");

    public Image GetCoachBackground() => Load("home_coach_desktop.png");

    public Image GetParentBackground() => Load("home_parent_desktop.png");

    public Image GetAdministratorBackground() => Load("home_administrator_desktop.png");

    public Image GetBrandLogo() => Load("sb_pro_logo_button_40x40.png");

    private static string NormalizeSport(string? sport) => (sport ?? string.Empty).Trim().ToLowerInvariant();

    private static Image Load(string fileName)
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Resources", fileName);
        if (!File.Exists(path))
            throw new FileNotFoundException($"Required desktop visual is missing: {fileName}", path);

        using Image source = Image.FromFile(path);
        return new Bitmap(source);
    }
}
