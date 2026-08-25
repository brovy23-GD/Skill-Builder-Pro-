using System;
using System.Drawing;
using System.Windows.Forms;
using SkillBuilderPro.WinForms.Models;
using SkillBuilderPro.WinForms.Properties;
using SkillBuilderPro.WinForms.Services;

namespace SkillBuilderPro.WinForms;

public partial class RoleSelectForm : Form
{
    public string SelectedRole { get; private set; } = string.Empty;
    public bool IsDemoMode { get; private set; }

    public RoleSelectForm()
    {
        InitializeComponent();
        Text = "SkillBuilderPro";
        StartPosition = FormStartPosition.CenterScreen;
        WindowState = FormWindowState.Maximized;
        MinimumSize = new Size(1000, 720);
        BackgroundImage = DesktopVisualResolver.Current.GetChooseExperienceBackground();
        BackgroundImageLayout = ImageLayout.Zoom;
        DoubleBuffered = true;
        BuildRoleSelector();
    }

    private void BuildRoleSelector()
    {
        var surface = new Panel { BackColor = Color.Transparent };
        Controls.Add(surface);

        var title = new Label
        {
            Text = "CHOOSE YOUR EXPERIENCE",
            Font = new Font("Segoe UI Semibold", 19F, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = Color.FromArgb(205, 18, 24, 33)
        };
        var subtitle = new Label
        {
            Text = "SELECT HOW YOU'LL ENTER SKILL BUILDER PRO",
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            ForeColor = Brand.Muted,
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = Color.FromArgb(205, 18, 24, 33)
        };
        surface.Controls.Add(title);
        surface.Controls.Add(subtitle);

        string[] labels = ["ATHLETE", "COACH", "PARENT", "ADMINISTRATOR"];
        string[] values = ["Athlete", "Coach", "Parent", "Admin"];
        string[] descriptions =
        [
            "Train. Compete. Elevate.",
            "Lead. Develop. Win.",
            "Support. Guide. Empower.",
            "Manage. Oversee. Optimize."
        ];
        string[] monograms = ["A", "C", "P", "AD"];
        var tiles = new Panel[4];

        for (var i = 0; i < labels.Length; i++)
        {
            var tile = new Panel { BackColor = Color.FromArgb(226, 15, 21, 29), Cursor = Cursors.Hand, TabStop = false };
            var accent = new Panel { BackColor = Brand.Blue, Height = 3, Dock = DockStyle.Top };
            var icon = new Label
            {
                Text = monograms[i],
                Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold),
                ForeColor = Brand.Blue,
                AutoSize = false,
                Size = new Size(34, 28),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(8, 13)
            };
            var name = new Label
            {
                Text = labels[i],
                Font = new Font("Segoe UI Semibold", 11.5F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Height = 28,
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(44, 13)
            };
            var description = new Label
            {
                Text = descriptions[i],
                Font = Brand.Meta,
                ForeColor = Brand.Muted,
                AutoSize = false,
                Height = 28,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(8, 46)
            };
            var select = new Button
            {
                Text = "SELECT",
                BackColor = Color.FromArgb(36, 48, 62),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = Brand.Btn,
                Cursor = Cursors.Hand,
                TabIndex = i,
                AccessibleName = $"Continue as {labels[i]}",
                AccessibleDescription = descriptions[i]
            };
            select.FlatAppearance.BorderColor = Brand.Blue;
            select.FlatAppearance.BorderSize = 1;

            var selectedValue = values[i];
            void Choose()
            {
                IsDemoMode = false;
                SelectedRole = selectedValue;
                DialogResult = DialogResult.OK;
                Close();
            }
            void Hover(bool active)
            {
                tile.BackColor = active ? Color.FromArgb(240, 22, 32, 43) : Color.FromArgb(226, 15, 21, 29);
                select.FlatAppearance.BorderSize = active ? 2 : 1;
            }

            select.Click += (_, _) => Choose();
            foreach (Control control in new Control[] { tile, icon, name, description })
            {
                control.Click += (_, _) => Choose();
                control.MouseEnter += (_, _) => Hover(true);
                control.MouseLeave += (_, _) => Hover(false);
            }
            select.MouseEnter += (_, _) => Hover(true);
            select.MouseLeave += (_, _) => Hover(false);
            tile.Controls.Add(accent);
            tile.Controls.Add(icon);
            tile.Controls.Add(name);
            tile.Controls.Add(description);
            tile.Controls.Add(select);
            surface.Controls.Add(tile);
            tiles[i] = tile;
        }

        var demoPanel = new Panel
        {
            BackColor = Color.FromArgb(215, 15, 21, 29),
            Cursor = Cursors.Hand
        };
        var demoIcon = new Label
        {
            Text = "D",
            Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold),
            ForeColor = Brand.Blue,
            AutoSize = false,
            Size = new Size(34, 34),
            Location = new Point(8, 7),
            TextAlign = ContentAlignment.MiddleCenter
        };
        var demoButton = new Button
        {
            Text = "DEMO MODE",
            Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = Color.FromArgb(30, 39, 51),
            FlatStyle = FlatStyle.Flat,
            Size = new Size(112, 34),
            Location = new Point(46, 7),
            Cursor = Cursors.Hand,
            TabIndex = 4,
            AccessibleName = "Demo Mode",
            AccessibleDescription = "Explore Skill Builder Pro. No sign-in required."
        };
        demoButton.FlatAppearance.BorderColor = Brand.Blue;
        demoButton.FlatAppearance.BorderSize = 1;
        var demoDescription = new Label
        {
            Text = "Explore Skill Builder Pro",
            Font = Brand.Meta,
            ForeColor = Brand.Muted,
            AutoSize = false,
            Size = new Size(185, 34),
            Location = new Point(167, 7),
            TextAlign = ContentAlignment.MiddleLeft
        };
        void EnterDemo()
        {
            IsDemoMode = true;
            SelectedRole = "Athlete";
            DialogResult = DialogResult.OK;
            Close();
        }
        void HoverDemo(bool active)
        {
            demoPanel.BackColor = active ? Color.FromArgb(235, 20, 29, 40) : Color.FromArgb(215, 15, 21, 29);
            demoButton.FlatAppearance.BorderSize = active ? 2 : 1;
        }
        demoButton.Click += (_, _) => EnterDemo();
        foreach (Control control in new Control[] { demoPanel, demoIcon, demoDescription })
        {
            control.Click += (_, _) => EnterDemo();
            control.MouseEnter += (_, _) => HoverDemo(true);
            control.MouseLeave += (_, _) => HoverDemo(false);
        }
        demoButton.MouseEnter += (_, _) => HoverDemo(true);
        demoButton.MouseLeave += (_, _) => HoverDemo(false);
        demoPanel.Controls.Add(demoIcon);
        demoPanel.Controls.Add(demoButton);
        demoPanel.Controls.Add(demoDescription);
        surface.Controls.Add(demoPanel);

        void LayoutSelector()
        {
            const int gap = 14;
            var width = Math.Min(1100, Math.Max(900, ClientSize.Width - 80));
            var tileWidth = (width - gap * 3) / 4;
            var tileHeight = 128;
            var surfaceHeight = 280;
            surface.SetBounds((ClientSize.Width - width) / 2, Math.Max(20, ClientSize.Height - surfaceHeight - 20), width, surfaceHeight);
            title.SetBounds(0, 0, width, 43);
            subtitle.SetBounds(0, 43, width, 27);
            for (var i = 0; i < tiles.Length; i++)
            {
                var tile = tiles[i];
                tile.SetBounds(i * (tileWidth + gap), 80, tileWidth, tileHeight);
                tile.Controls[2].Width = tileWidth - 52;
                tile.Controls[3].Width = tileWidth - 16;
                tile.Controls[4].SetBounds(22, 82, tileWidth - 44, 34);
            }
            demoPanel.SetBounds((width - 360) / 2, 220, 360, 48);
        }

        LayoutSelector();
        Resize += (_, _) => LayoutSelector();
        surface.BringToFront();
    }
}
