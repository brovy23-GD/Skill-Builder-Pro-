using SkillBuilderPro.WinForms.Theming;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

public class RoundedTabControl : TabControl
{
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string Sport { get; set; } = "Basketball";

    
    public RoundedTabControl()
    {
        DrawMode = TabDrawMode.OwnerDrawFixed;
        ItemSize = new Size(120, 40);
        SizeMode = TabSizeMode.Fixed;
    }

    protected override void OnDrawItem(DrawItemEventArgs e)
    {
        var theme = TeamThemes.GetThemeForSport(Sport);

        // get the current tab page

        // Selected tab
        bool selected = (e.Index == this.SelectedIndex);

        // Tab bounds
        Rectangle tabRect = e.Bounds;
        tabRect.Inflate(-4, -4);

        // Background
        using (Brush backBrush = new SolidBrush(selected ? theme.Panel : theme.Base))
            e.Graphics.FillRoundedRectangle(backBrush, tabRect, 12);

        // Accent underline for selected tab
        if (selected)
        {
            Rectangle underline = new Rectangle(tabRect.X, tabRect.Bottom - 4, tabRect.Width, 4);
            using (Brush accentBrush = new SolidBrush(theme.Accent))
                e.Graphics.FillRectangle(accentBrush, underline);
        }

        // Text
        string text = this.TabPages[e.Index].Text;
        using (Brush textBrush = new SolidBrush(theme.Text))
        {
            StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            e.Graphics.DrawString(text, this.Font, textBrush, tabRect, sf);
        }
    }
}

public static class GraphicsExtensions
{
    public static void FillRoundedRectangle(this Graphics g, Brush brush, Rectangle rect, int radius)
    {
        using (GraphicsPath path = RoundedRect(rect, radius))
            g.FillPath(brush, path);
    }

    private static GraphicsPath RoundedRect(Rectangle rect, int radius)
    {
        GraphicsPath path = new GraphicsPath();
        int diameter = radius * 2;

        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();

        return path;
    }
}


