using SkillBuilderPro.WinForms.Models;
using SkillBuilderPro.WinForms.Properties;
using SkillBuilderPro.WinForms.Theming;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SkillBuilderPro.WinForms
{
    /// <summary>
    /// LockerRoomForm - you walk into the locker room and stand at YOUR
    /// locker: a team-color metal door with your nameplate, vents, and
    /// handle. Click it and it slides open to the full player card.
    /// </summary>
    public partial class LockerRoomForm : Form
    {
        private readonly User _user;

        private Panel interiorPanel;    // the open locker (full player card)
        private Panel doorPanel;        // team-color metal door covering it
        private System.Windows.Forms.Timer slideTimer;
        private PictureBox profilePictureBox;

        private static readonly Color CardDark = Color.FromArgb(32, 38, 50);
        private static readonly Color Shelf = Color.FromArgb(42, 49, 63);
        private static readonly Color TextLight = Color.FromArgb(235, 238, 243);
        private static readonly Color SubtleText = Color.FromArgb(155, 170, 190);

        public LockerRoomForm(User user)
        {
            _user = user;

            InitializeComponent();
            SetupForm();
            BuildInterior();
            BuildDoor();
            BuildBackButton();

            this.Shown += (s, e) => AlignPanels();
            this.Resize += (s, e) => AlignPanels();
        }

        private void SetupForm()
        {
            this.Text = "Locker Room - Skill Builder Pro";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(1000, 820);
            this.BackColor = Color.FromArgb(15, 18, 26);

            this.BackgroundImage = ApplyDarkOverlay(Resource1.LockerRoom, 0.35f);
            this.BackgroundImageLayout = ImageLayout.Zoom;
        }

        private void AlignPanels()
        {
            interiorPanel.Location = new Point(
                Math.Max((this.ClientSize.Width - interiorPanel.Width) / 2, 0),
                Math.Max((this.ClientSize.Height - interiorPanel.Height) / 2, 10));

            if (doorPanel != null && doorPanel.Visible && !slideTimer.Enabled)
            {
                doorPanel.Size = interiorPanel.Size;
                doorPanel.Location = interiorPanel.Location;
                doorPanel.BringToFront();
            }
        }

        // ------------------------------
        // LOCKER INTERIOR - full player card
        // ------------------------------

        private void BuildInterior()
        {
            var theme = TeamThemes.GetThemeForSport(_user.Sport);

            interiorPanel = new Panel
            {
                Size = new Size(520, 760),
                BackColor = CardDark
            };

            // Nameplate strip
            Panel nameplate = new Panel { Dock = DockStyle.Top, Height = 58, BackColor = theme.Accent };
            nameplate.Controls.Add(new Label
            {
                Text = (_user.FullName ?? "ATHLETE").ToUpper(),
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            });
            interiorPanel.Controls.Add(nameplate);

            // Photo / sport-ball avatar on the jersey hook
            profilePictureBox = new PictureBox
            {
                Size = new Size(140, 140),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Shelf,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point((520 - 140) / 2, 74)
            };
            if (!string.IsNullOrWhiteSpace(_user.PhotoPath) && System.IO.File.Exists(_user.PhotoPath))
                profilePictureBox.Image = Image.FromFile(_user.PhotoPath);
            else
                profilePictureBox.Image = CreateSportAvatar(_user.Sport, 140);
            interiorPanel.Controls.Add(profilePictureBox);

            Button uploadPhotoButton = CreateButton("UPLOAD PHOTO", theme.Accent, 170, 32);
            uploadPhotoButton.Location = new Point((520 - 170) / 2, 224);
            uploadPhotoButton.Click += UploadPhotoButton_Click;
            interiorPanel.Controls.Add(uploadPhotoButton);

            // SHELF 1 - PLAYER CARD: identity + status chip
            string statusText = _user.IsActive ? "ACTIVE" : "INACTIVE";
            Panel shelfA = CreateShelfPanel(274, 122, "PLAYER CARD", theme.Accent);
            AddStat(shelfA, "SPORT", _user.Sport, 24, 34, 210);
            AddStat(shelfA, "FOCUS", _user.TargetArea, 246, 34, 210);
            AddStat(shelfA, "LEVEL", _user.ExperienceLevel, 24, 76, 210);
            AddStat(shelfA, "ROLE", _user.Role, 246, 76, 210);
            shelfA.Controls.Add(new Label
            {
                Text = statusText,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = _user.IsActive ? Color.FromArgb(38, 120, 70) : Color.FromArgb(120, 45, 45),
                AutoSize = false,
                Size = new Size(72, 20),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(shelfA.Width - 96, 8)
            });
            interiorPanel.Controls.Add(shelfA);

            // SHELF 2 - CONTACT
            Panel shelfB = CreateShelfPanel(408, 96, "CONTACT", theme.Accent);
            AddStat(shelfB, "EMAIL", _user.Email, 24, 34, 300);
            AddStat(shelfB, "PHONE", _user.Phone, 330, 34, 140);
            AddStat(shelfB, "AGE", $"{_user.Age}", 24, 62, 120);
            interiorPanel.Controls.Add(shelfB);

            // SHELF 3 - PHYSICAL + TEAM + BIO
            Panel shelfC = CreateShelfPanel(516, 140, "PHYSICAL", theme.Accent);
            AddStat(shelfC, "HEIGHT", $"{_user.Height}", 24, 34, 140);
            AddStat(shelfC, "WEIGHT", $"{_user.Weight}", 170, 34, 140);
            AddStat(shelfC, "TEAM", _user.Team, 316, 34, 160);
            AddStat(shelfC, "BIO", string.IsNullOrWhiteSpace(_user.Bio) ? "-" : _user.Bio, 24, 82, 448);
            interiorPanel.Controls.Add(shelfC);

            Button editProfileButton = CreateButton("EDIT PROFILE", theme.Accent, 220, 40);
            editProfileButton.Location = new Point((520 - 220) / 2, 686);
            interiorPanel.Controls.Add(editProfileButton);

            this.Controls.Add(interiorPanel);
        }

        // ------------------------------
        // LOCKER DOOR - team color, nameplate, vents, handle; click to open
        // ------------------------------

        private void BuildDoor()
        {
            var theme = TeamThemes.GetThemeForSport(_user.Sport);

            doorPanel = new Panel
            {
                Size = interiorPanel.Size,
                Location = interiorPanel.Location,
                BackColor = theme.Panel,
                Cursor = Cursors.Hand
            };

            Label doorNameplate = new Label
            {
                Text = (_user.FullName ?? "ATHLETE").ToUpper(),
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = false,
                Width = doorPanel.Width - 40,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 24),
                Cursor = Cursors.Hand
            };
            Panel nameplateStrip = new Panel
            {
                Size = new Size(doorPanel.Width - 80, 3),
                Location = new Point(40, 68),
                BackColor = Color.FromArgb(170, 175, 182),
                Cursor = Cursors.Hand
            };

            var ventPanels = new List<Panel>();
            for (int i = 0; i < 4; i++)
            {
                var vent = new Panel
                {
                    Size = new Size(110, 7),
                    Location = new Point((doorPanel.Width - 110) / 2, 112 + i * 22),
                    BackColor = Color.FromArgb(40, 42, 48),
                    Cursor = Cursors.Hand
                };
                ventPanels.Add(vent);
                doorPanel.Controls.Add(vent);
            }

            Panel handle = new Panel
            {
                Size = new Size(12, 52),
                Location = new Point(doorPanel.Width - 30, doorPanel.Height / 2 - 26),
                BackColor = Color.FromArgb(150, 155, 162),
                Cursor = Cursors.Hand
            };

            Label hintLabel = new Label
            {
                Text = "CLICK TO OPEN YOUR LOCKER",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(200, 205, 212),
                AutoSize = false,
                Width = doorPanel.Width - 40,
                Height = 26,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, doorPanel.Height - 60),
                Cursor = Cursors.Hand
            };

            doorPanel.Controls.Add(doorNameplate);
            doorPanel.Controls.Add(nameplateStrip);
            doorPanel.Controls.Add(handle);
            doorPanel.Controls.Add(hintLabel);

            this.Controls.Add(doorPanel);
            doorPanel.BringToFront();

            // Slide-open animation, started by clicking anywhere on the door
            slideTimer = new System.Windows.Forms.Timer { Interval = 15 };
            int step = Math.Max(interiorPanel.Width / 26, 12);
            slideTimer.Tick += (s, e) =>
            {
                doorPanel.Width -= step;
                if (doorPanel.Width <= 0)
                {
                    slideTimer.Stop();
                    doorPanel.Visible = false;
                }
            };

            EventHandler openDoor = (s, e) =>
            {
                if (doorPanel.Visible && !slideTimer.Enabled)
                    slideTimer.Start();
            };
            doorPanel.Click += openDoor;
            doorNameplate.Click += openDoor;
            nameplateStrip.Click += openDoor;
            handle.Click += openDoor;
            hintLabel.Click += openDoor;
            foreach (var vent in ventPanels)
                vent.Click += openDoor;
        }

        private void BuildBackButton()
        {
            Button backButton = CreateButton("← BACK TO DASHBOARD", Color.FromArgb(60, 66, 74), 220, 42);
            backButton.Location = new Point(24, 24);
            backButton.Click += (s, e) => this.Close();
            this.Controls.Add(backButton);
            backButton.BringToFront();
        }

        // ------------------------------
        // HELPERS
        // ------------------------------

        private Panel CreateShelfPanel(int y, int height, string caption, Color accent)
        {
            Panel shelf = new Panel
            {
                Size = new Size(interiorPanel.Width - 40, height),
                Location = new Point(20, y),
                BackColor = Shelf
            };

            shelf.Controls.Add(new Label
            {
                Text = string.Join(" ", caption.ToCharArray()),
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = SubtleText,
                AutoSize = false,
                Width = 220,
                Height = 18,
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(24, 8)
            });
            shelf.Controls.Add(new Panel
            {
                Size = new Size(34, 2),
                Location = new Point(24, 27),
                BackColor = accent
            });

            return shelf;
        }

        private void AddStat(Panel shelf, string caption, string value, int x, int y, int width)
        {
            shelf.Controls.Add(new Label
            {
                Text = caption,
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = SubtleText,
                AutoSize = false,
                Width = width,
                Height = 14,
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(x, y)
            });
            shelf.Controls.Add(new Label
            {
                Text = string.IsNullOrWhiteSpace(value) ? "-" : value,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = TextLight,
                AutoSize = false,
                Width = width,
                Height = 24,
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(x, y + 14)
            });
        }

        private Button CreateButton(string text, Color backColor, int width, int height)
        {
            Button btn = new Button
            {
                Text = text,
                Width = width,
                Height = height,
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            btn.FlatAppearance.BorderSize = 0;

            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(
                Math.Min(backColor.R + 20, 255),
                Math.Min(backColor.G + 20, 255),
                Math.Min(backColor.B + 20, 255));
            btn.MouseLeave += (s, e) => btn.BackColor = backColor;

            return btn;
        }

        private void UploadPhotoButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Select Profile Photo";
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string userFolder = System.IO.Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                        "SkillBuilderPro", "ProfilePhotos");

                    if (!System.IO.Directory.Exists(userFolder))
                        System.IO.Directory.CreateDirectory(userFolder);

                    string destPath = System.IO.Path.Combine(userFolder, System.IO.Path.GetFileName(ofd.FileName));
                    System.IO.File.Copy(ofd.FileName, destPath, true);
                    _user.PhotoPath = destPath;
                    profilePictureBox.Image = Image.FromFile(destPath);
                }
            }
        }

        private Image CreateSportAvatar(string sport, int size)
        {
            Bitmap avatar = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(avatar))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(Shelf);

                int pad = size / 6;
                Rectangle ball = new Rectangle(pad, pad, size - pad * 2, size - pad * 2);
                string s = (sport ?? "").Trim().ToLower();

                if (s == "basketball")
                {
                    using (var orange = new SolidBrush(Color.FromArgb(214, 106, 40)))
                    using (var seam = new Pen(Color.FromArgb(30, 30, 30), 3))
                    {
                        g.FillEllipse(orange, ball);
                        g.DrawEllipse(seam, ball);
                        g.DrawLine(seam, ball.Left, ball.Top + ball.Height / 2, ball.Right, ball.Top + ball.Height / 2);
                        g.DrawLine(seam, ball.Left + ball.Width / 2, ball.Top, ball.Left + ball.Width / 2, ball.Bottom);
                    }
                }
                else if (s == "baseball" || s == "softball")
                {
                    Color ballColor = s == "softball" ? Color.FromArgb(222, 214, 120) : Color.FromArgb(235, 235, 230);
                    using (var body = new SolidBrush(ballColor))
                    using (var outline = new Pen(Color.FromArgb(120, 120, 120), 2))
                    using (var stitch = new Pen(Color.FromArgb(170, 40, 45), 3))
                    {
                        g.FillEllipse(body, ball);
                        g.DrawEllipse(outline, ball);
                        g.DrawArc(stitch, ball.Left - ball.Width / 3, ball.Top + ball.Height / 8, ball.Width, ball.Height * 3 / 4, -70, 140);
                        g.DrawArc(stitch, ball.Left + ball.Width / 3, ball.Top + ball.Height / 8, ball.Width, ball.Height * 3 / 4, 110, 140);
                    }
                }
                else if (s == "football")
                {
                    Rectangle fb = new Rectangle(pad / 2, size / 4, size - pad, size / 2);
                    using (var brown = new SolidBrush(Color.FromArgb(120, 68, 33)))
                    using (var lace = new Pen(Color.White, 3))
                    {
                        g.FillEllipse(brown, fb);
                        int cx = size / 2, cy = size / 2;
                        g.DrawLine(lace, cx - size / 6, cy, cx + size / 6, cy);
                        for (int i = -2; i <= 2; i++)
                            g.DrawLine(lace, cx + i * size / 14, cy - size / 18, cx + i * size / 14, cy + size / 18);
                    }
                }
                else
                {
                    using (var body = new SolidBrush(Color.FromArgb(120, 132, 150)))
                    {
                        int headSize = size / 3;
                        g.FillEllipse(body, (size - headSize) / 2, size / 6, headSize, headSize);
                        g.FillEllipse(body, size / 6, size / 2 + size / 12, size - size / 3, size);
                    }
                }
            }
            return avatar;
        }

        private static Image ApplyDarkOverlay(Image source, float opacity = 0.45f)
        {
            Bitmap darkened = new Bitmap(source.Width, source.Height);
            using (Graphics g = Graphics.FromImage(darkened))
            using (SolidBrush overlay = new SolidBrush(Color.FromArgb((int)(opacity * 255), 0, 0, 0)))
            {
                g.DrawImage(source, 0, 0, source.Width, source.Height);
                g.FillRectangle(overlay, 0, 0, source.Width, source.Height);
            }
            return darkened;
        }
    }
}