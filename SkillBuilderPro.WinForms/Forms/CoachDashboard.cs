using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SkillBuilderPro.WinForms.Forms;
using SkillBuilderPro.WinForms.Models;
using SkillBuilderPro.WinForms.Properties;
using SkillBuilderPro.WinForms.Utils;

namespace SkillBuilderPro.WinForms
{
    public partial class CoachDashboard : Form
    {
        private readonly User _user;
        private Panel rosterHost;
        private Label statsLabel;
        private ComboBox focusFilter;

        private readonly Color _accent = Brand.RoleColor("Coach");

        private const int CardW = 880;
        private const int CardH = 92;

        public CoachDashboard(User user)
        {
            _user = user;

            InitializeComponent();

            this.Text = "SkillBuilderPro - Coach Office";
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(1200, 800);
            this.BackColor = Brand.Base;
            this.BackgroundImage = Brand.Hero(Resource1.CoachOffice);
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.Padding = new Padding(40, 0, 40, 40);
            this.DoubleBuffered = true;

            BuildBody();
            BuildHeader();
            LoadRoster();
        }

        private void BuildHeader()
        {
            Panel header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = Brand.Panel
            };

            header.Controls.Add(new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 3,
                BackColor = _accent
            });

            header.Controls.Add(new Label
            {
                Text = "COACH'S OFFICE",
                ForeColor = Brand.Text,
                Font = Brand.H1,
                AutoSize = true,
                Location = new Point(150, 12)
            });

            statsLabel = new Label
            {
                ForeColor = Brand.Muted,
                Font = new Font("Segoe UI", 11F),
                AutoSize = true,
                Location = new Point(150, 50)
            };
            header.Controls.Add(statsLabel);

            Button exitButton = new Button
            {
                Text = "LOG OUT",
                Size = new Size(140, 40),
                BackColor = _accent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            exitButton.FlatAppearance.BorderSize = 0;
            exitButton.FlatAppearance.MouseOverBackColor = Brand.BlueLit;
            exitButton.Click += (s, e) => Close();
            header.Controls.Add(exitButton);

            focusFilter = new ComboBox
            {
                Width = 190,
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                Font = Brand.Body,
                BackColor = Brand.Raised,
                ForeColor = Brand.Text
            };
            focusFilter.SelectedIndexChanged += (s, e) => LoadRoster();
            header.Controls.Add(focusFilter);

            void PositionRight()
            {
                exitButton.Location = new Point(
                    header.ClientSize.Width - exitButton.Width - 24, 25);
                focusFilter.Location = new Point(
                    exitButton.Left - focusFilter.Width - 16, 30);
            }
            header.Resize += (s, e) => PositionRight();
            PositionRight();

            NavHelper.AddBackButton(this, header);
            NavHelper.AddMuteButton(header, 385);

            this.Controls.Add(header);
            header.SendToBack();
        }

        private void BuildBody()
        {
            rosterHost = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                AutoScroll = true
            };
            rosterHost.Resize += (s, e) => CenterCards();
            this.Controls.Add(rosterHost);
        }

        private void CenterCards()
        {
            int x = Math.Max((rosterHost.ClientSize.Width - CardW) / 2, 20);
            foreach (Control c in rosterHost.Controls)
                if (c is GlassRow)
                    c.Left = x;
        }

        private void LoadRoster()
        {
            var team = AthleteStore.AthletesBySport(_user.Sport);

            if (focusFilter.Items.Count == 0)
            {
                focusFilter.Items.Add("All Focus Areas");
                foreach (string f in team.Select(a => a.TargetArea).Distinct().OrderBy(f => f))
                    focusFilter.Items.Add(f);
                focusFilter.SelectedIndex = 0;
            }

            string filter = focusFilter.SelectedItem?.ToString() ?? "All Focus Areas";
            var shown = filter == "All Focus Areas"
                ? team
                : team.Where(a => a.TargetArea == filter).ToList();

            rosterHost.Controls.Clear();
            rosterHost.SuspendLayout();

            int y = 24;
            foreach (User a in shown)
            {
                User captured = a;

                GlassRow card = new GlassRow
                {
                    Size = new Size(CardW, CardH),
                    Location = new Point(40, y),
                    Fill = Brand.Card,
                    Alpha = 150,
                    HoverAlpha = 195,
                    HoverFill = Brand.Raised,
                    Stripe = _accent,
                    StripeWidth = 5
                };

                card.Cells.Add(new GlassRow.Cell
                {
                    Text = a.JerseyNumber > 0 ? "#" + a.JerseyNumber : "—",
                    X = 10,
                    Width = 80,
                    Font = new Font("Segoe UI Black", 17F),
                    Color = Brand.BlueLit
                });

                card.Cells.Add(new GlassRow.Cell
                {
                    Text = a.FullName,
                    X = 92,
                    Width = 320,
                    Y = 12,
                    H = 28,
                    Font = Brand.H2,
                    Color = Brand.TextStrong
                });

                card.Cells.Add(new GlassRow.Cell
                {
                    Text = a.Team + "   •   " + a.TargetArea + "   •   " + a.ExperienceLevel,
                    X = 92,
                    Width = 420,
                    Y = 42,
                    H = 22,
                    Font = Brand.RowCell,
                    Color = Brand.TextCell
                });

                card.Cells.Add(new GlassRow.Cell
                {
                    Text = string.IsNullOrWhiteSpace(a.Goal) ? "No goal set" : a.Goal,
                    X = 92,
                    Width = 420,
                    Y = 64,
                    H = 20,
                    Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                    Color = Brand.Muted
                });
                // vertical placement for the three stacked cells
                

                Button locker = MakeButton("LOCKER ROOM", Brand.Graphite, 640, 12);
                locker.Click += (s, e) => new LockerRoomForm(captured).Show();

                Button training = MakeButton("TRAINING & GOALS", _accent, 640, 50);
                training.Click += (s, e) =>
                {
                    using (var view = new MainForm(captured, false))
                        view.ShowDialog(this);
                };

                card.Controls.Add(locker);
                card.Controls.Add(training);
                rosterHost.Controls.Add(card);

                y += CardH + 10;
            }

            rosterHost.ResumeLayout();
            CenterCards();

            statsLabel.Text =
                $"{_user.FullName}   |   {_user.Sport}  •  {_user.Team}   |   " +
                $"{team.Count} athlete{(team.Count == 1 ? "" : "s")}   |   " +
                $"Advanced: {team.Count(a => a.ExperienceLevel == "Advanced")}";
        }

        private Button MakeButton(string text, Color bg, int x, int y)
        {
            Button b = new Button
            {
                Text = text,
                Size = new Size(200, 34),
                Location = new Point(x, y),
                BackColor = bg,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = Brand.Btn,
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = Brand.BlueLit;
            return b;
        }
    }
}