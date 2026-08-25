namespace SkillBuilderPro.MAUI.Services;

public static class ResponsiveLayout
{
    public const double PhoneMaxWidth = 700;
    public const double CompactPhoneMaxWidth = 430;
    public const double PhoneHorizontalPadding = 16;
    public const double PhoneTopPadding = 12;
    public const double PhoneBottomContentPadding = 104;

    public static bool IsPhone(double width) => width > 0 && width < PhoneMaxWidth;

    public static Thickness PhonePagePadding(double width) => IsPhone(width)
        ? new Thickness(PhoneHorizontalPadding, PhoneTopPadding, PhoneHorizontalPadding, PhoneBottomContentPadding)
        : new Thickness(22);
}
