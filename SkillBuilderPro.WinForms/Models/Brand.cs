using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace SkillBuilderPro.WinForms.Models
{
    /// <summary>Single source of truth for app colors, type, and image treatment.</summary>
    public static class Brand
    {
        // Core surfaces
        public static readonly Color Base = Color.FromArgb(18, 22, 30);
        public static readonly Color Panel = Color.FromArgb(30, 35, 46);
        public static readonly Color Card = Color.FromArgb(38, 44, 58);
        public static readonly Color Raised = Color.FromArgb(52, 60, 76);
        public static readonly Color Line = Color.FromArgb(64, 74, 92);

        // Accents — blue family only
        public static readonly Color Blue = Color.FromArgb(0, 120, 215);
        public static readonly Color BlueLit = Color.FromArgb(38, 152, 240);
        public static readonly Color BlueDeep = Color.FromArgb(14, 74, 134);
        public static readonly Color Steel = Color.FromArgb(46, 108, 176);
        public static readonly Color Graphite = Color.FromArgb(62, 70, 86);

        // Text
        public static readonly Color Text = Color.FromArgb(232, 237, 243);
        public static readonly Color Muted = Color.FromArgb(150, 164, 184);
        public static readonly Color TextStrong = Color.FromArgb(248, 250, 253);
        public static readonly Color TextCell = Color.FromArgb(226, 233, 242);

        // Type
        public static readonly Font H1 = new Font("Segoe UI", 18F, FontStyle.Bold);
        public static readonly Font H2 = new Font("Segoe UI", 14F, FontStyle.Bold);
        public static readonly Font Body = new Font("Segoe UI", 10F);
        public static readonly Font Meta = new Font("Segoe UI", 9.5F);
        public static readonly Font Btn = new Font("Segoe UI Semibold", 9.5F);
        public static readonly Font RowName = new Font("Segoe UI", 10F, FontStyle.Bold);
        public static readonly Font RowCell = new Font("Segoe UI", 9.5F, FontStyle.Bold);

        public static Color RoleColor(string role)
        {
            switch (role)
            {
                case "Coach": return BlueDeep;
                case "Parent": return Steel;
                case "Admin": return Graphite;
                default: return Blue;   // Athlete
            }
        }

        public static string EnterText(string role)
        {
            switch (role)
            {
                case "Coach": return "ENTER COACH OFFICE";
                case "Parent": return "ENTER PARENT PORTAL";
                case "Admin": return "ENTER ADMIN CONSOLE";
                default: return "ENTER LOCKER ROOM";
            }
        }

        /// <summary>Vivid background: boosts saturation, then lightly darkens.</summary>
        public static Image Hero(Image source, float darken = 0.22f, float saturation = 1.45f)
        {
            if (source == null) return null;
            if (darken > 1f) darken /= 100f;
            darken = Math.Max(0f, Math.Min(1f, darken));

            const float lumR = 0.3086f, lumG = 0.6094f, lumB = 0.0820f;
            float s = saturation;

            ColorMatrix matrix = new ColorMatrix(new float[][]
            {
                new float[] { lumR * (1 - s) + s, lumR * (1 - s),     lumR * (1 - s),     0, 0 },
                new float[] { lumG * (1 - s),     lumG * (1 - s) + s, lumG * (1 - s),     0, 0 },
                new float[] { lumB * (1 - s),     lumB * (1 - s),     lumB * (1 - s) + s, 0, 0 },
                new float[] { 0, 0, 0, 1, 0 },
                new float[] { 0.02f, 0.02f, 0.04f, 0, 1 }
            });

            Bitmap b = new Bitmap(source.Width, source.Height);
            using (Graphics g = Graphics.FromImage(b))
            using (ImageAttributes attrs = new ImageAttributes())
            {
                attrs.SetColorMatrix(matrix);
                g.DrawImage(source,
                    new Rectangle(0, 0, source.Width, source.Height),
                    0, 0, source.Width, source.Height,
                    GraphicsUnit.Pixel, attrs);

                using (SolidBrush overlay = new SolidBrush(
                    Color.FromArgb((int)(darken * 255), 0, 0, 0)))
                    g.FillRectangle(overlay, 0, 0, source.Width, source.Height);
            }
            return b;
        }

        /// <summary>Flat dark overlay. Prefer Hero for backgrounds.</summary>
        public static Image Darken(Image source, float opacity)
        {
            if (source == null) return null;
            if (opacity > 1f) opacity /= 100f;
            opacity = Math.Max(0f, Math.Min(1f, opacity));

            Bitmap b = new Bitmap(source.Width, source.Height);
            using (Graphics g = Graphics.FromImage(b))
            using (SolidBrush overlay = new SolidBrush(
                Color.FromArgb((int)(opacity * 255), 0, 0, 0)))
            {
                g.DrawImage(source, 0, 0, source.Width, source.Height);
                g.FillRectangle(overlay, 0, 0, source.Width, source.Height);
            }
            return b;
        }
    }
}