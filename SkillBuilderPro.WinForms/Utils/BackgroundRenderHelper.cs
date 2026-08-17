using System.Drawing.Drawing2D;

namespace SkillBuilderPro.WinForms.Utils;

public static class BackgroundRenderHelper
{
    public static Rectangle AspectFit(Size imageSize, Rectangle clientBounds)
        => Scale(imageSize, clientBounds, fill: false);

    public static Rectangle AspectFill(Size imageSize, Rectangle clientBounds)
        => Scale(imageSize, clientBounds, fill: true);

    public static Point ProportionalPoint(Rectangle renderedBounds, float x, float y) => new(
        renderedBounds.Left + (int)Math.Round(renderedBounds.Width * x),
        renderedBounds.Top + (int)Math.Round(renderedBounds.Height * y));

    public static Rectangle ProportionalBounds(Rectangle renderedBounds, float x, float y, float width, float height)
        => new(ProportionalPoint(renderedBounds, x, y), new Size(
            (int)Math.Round(renderedBounds.Width * width),
            (int)Math.Round(renderedBounds.Height * height)));

    public static void DrawAspectFill(Graphics graphics, Image image, Rectangle clientBounds)
    {
        Rectangle destination = AspectFill(image.Size, clientBounds);
        GraphicsState state = graphics.Save();
        graphics.SetClip(clientBounds);
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
        graphics.DrawImage(image, destination);
        graphics.Restore(state);
    }

    private static Rectangle Scale(Size imageSize, Rectangle clientBounds, bool fill)
    {
        if (imageSize.Width <= 0 || imageSize.Height <= 0 || clientBounds.Width <= 0 || clientBounds.Height <= 0)
            return Rectangle.Empty;

        double xScale = (double)clientBounds.Width / imageSize.Width;
        double yScale = (double)clientBounds.Height / imageSize.Height;
        double scale = fill ? Math.Max(xScale, yScale) : Math.Min(xScale, yScale);
        int width = (int)Math.Round(imageSize.Width * scale);
        int height = (int)Math.Round(imageSize.Height * scale);
        return new Rectangle(
            clientBounds.Left + (clientBounds.Width - width) / 2,
            clientBounds.Top + (clientBounds.Height - height) / 2,
            width,
            height);
    }
}
