using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SkillBuilderPro.WinForms.Models;
using SkillBuilderPro.WinForms.Properties;

namespace SkillBuilderPro.WinForms
{
    public partial class ParentDashboard : Form
    {
        private readonly User _user;
        private Panel athleteHost;
        private Label statsLabel;

        private readonly Color _accent = Brand.RoleColor("Parent");

        public ParentDashboard(User user)
        {
            _user = user;

            InitializeComponent();

            this.Text = "SkillBuilderPro - Parent Portal";
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(1200, 800);
            this.BackColor = Brand.Base;
            this.BackgroundImage = Brand.Darken(Resource1.parentsbackground, 0.30f);
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.Padding = new Padding(40, 0, 40, 40);
            this.DoubleBuffered = true;

            BuildBody();
            BuildHeader();
            LoadAthletes();
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
                Text = "PARENT PORTAL",
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
                Size = new Size(145, 40),
                BackColor = _accent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            exitButton.FlatAppearance.BorderSize = 0;
            exitButton.Click += (s, e) => Close();
            header.Controls.Add(exitButton);

            Button addButton = new Button
            {
                Text = "+ ADD ATHLETE",
                Size = new Size(165, 40),
                BackColor = Brand.Blue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            addButton.FlatAppearance.BorderSize = 0;
            addButton.FlatAppearance.MouseOverBackColor = Brand.BlueLit;
            addButton.Click += AddAthlete_Click;
            header.Controls.Add(addButton);

            void PositionRight()
            {
                exitButton.Location = new Point(
                    header.ClientSize.Width - exitButton.Width - 24, 25);
                addButton.Location = new Point(
                    exitButton.Left - addButton.Width - 12, 25);
            }
            header.Resize += (s, e) => PositionRight();
            PositionRight();

            NavHelper.AddBackButton(this, header);
            NavHelper.AddMuteButton(header, 360);

            this.Controls.Add(header);
            header.SendToBack();
        }

        private void BuildBody()
        {
            athleteHost = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(70, 18, 22, 30),
                AutoScroll = true
            };
            this.Controls.Add(athleteHost);
        }

        private void AddAthlete_Click(object sender, EventArgs e)
        {
            var form = new CreateProfileForm();
            if (form.ShowDialog(this) == DialogResult.OK && form.CreatedUser != null)
            {
                form.CreatedUser.Role = "Athlete";
                form.CreatedUser.GuardianEmail = _user.Email;
                form.CreatedUser.IsActive = true;

                AthleteStore.Add(form.CreatedUser);
                LoadAthletes();
            }
        }

        private void LoadAthletes()
        {
            var athletes = AthleteStore.AthletesFor(_user.Email);

            athleteHost.Controls.Clear();
            athleteHost.SuspendLayout();

            if (athletes.Count == 0)
            {
                athleteHost.Controls.Add(new Label
                {
                    Text = "No athletes linked to your account yet.\n\n"
                         + "Use + ADD ATHLETE to create a profile for your child.",
                    Font = new Font("Segoe UI", 13F),
                    ForeColor = Brand.Muted,
                    AutoSize = false,
                    Size = new Size(520, 90),
                    Location = new Point(40, 40)
                });
            }

            int y = 24;
            foreach (User a in athletes)
            {
                User captured = a;

                Panel card = new Panel
                {
                    Size = new Size(880, 92),
                    Location = new Point(40, y),
                    BackColor = Brand.Card
                };

                card.Controls.Add(new Panel
                {
                    Size = new Size(5, 92),
                    Location = new Point(0, 0),
                    BackColor = _accent
                });

                card.Controls.Add(new Label
                {
                    Text = a.JerseyNumber > 0 ? $"#{a.JerseyNumber}" : "—",
                    Font = new Font("Segoe UI Black", 17F),
                    ForeColor = _accent,
                    AutoSize = false,
                    Size = new Size(74, 30),
                    Location = new Point(18, 16),
                    TextAlign = ContentAlignment.MiddleLeft
                });

                card.Controls.Add(new Label
                {
                    Text = a.FullName,
                    Font = Brand.H2,
                    ForeColor = Brand.Text,
                    AutoSize = false,
                    Size = new Size(300, 26),
                    Location = new Point(100, 15)
                });

                card.Controls.Add(new Label
                {
                    Text = $"{a.Sport}  •  {a.Team}  •  {a.TargetArea}  •  {a.ExperienceLevel}",
                    Font = Brand.Meta,
                    ForeColor = Brand.Muted,
                    AutoSize = false,
                    Size = new Size(400, 20),
                    Location = new Point(100, 44)
                });

                card.Controls.Add(new Label
                {
                    Text = string.IsNullOrWhiteSpace(a.Goal) ? "No goal set" : a.Goal,
                    Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                    ForeColor = Color.FromArgb(198, 208, 224),
                    AutoSize = false,
                    Size = new Size(400, 18),
                    Location = new Point(100, 66)
                });

                Button viewLocker = MakeButton("LOCKER ROOM", Brand.Graphite, 640, 12);
                viewLocker.Click += (s, e) => new LockerRoomForm(captured).Show();

                Button viewTraining = MakeButton("TRAINING & GOALS", _accent, 640, 50);
                viewTraining.Click += (s, e) =>
                {
                    using (var athleteView = new MainForm(captured, false))
                        athleteView.ShowDialog(this);
                };

                card.Controls.Add(viewLocker);
                card.Controls.Add(viewTraining);
                athleteHost.Controls.Add(card);

                y += 102;
            }

            athleteHost.ResumeLayout();

            statsLabel.Text = athletes.Count == 0
                ? $"{_user.FullName}   |   No athletes yet"
                : $"{_user.FullName}   |   {athletes.Count} athlete"
                  + (athletes.Count == 1 ? "" : "s")
                  + $"   |   {string.Join(", ", athletes.Select(x => x.Sport).Distinct())}";
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