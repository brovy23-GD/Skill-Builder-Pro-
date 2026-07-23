using System;
using System.Drawing;
using System.Windows.Forms;
using SkillBuilderPro.WinForms.Properties;

namespace SkillBuilderPro.WinForms
{
    public partial class RoleSelectForm : Form
    {
        public string SelectedRole { get; private set; }

        public RoleSelectForm()
        {
            InitializeComponent();

            this.Text = "SkillBuilderPro";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(1000, 800);
            this.BackgroundImage = Brand.Hero(Resource1.weight_room, 0.18f, 1.5f);
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.DoubleBuffered = true;

            BuildCard();
        }

        private void BuildCard()
        {
            Panel card = new Panel
            {
                Size = new Size(900, 400),
                BackColor = Brand.Panel
            };

            void CenterCard() => card.Location = new Point(
                (this.ClientSize.Width - card.Width) / 2,
                Math.Max((int)(this.ClientSize.Height * 0.13), 20));
            CenterCard();
            this.Resize += (s, e) => CenterCard();

            card.Controls.Add(new Label
            {
                Text = "SKILL BUILDER PRO",
                Font = new Font("Segoe UI Black", 30F),
                ForeColor = Color.White,
                AutoSize = false,
                Height = 58,
                Width = card.Width,
                Location = new Point(0, 30),
                TextAlign = ContentAlignment.MiddleCenter
            });

            card.Controls.Add(new Label
            {
                Text = "BUILT FOR ATHLETES.  POWERED BY PRECISION.",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Brand.Muted,
                AutoSize = false,
                Height = 22,
                Width = card.Width,
                Location = new Point(0, 90),
                TextAlign = ContentAlignment.MiddleCenter
            });

            card.Controls.Add(new Panel
            {
                Size = new Size(90, 3),
                Location = new Point((card.Width - 90) / 2, 122),
                BackColor = Brand.Blue
            });

            card.Controls.Add(new Label
            {
                Text = "CHOOSE YOUR ROLE",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Brand.Muted,
                AutoSize = false,
                Height = 20,
                Width = card.Width,
                Location = new Point(0, 138),
                TextAlign = ContentAlignment.MiddleCenter
            });

            string[] roles = { "ATHLETE", "COACH", "PARENT", "ADMIN" };
            string[] blurbs =
            {
                "Build drills, track goals,\nown your schedule.",
                "Manage the roster and\nassign training.",
                "Follow your athlete's\nprogress.",
                "Full system and\nuser management."
            };

            const int tileW = 195, tileH = 180, gap = 22;
            int totalW = tileW * 4 + gap * 3;
            int startX = (card.Width - totalW) / 2;

            for (int i = 0; i < roles.Length; i++)
            {
                string proper = roles[i].Substring(0, 1) + roles[i].Substring(1).ToLower();
                Color roleColor = Brand.RoleColor(proper);

                Panel tile = new Panel
                {
                    Size = new Size(tileW, tileH),
                    Location = new Point(startX + i * (tileW + gap), 172),
                    BackColor = Brand.Card,
                    Cursor = Cursors.Hand
                };

                tile.Controls.Add(new Panel
                {
                    Size = new Size(tileW, 4),
                    Location = new Point(0, 0),
                    BackColor = roleColor
                });

                Label name = new Label
                {
                    Text = roles[i],
                    Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                    ForeColor = Color.White,
                    AutoSize = false,
                    Size = new Size(tileW, 30),
                    Location = new Point(0, 22),
                    TextAlign = ContentAlignment.MiddleCenter
                };

                Label blurb = new Label
                {
                    Text = blurbs[i],
                    Font = Brand.Meta,
                    ForeColor = Brand.Muted,
                    AutoSize = false,
                    Size = new Size(tileW - 24, 46),
                    Location = new Point(12, 58),
                    TextAlign = ContentAlignment.TopCenter
                };

                Button select = new Button
                {
                    Text = "SELECT",
                    Size = new Size(tileW - 48, 36),
                    Location = new Point(24, tileH - 54),
                    BackColor = roleColor,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = Brand.Btn,
                    Cursor = Cursors.Hand
                };
                select.FlatAppearance.BorderSize = 0;

                string captured = proper;
                void Choose()
                {
                    SelectedRole = captured;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                void Hover(bool on) => tile.BackColor = on ? Brand.Raised : Brand.Card;

                foreach (Control c in new Control[] { tile, name, blurb, select })
                {
                    c.Click += (s, e) => Choose();
                    c.MouseEnter += (s, e) => Hover(true);
                    c.MouseLeave += (s, e) => Hover(false);
                }

                tile.Controls.Add(name);
                tile.Controls.Add(blurb);
                tile.Controls.Add(select);
                card.Controls.Add(tile);
            }

            this.Controls.Add(card);
            card.BringToFront();
        }
    }
}