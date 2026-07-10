using System.Drawing;

namespace SkillBuilderPro.WinForms.Theming
{
    public static class TeamThemes
    {
        public struct Theme
        {
            public Color Base;       // neutral background
            public Color Panel;      // panel background
            public Color Accent;     // primary team color
            public Color Accent2;    // secondary team color
            public Color Text;       // text color
        }

        public static Theme GetThemeForSport(string sport)
        {
            var s = (sport ?? "").Trim().ToLower();

            switch (s)
            {
                case "basketball": // Bulls
                    return new Theme
                    {
                        Base = ColorTranslator.FromHtml("#0A0F1E"),
                        Panel = ColorTranslator.FromHtml("#121212"),
                        Accent = ColorTranslator.FromHtml("#CE1141"),
                        Accent2 = ColorTranslator.FromHtml("#000000"),
                        Text = Color.White
                    };

                case "baseball": // White Sox
                    return new Theme
                    {
                        Base = ColorTranslator.FromHtml("#0A0F1E"),
                        Panel = ColorTranslator.FromHtml("#121212"),
                        Accent = ColorTranslator.FromHtml("#27251F"),
                        Accent2 = ColorTranslator.FromHtml("#C4CED4"),
                        Text = Color.White
                    };

                case "softball": // Cubs
                    return new Theme
                    {
                        Base = ColorTranslator.FromHtml("#0A0F1E"),
                        Panel = ColorTranslator.FromHtml("#121212"),
                        Accent = ColorTranslator.FromHtml("#0E3386"),
                        Accent2 = ColorTranslator.FromHtml("#CC3433"),
                        Text = Color.White
                    };

                case "football": // Bears
                    return new Theme
                    {
                        Base = ColorTranslator.FromHtml("#0A0F1E"),
                        Panel = ColorTranslator.FromHtml("#121212"),
                        Accent = ColorTranslator.FromHtml("#0B162A"),
                        Accent2 = ColorTranslator.FromHtml("#E64100"),
                        Text = Color.White
                    };

                case "hockey": // Blackhawks
                    return new Theme
                    {
                        Base = ColorTranslator.FromHtml("#0A0F1E"),
                        Panel = ColorTranslator.FromHtml("#121212"),
                        Accent = ColorTranslator.FromHtml("#CF0A2C"),
                        Accent2 = ColorTranslator.FromHtml("#000000"),
                        Text = Color.White
                    };

                default:
                    return new Theme
                    {
                        Base = ColorTranslator.FromHtml("#0A0F1E"),
                        Panel = ColorTranslator.FromHtml("#121212"),
                        Accent = ColorTranslator.FromHtml("#4A7BA6"),
                        Accent2 = ColorTranslator.FromHtml("#1C1C1C"),
                        Text = Color.White
                    };
            }
        }
    }
}

