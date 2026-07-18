using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SkillBuilderPro.WinForms
{
    /// <summary>A translucent row/card that paints its own fill, stripe, and text.</summary>
    public class GlassRow : Panel
    {
        public class Cell
        {
            public string Text = "";
            public int X;
            public int Width;
            public int Y = 0;      // 0 = vertically centered
            public int H = 0;      // 0 = full height
            public Font Font = Brand.Meta;
            public Color Color = Brand.Muted;
            public ContentAlignment Align = ContentAlignment.MiddleLeft;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Alpha { get; set; } = 150;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int HoverAlpha { get; set; } = 210;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color Fill { get; set; } = Brand.Card;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color HoverFill { get; set; } = Brand.Blue;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color Stripe { get; set; } = Color.Empty;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int StripeWidth { get; set; } = 3;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<Cell> Cells { get; } = new List<Cell>();

        private bool _hot;

        public GlassRow()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor
                   | ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.UserPaint, true);
            BackColor = Color.Transparent;
        }

        public void SetHot(bool on)
        {
            if (_hot == on) return;
            _hot = on;
            Invalidate();
        }

        protected override void OnMouseEnter(System.EventArgs e)
        {
            base.OnMouseEnter(e);
            SetHot(true);
        }

        protected override void OnMouseLeave(System.EventArgs e)
        {
            base.OnMouseLeave(e);
            if (!ClientRectangle.Contains(PointToClient(MousePosition)))
                SetHot(false);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            Color baseColor = _hot ? HoverFill : Fill;
            int a = _hot ? HoverAlpha : Alpha;

            using (SolidBrush fill = new SolidBrush(
                Color.FromArgb(a, baseColor.R, baseColor.G, baseColor.B)))
                g.FillRectangle(fill, ClientRectangle);

            if (Stripe != Color.Empty)
                using (SolidBrush s = new SolidBrush(Stripe))
                    g.FillRectangle(s, 0, 0, StripeWidth, Height);

            foreach (Cell c in Cells)
            {
                RectangleF rect = c.H > 0
                    ? new RectangleF(c.X + 8, c.Y, c.Width - 12, c.H)
                    : new RectangleF(c.X + 8, 0, c.Width - 12, Height);

                using (StringFormat sf = new StringFormat
                {
                    LineAlignment = StringAlignment.Center,
                    Alignment = c.Align == ContentAlignment.MiddleCenter
                        ? StringAlignment.Center
                        : StringAlignment.Near,
                    Trimming = StringTrimming.EllipsisCharacter,
                    FormatFlags = StringFormatFlags.NoWrap
                })
                {
                    using (SolidBrush shadow = new SolidBrush(Color.FromArgb(160, 0, 0, 0)))
                        g.DrawString(c.Text, c.Font, shadow,
                            new RectangleF(rect.X + 1, rect.Y + 1, rect.Width, rect.Height), sf);

                    using (SolidBrush b = new SolidBrush(c.Color))
                        g.DrawString(c.Text, c.Font, b, rect, sf);
                }
            }
        }
    }
}