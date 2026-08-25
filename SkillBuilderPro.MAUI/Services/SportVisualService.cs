using Microsoft.Maui.Devices;
using System.Diagnostics;

namespace SkillBuilderPro.MAUI.Services;

public enum VisualDeviceClass { Phone, Tablet, Desktop }
public enum VisualOrientation { Portrait, Landscape }

public interface ISportVisualService
{
    IReadOnlyList<string> SupportedSports { get; }
    string GetTrainingPageBackground(string? sport);
    string GetTrainingPageBackground(string? sport, double viewportWidth, double viewportHeight);
    string GetTrainingPageBackground(string? sport, VisualDeviceClass deviceClass, VisualOrientation orientation);
    string GetTrainingBuilderBackground(string? sport);
    string GetTrainingBuilderBackground(string? sport, double viewportWidth, double viewportHeight);
    string GetTrainingBuilderBackground(string? sport, VisualDeviceClass deviceClass, VisualOrientation orientation);
    string GetAthleteHomeBackground(double viewportWidth, double viewportHeight);
    string GetAthleteHomeBackground(VisualDeviceClass deviceClass, VisualOrientation orientation);
    string GetChooseRoleBackground(double viewportWidth, double viewportHeight);
    string GetChooseRoleBackground(VisualDeviceClass deviceClass, VisualOrientation orientation);
}

public sealed class SportVisualService : ISportVisualService
{
    public IReadOnlyList<string> SupportedSports { get; } =
        ["Basketball", "Football", "Baseball", "Softball", "Soccer", "Hockey"];

    public string GetTrainingPageBackground(string? sport)
    {
        var display = DeviceDisplay.Current.MainDisplayInfo;
        return GetTrainingPageBackground(sport, display.Width / display.Density, display.Height / display.Density);
    }

    public string GetTrainingPageBackground(string? sport, double viewportWidth, double viewportHeight)
    {
        var (deviceClass, orientation) = ClassifyViewport(viewportWidth, viewportHeight);
        return GetTrainingPageBackground(sport, deviceClass, orientation);
    }

    public string GetTrainingPageBackground(string? sport, VisualDeviceClass deviceClass, VisualOrientation orientation)
    {
        var normalizedSport = SupportedSports.FirstOrDefault(
            item => string.Equals(item, sport?.Trim(), StringComparison.OrdinalIgnoreCase));
        if (normalizedSport is null)
        {
            Debug.WriteLine($"Training visual fallback used for unsupported sport '{sport ?? "<null>"}'.");
            return GetAthleteHomeBackground(deviceClass, orientation);
        }
        return $"training_{normalizedSport.ToLowerInvariant()}_chicago_{GetVariant(deviceClass, orientation)}.png";
    }

    public string GetTrainingBuilderBackground(string? sport)
    {
        var display = DeviceDisplay.Current.MainDisplayInfo;
        return GetTrainingBuilderBackground(sport, display.Width / display.Density, display.Height / display.Density);
    }

    public string GetTrainingBuilderBackground(string? sport, double viewportWidth, double viewportHeight)
    {
        var (deviceClass, orientation) = ClassifyViewport(viewportWidth, viewportHeight);
        return GetTrainingBuilderBackground(sport, deviceClass, orientation);
    }

    public string GetTrainingBuilderBackground(
        string? sport,
        VisualDeviceClass deviceClass,
        VisualOrientation orientation)
    {
        var normalizedSport = SupportedSports.FirstOrDefault(
            item => string.Equals(item, sport?.Trim(), StringComparison.OrdinalIgnoreCase));
        if (normalizedSport is null)
        {
            Debug.WriteLine($"Training Builder visual fallback used for unsupported sport '{sport ?? "<null>"}'.");
            return GetAthleteHomeBackground(deviceClass, orientation);
        }

        return $"training_builder_{normalizedSport.ToLowerInvariant()}_{GetVariant(deviceClass, orientation)}.png";
    }

    public string GetAthleteHomeBackground(double viewportWidth, double viewportHeight)
    {
        var (deviceClass, orientation) = ClassifyViewport(viewportWidth, viewportHeight);
        return GetAthleteHomeBackground(deviceClass, orientation);
    }

    public string GetAthleteHomeBackground(VisualDeviceClass deviceClass, VisualOrientation orientation) =>
        $"home_athlete_{GetVariant(deviceClass, orientation)}.png";

    public string GetChooseRoleBackground(double viewportWidth, double viewportHeight)
    {
        var (deviceClass, orientation) = ClassifyViewport(viewportWidth, viewportHeight);
        return GetChooseRoleBackground(deviceClass, orientation);
    }

    public string GetChooseRoleBackground(VisualDeviceClass deviceClass, VisualOrientation orientation) =>
        $"choose_role_{GetVariant(deviceClass, orientation)}.png";

    public static (VisualDeviceClass DeviceClass, VisualOrientation Orientation) ClassifyViewport(double width, double height)
    {
        var orientation = width > height ? VisualOrientation.Landscape : VisualOrientation.Portrait;
        if (DeviceInfo.Platform == DevicePlatform.WinUI || DeviceInfo.Idiom == DeviceIdiom.Desktop)
            return (VisualDeviceClass.Desktop, orientation);
        if (DeviceInfo.Idiom == DeviceIdiom.Phone)
            return (VisualDeviceClass.Phone, orientation);
        if (DeviceInfo.Idiom == DeviceIdiom.Tablet)
            return (VisualDeviceClass.Tablet, orientation);
        var shortestSide = Math.Min(width, height);
        return shortestSide > 0 && shortestSide < 700
            ? (VisualDeviceClass.Phone, orientation)
            : (VisualDeviceClass.Tablet, orientation);
    }

    private static string GetVariant(VisualDeviceClass deviceClass, VisualOrientation orientation) =>
        deviceClass switch
        {
            VisualDeviceClass.Desktop => "desktop",
            VisualDeviceClass.Tablet when orientation == VisualOrientation.Portrait => "tablet_portrait",
            VisualDeviceClass.Tablet => "tablet_landscape",
            VisualDeviceClass.Phone when orientation == VisualOrientation.Portrait => "phone_portrait",
            _ => "phone_landscape"
        };
}
