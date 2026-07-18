using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SkillBuilderPro.WinForms.Models;
using SkillBuilderPro.WinForms.Properties;

namespace SkillBuilderPro.WinForms
{
    public partial class AdminDashboard : Form
    {
        private readonly User _user;
        private Panel rosterHost;
        private Label statsLabel;

        private readonly Color _accent = Brand.RoleColor("Admin");
        private readonly List<Control> _blocks = new List<Control>();

        private static readonly string[] ColNames =
            { "NAME", "ROLE", "SPORT", "TEAM", "LEVEL" };
        private static readonly int[] ColWidths =
            { 165, 85, 100, 150, 100 };

        private const int RowH = 26;
        private const int RowGap = 29;
        private const int ColGap = 36;

        public AdminDashboard(User user)
        {
            _user = user;

            InitializeComponent();

            this.Text = "SkillBuilderPro - Admin Console";
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(1200, 800);
            this.BackColor = Brand.Base;
            this.BackgroundImage = Brand.Hero(Resource1.AdminDash);
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
                Text = "ADMIN CONSOLE",
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
            exitButton.FlatAppearance.MouseOverBackColor = Brand.Raised;
            exitButton.Click += (s, e) => Close();
            header.Controls.Add(exitButton);

            void PositionRight() => exitButton.Location =
                new Point(header.ClientSize.Width - exitButton.Width - 24, 25);
            header.Resize += (s, e) => PositionRight();
            PositionRight();

            NavHelper.AddBackButton(this, header);
            NavHelper.AddMuteButton(header);

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
            rosterHost.Resize += (s, e) => LayoutBlocks();
            this.Controls.Add(rosterHost);
        }

        private int TableWidth => ColWidths.Sum();

        private void LoadRoster()
        {
            var roster = AthleteStore.AllUsers()
                .OrderBy(u => u.Role == "Athlete" ? 1 : 0)
                .ThenBy(u => u.FullName)
                .ToList();

            rosterHost.Controls.Clear();
            _blocks.Clear();
            rosterHost.SuspendLayout();

            foreach (User u in roster)
            {
                GlassRow row = new GlassRow
                {
                    Size = new Size(TableWidth, RowH),
                    Fill = Brand.Card,
                    Alpha = 150,
                    Stripe = Brand.RoleColor(u.Role)
                };

                string[] cells = { u.FullName, u.Role, u.Sport, u.Team, u.ExperienceLevel };

                int cx = 0;
                for (int i = 0; i < cells.Length; i++)
                {
                    row.Cells.Add(new GlassRow.Cell
                    {
                        Text = cells[i],
                        X = cx,
                        Width = ColWidths[i],
                        Font = i == 0 ? Brand.RowName : Brand.RowCell,
                        Color = i == 0 ? Brand.TextStrong : Brand.TextCell
                    });
                    cx += ColWidths[i];
                }

                rosterHost.Controls.Add(row);
                _blocks.Add(row);
            }

            rosterHost.ResumeLayout();
            LayoutBlocks();

            statsLabel.Text =
                $"Total: {roster.Count}   |   " +
                $"Athletes: {roster.Count(u => u.Role == "Athlete")}   |   " +
                $"Coaches: {roster.Count(u => u.Role == "Coach")}   |   " +
                $"Parents: {roster.Count(u => u.Role == "Parent")}   |   " +
                $"Sports: {roster.Select(u => u.Sport).Distinct().Count()}";
        }

        /// <summary>Centers the table and splits into 2 columns when there's room.</summary>
        private void LayoutBlocks()
        {
            if (_blocks.Count == 0) return;

            int avail = rosterHost.ClientSize.Width;
            int cols = avail >= (TableWidth * 2 + ColGap + 80) ? 2 : 1;

            int blockW = TableWidth * cols + ColGap * (cols - 1);
            int startX = Math.Max((avail - blockW) / 2, 20);
            int perCol = (int)Math.Ceiling(_blocks.Count / (double)cols);

            for (int i = 0; i < _blocks.Count; i++)
            {
                int col = i / perCol;
                int idx = i % perCol;
                _blocks[i].Location = new Point(
                    startX + col * (TableWidth + ColGap),
                    24 + idx * RowGap);
            }
        }
    }
}