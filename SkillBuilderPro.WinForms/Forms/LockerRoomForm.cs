using SkillBuilderPro.WinForms.Models;
using SkillBuilderPro.WinForms.Properties;
using SkillBuilderPro.WinForms.Theming;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SkillBuilderPro.WinForms
{
    /// <summary>Panel with double-buffering so custom Paint doesn't flicker.</summary>
    public class BufferedPanel : Panel
    {
        public BufferedPanel() { this.DoubleBuffered = true; }
    }

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
        private Point doorClosedLocation;
        private bool lockerOpen;
        private Button backButton = null!;
        private Button exitButton = null!;

        private static readonly Color CardDark = Color.FromArgb(32, 38, 50);
        private static readonly Color Shelf = Color.FromArgb(42, 49, 63);
        private static readonly Color TextLight = Color.FromArgb(235, 238, 243);
        private static readonly Color SubtleText = Color.FromArgb(155, 170, 190);

        // Brand-blue glow used on the locker door and open card
        private static readonly Color GlowBlue = Color.FromArgb(88, 166, 255);

        public LockerRoomForm(User user)
        {
            _user = user;

            InitializeComponent();
            SetupForm();
            BuildMauiDossierInterior();
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
            this.MinimumSize = new Size(1000, 860);
            this.BackColor = Color.FromArgb(15, 18, 26);

            string approvedLockerRoomPath = System.IO.Path.Combine(
                AppContext.BaseDirectory,
                "Resources",
                "locker_room_background_approved.png");
            if (System.IO.File.Exists(approvedLockerRoomPath))
            {
                using Image approvedLockerRoom = Image.FromFile(approvedLockerRoomPath);
                this.BackgroundImage = new Bitmap(approvedLockerRoom);
            }
            else
            {
                this.BackgroundImage = ApplyDarkOverlay(Resource1.LockerRoom, 0.35f);
            }
            this.BackgroundImageLayout = ImageLayout.Zoom;
        }

        private void AlignPanels()
        {
            interiorPanel.Location = new Point(
                Math.Max((this.ClientSize.Width - interiorPanel.Width) / 2, 0),
                Math.Max((this.ClientSize.Height - interiorPanel.Height) / 2, 10));

            if (doorPanel != null && !slideTimer.Enabled)
            {
                doorClosedLocation = new Point(
                    Math.Max((this.ClientSize.Width - doorPanel.Width) / 2, 0),
                    Math.Max((this.ClientSize.Height - doorPanel.Height) / 2, 10));

                if (!lockerOpen)
                    doorPanel.Location = doorClosedLocation;

                doorPanel.BringToFront();
                backButton?.BringToFront();
                exitButton?.BringToFront();
            }
        }

        // ------------------------------
        // LOCKER INTERIOR - full player card
        // ------------------------------

        private void BuildMauiDossierInterior()
        {
            interiorPanel = new BufferedPanel
            {
                Size = new Size(900, 760),
                BackColor = Color.FromArgb(245, 10, 16, 24),
                Visible = false
            };

            Label AddLabel(string text, float size, FontStyle style, Color color, Rectangle bounds,
                ContentAlignment alignment = ContentAlignment.MiddleLeft)
            {
                var label = new Label
                {
                    Text = text,
                    Font = new Font("Segoe UI", size, style),
                    ForeColor = color,
                    BackColor = Color.Transparent,
                    AutoSize = false,
                    Bounds = bounds,
                    TextAlign = alignment
                };
                interiorPanel.Controls.Add(label);
                return label;
            }

            void AddValuePair(string caption, string value, int x, int y, int width)
            {
                AddLabel(caption, 8.5F, FontStyle.Regular, SubtleText, new Rectangle(x, y, width, 20));
                AddLabel(string.IsNullOrWhiteSpace(value) ? "-" : value, 15F, FontStyle.Regular, TextLight,
                    new Rectangle(x, y + 19, width, 32));
            }

            interiorPanel.Paint += (s, e) =>
            {
                using var border = new Pen(Color.FromArgb(74, 107, 132, 159), 1);
                e.Graphics.DrawRectangle(border, 0, 0, interiorPanel.Width - 1, interiorPanel.Height - 1);
            };

            AddLabel("ATHLETE DOSSIER", 9F, FontStyle.Bold, GlowBlue, new Rectangle(24, 18, 300, 22));
            AddLabel((_user.FullName ?? "ATHLETE").ToUpperInvariant(), 23F, FontStyle.Regular, TextLight,
                new Rectangle(24, 42, 600, 42));
            AddLabel(_user.IsActive ? "ACTIVE ATHLETE" : "INACTIVE ATHLETE", 9F, FontStyle.Regular,
                SubtleText, new Rectangle(24, 84, 300, 20));

            profilePictureBox = new PictureBox
            {
                Bounds = new Rectangle(24, 122, 220, 230),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(120, 22, 30, 41),
                BorderStyle = BorderStyle.FixedSingle
            };
            if (!string.IsNullOrWhiteSpace(_user.PhotoPath) && System.IO.File.Exists(_user.PhotoPath))
                profilePictureBox.Image = Image.FromFile(_user.PhotoPath);
            else
                profilePictureBox.Image = CreateSportAvatar(_user.Sport, 180);
            interiorPanel.Controls.Add(profilePictureBox);

            var uploadPhotoButton = CreateButton("CHANGE PHOTO", Color.FromArgb(28, 42, 58), 150, 38);
            uploadPhotoButton.Location = new Point(59, 364);
            uploadPhotoButton.Click += UploadPhotoButton_Click;
            interiorPanel.Controls.Add(uploadPhotoButton);

            AddLabel("PLAYER CARD", 13F, FontStyle.Bold, TextLight, new Rectangle(274, 122, 300, 28));
            AddValuePair("PRIMARY SPORT", _user.Sport, 274, 158, 250);
            AddValuePair("LOCKER NUMBER", _user.JerseyNumber > 0 ? _user.JerseyNumber.ToString() : "-", 610, 158, 230);
            AddValuePair("DEVELOPMENT FOCUS", _user.TargetArea, 274, 226, 250);
            AddValuePair("SKILL LEVEL", _user.ExperienceLevel, 610, 226, 230);
            AddValuePair("TEAM", _user.Team, 274, 294, 250);
            AddValuePair("ROLE", _user.Role, 610, 294, 230);

            void AddMetric(string caption, string value, int x, int width)
            {
                var metric = new Panel { Bounds = new Rectangle(x, 430, width, 82), BackColor = Color.FromArgb(170, 18, 27, 38) };
                metric.Controls.Add(new Label { Text = caption, Dock = DockStyle.Top, Height = 28, ForeColor = SubtleText, Font = new Font("Segoe UI", 8.5F), TextAlign = ContentAlignment.BottomCenter });
                metric.Controls.Add(new Label { Text = value, Dock = DockStyle.Fill, ForeColor = TextLight, Font = new Font("Segoe UI", 16F), TextAlign = ContentAlignment.MiddleCenter });
                interiorPanel.Controls.Add(metric);
            }
            AddMetric("CURRENT RANK", string.IsNullOrWhiteSpace(_user.ExperienceLevel) ? "-" : _user.ExperienceLevel, 24, 200);
            AddMetric("STREAK", "-", 242, 150);
            AddMetric("ACTIVE GOALS", string.IsNullOrWhiteSpace(_user.Goal) ? "0" : "1", 410, 160);
            AddMetric("NEXT MILESTONE", string.IsNullOrWhiteSpace(_user.Goal) ? "Not set" : _user.Goal, 588, 288);

            var contact = new Panel { Bounds = new Rectangle(24, 532, 400, 112), BackColor = Color.FromArgb(170, 18, 27, 38) };
            contact.Controls.Add(new Label { Text = "CONTACT", ForeColor = GlowBlue, Font = new Font("Segoe UI", 9F, FontStyle.Bold), Bounds = new Rectangle(18, 12, 160, 22) });
            contact.Controls.Add(new Label { Text = $"{_user.Email}\r\n{_user.Phone}", ForeColor = TextLight, Font = new Font("Segoe UI", 10.5F), Bounds = new Rectangle(18, 38, 360, 58) });
            interiorPanel.Controls.Add(contact);

            var bio = new Panel { Bounds = new Rectangle(442, 532, 434, 112), BackColor = Color.FromArgb(170, 18, 27, 38) };
            bio.Controls.Add(new Label { Text = "ATHLETE BIO", ForeColor = GlowBlue, Font = new Font("Segoe UI", 9F, FontStyle.Bold), Bounds = new Rectangle(18, 12, 180, 22) });
            bio.Controls.Add(new Label { Text = string.IsNullOrWhiteSpace(_user.Bio) ? "No athlete bio added." : _user.Bio, ForeColor = TextLight, Font = new Font("Segoe UI", 10.5F), Bounds = new Rectangle(18, 38, 398, 60) });
            interiorPanel.Controls.Add(bio);

            var editProfileButton = CreateButton("EDIT PROFILE", Color.FromArgb(28, 42, 58), 160, 42);
            editProfileButton.Location = new Point(24, 674);
            editProfileButton.Click += EditProfileButton_Click;
            interiorPanel.Controls.Add(editProfileButton);

            this.Controls.Add(interiorPanel);
        }

        // Retained for compatibility reference; the active locker now uses the
        // MAUI-standard Athlete Dossier built above.
        private void BuildInterior()
        {
            var theme = TeamThemes.GetThemeForSport(_user.Sport);

            interiorPanel = new BufferedPanel
            {
                Size = new Size(520, 790),
                BackColor = CardDark,
                Visible = false
            };

            // Blue brand glow outline around the open locker card
            interiorPanel.Paint += (s, e) =>
            {
                Graphics g = e.Graphics;
                for (int i = 4; i >= 1; i--)
                {
                    using (Pen glow = new Pen(Color.FromArgb(30 * (5 - i), GlowBlue), i * 2))
                        g.DrawRectangle(glow, i, i, interiorPanel.Width - i * 2 - 1, interiorPanel.Height - i * 2 - 1);
                }
            };

            // Nameplate strip - jersey number + name
            Panel nameplate = new Panel { Dock = DockStyle.Top, Height = 58, BackColor = theme.Accent };
            nameplate.Controls.Add(new Label
            {
                Text = $"#{_user.JerseyNumber}  {(_user.FullName ?? "ATHLETE").ToUpper()}",
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
            uploadPhotoButton.Location = new Point((520 - 170) / 2, 226);
            uploadPhotoButton.Click += UploadPhotoButton_Click;
            interiorPanel.Controls.Add(uploadPhotoButton);

            // SHELF 1 - PLAYER CARD: identity + status chip
            string statusText = _user.IsActive ? "ACTIVE" : "INACTIVE";
            Panel shelfA = CreateShelfPanel(276, 126, "PLAYER CARD", theme.Accent);
            AddStat(shelfA, "SPORT", _user.Sport, 24, 36, 210);
            AddStat(shelfA, "FOCUS", _user.TargetArea, 250, 36, 210);
            AddStat(shelfA, "LEVEL", _user.ExperienceLevel, 24, 80, 210);
            AddStat(shelfA, "ROLE", _user.Role, 250, 80, 210);
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

            // SHELF 2 - CONTACT: email full row, phone + age on row two
            Panel shelfB = CreateShelfPanel(414, 126, "CONTACT", theme.Accent);
            AddStat(shelfB, "EMAIL", _user.Email, 24, 36, 452);
            AddStat(shelfB, "PHONE", _user.Phone, 24, 80, 210);
            AddStat(shelfB, "AGE", $"{_user.Age}", 250, 80, 120);
            interiorPanel.Controls.Add(shelfB);

            // SHELF 3 - PHYSICAL: stats row + full-width wrapping bio
            Panel shelfC = CreateShelfPanel(552, 152, "PHYSICAL", theme.Accent);
            AddStat(shelfC, "HEIGHT", FormatHeight(_user.Height), 24, 36, 130);
            AddStat(shelfC, "WEIGHT", $"{_user.Weight} lbs", 160, 36, 130);
            AddStat(shelfC, "TEAM", _user.Team, 296, 36, 180);
            AddBioStat(shelfC, string.IsNullOrWhiteSpace(_user.Bio) ? "-" : _user.Bio, 24, 84, 452);
            interiorPanel.Controls.Add(shelfC);

            Button editProfileButton = CreateButton("EDIT PROFILE", theme.Accent, 220, 40);
            editProfileButton.Location = new Point((520 - 220) / 2, 726);
            editProfileButton.Click += EditProfileButton_Click;
            interiorPanel.Controls.Add(editProfileButton);

            this.Controls.Add(interiorPanel);
        }

        // ------------------------------
        // EDIT PROFILE
        // ------------------------------

        private void EditProfileButton_Click(object sender, EventArgs e)
        {
            if (ShowEditProfileDialog())
            {
                // Rebuild the interior so every shelf reflects the edits
                this.Controls.Remove(interiorPanel);
                interiorPanel.Dispose();
                BuildMauiDossierInterior();
                interiorPanel.Visible = lockerOpen;
                interiorPanel.BringToFront();
                AlignPanels();
            }
        }

        /// <summary>
        /// Dark modal for editing the profile fields shown in the locker.
        /// Returns true if the user saved changes.
        /// </summary>
        private bool ShowEditProfileDialog()
        {
            var theme = TeamThemes.GetThemeForSport(_user.Sport);
            bool saved = false;

            using (Form dlg = new Form())
            {
                dlg.Text = "Edit Profile";
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.Size = new Size(420, 560);
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.MaximizeBox = false;
                dlg.MinimizeBox = false;
                dlg.BackColor = CardDark;

                int y = 20;

                TextBox AddField(string caption, string value)
                {
                    dlg.Controls.Add(new Label
                    {
                        Text = caption,
                        Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                        ForeColor = SubtleText,
                        AutoSize = false,
                        Size = new Size(340, 16),
                        Location = new Point(30, y)
                    });
                    TextBox box = new TextBox
                    {
                        Text = value ?? "",
                        Font = new Font("Segoe UI", 10F),
                        BackColor = Shelf,
                        ForeColor = TextLight,
                        BorderStyle = BorderStyle.FixedSingle,
                        Size = new Size(340, 26),
                        Location = new Point(30, y + 18)
                    };
                    dlg.Controls.Add(box);
                    y += 56;
                    return box;
                }

                TextBox nameBox = AddField("FULL NAME", _user.FullName);
                TextBox phoneBox = AddField("PHONE", _user.Phone);
                TextBox teamBox = AddField("TEAM", _user.Team);
                TextBox heightBox = AddField("HEIGHT (e.g. 5.10 = 5'10\")", _user.Height.ToString());
                TextBox weightBox = AddField("WEIGHT (lbs)", _user.Weight.ToString());
                TextBox bioBox = AddField("BIO", _user.Bio);
                TextBox goalBox = AddField("GOAL", _user.Goal);

                Button saveBtn = CreateButton("SAVE", theme.Accent, 150, 38);
                saveBtn.Location = new Point(30, y + 8);
                saveBtn.Click += (s, e) =>
                {
                    _user.FullName = nameBox.Text.Trim();
                    _user.Phone = phoneBox.Text.Trim();
                    _user.Team = teamBox.Text.Trim();
                    if (double.TryParse(heightBox.Text.Trim(), out double h)) _user.Height = h;
                    if (double.TryParse(weightBox.Text.Trim(), out double w)) _user.Weight = w;
                    _user.Bio = bioBox.Text.Trim();
                    _user.Goal = goalBox.Text.Trim();
                    saved = true;
                    dlg.DialogResult = DialogResult.OK;
                };

                Button cancelBtn = CreateButton("CANCEL", Color.FromArgb(70, 70, 80), 120, 38);
                cancelBtn.Location = new Point(250, y + 8);
                cancelBtn.Click += (s, e) => dlg.DialogResult = DialogResult.Cancel;

                dlg.Controls.Add(saveBtn);
                dlg.Controls.Add(cancelBtn);
                dlg.AcceptButton = saveBtn;
                dlg.CancelButton = cancelBtn;

                dlg.ShowDialog(this);
            }

            return saved;
        }

        // ------------------------------
        // LOCKER DOOR - team color, glow outline, jersey number,
        // nameplate, vents, handle; click to open
        // ------------------------------

        private void BuildDoor()
        {
            var theme = TeamThemes.GetThemeForSport(_user.Sport);

            doorPanel = new BufferedPanel
            {
                // Match the MAUI closed-door visual object and aspect ratio.
                Size = new Size(350, 548),
                BackColor = theme.Panel,
                Cursor = Cursors.Hand
            };

            string approvedDoorPath = System.IO.Path.Combine(AppContext.BaseDirectory, "Resources", "locker_door_dynamic_approved.png");
            if (System.IO.File.Exists(approvedDoorPath))
            {
                using Image approvedDoor = Image.FromFile(approvedDoorPath);
                doorPanel.BackgroundImage = new Bitmap(approvedDoor);
                doorPanel.BackgroundImageLayout = ImageLayout.Zoom;
            }

            Label doorNameplate = new Label
            {
                Text = (_user.FullName ?? "ATHLETE").ToUpper(),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(216, 222, 230),
                BackColor = Color.FromArgb(16, 20, 25),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand
            };

            Label? lockerNumber = _user.JerseyNumber > 0 ? new Label
            {
                Text = _user.JerseyNumber.ToString(),
                Font = new Font("Segoe UI Semibold", 62F, FontStyle.Bold),
                ForeColor = Color.FromArgb(85, 174, 255),
                BackColor = Color.Transparent,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand
            } : null;

            doorPanel.Controls.Add(doorNameplate);
            if (lockerNumber is not null)
                doorPanel.Controls.Add(lockerNumber);

            void PositionDoorOverlays()
            {
                int width = doorPanel.ClientSize.Width;
                int height = doorPanel.ClientSize.Height;
                doorNameplate.Bounds = new Rectangle((int)(width * .355), (int)(height * .066), (int)(width * .285), (int)(height * .055));
                if (lockerNumber is not null)
                    lockerNumber.Bounds = new Rectangle((int)(width * .36), (int)(height * .48), (int)(width * .28), (int)(height * .22));
            }

            doorPanel.SizeChanged += (s, e) => PositionDoorOverlays();
            PositionDoorOverlays();

            this.Controls.Add(doorPanel);
            doorPanel.BringToFront();

            // Slide-open animation, started by clicking anywhere on the door
            slideTimer = new System.Windows.Forms.Timer { Interval = 15 };
            int step = Math.Max(doorPanel.Width / 26, 12);
            slideTimer.Tick += (s, e) =>
            {
                doorPanel.Left -= step;
                if (doorPanel.Left <= doorClosedLocation.X - doorPanel.Width)
                {
                    slideTimer.Stop();
                    doorPanel.Visible = false;
                    lockerOpen = true;
                    interiorPanel.BringToFront();
                    backButton.BringToFront();
                    exitButton.BringToFront();
                }
            };

            EventHandler openDoor = (s, e) =>
            {
                if (doorPanel.Visible && !slideTimer.Enabled)
                {
                    // MAUI reveals the dossier behind the moving door, then leaves
                    // the opaque locker interior as the active view.
                    interiorPanel.Visible = true;
                    interiorPanel.BringToFront();
                    doorPanel.BringToFront();
                    backButton.BringToFront();
                    exitButton.BringToFront();
                    slideTimer.Start();
                }
            };
            doorPanel.Click += openDoor;
            doorNameplate.Click += openDoor;
            if (lockerNumber is not null)
                lockerNumber.Click += openDoor;
        }

        private void BuildBackButton()
        {
            backButton = CreateButton("BACK", Color.FromArgb(60, 66, 74), 100, 42);
            backButton.Location = new Point(24, 24);
            backButton.Click += (s, e) =>
            {
                if (lockerOpen || slideTimer.Enabled)
                {
                    slideTimer.Stop();
                    lockerOpen = false;
                    interiorPanel.Visible = false;
                    doorPanel.Location = doorClosedLocation;
                    doorPanel.Visible = true;
                    doorPanel.BringToFront();
                    backButton.BringToFront();
                    return;
                }

                this.Close();
            };
            this.Controls.Add(backButton);
            backButton.BringToFront();

            exitButton = CreateButton("EXIT", Color.FromArgb(60, 66, 74), 100, 42);
            exitButton.Location = new Point(136, 24);
            exitButton.Click += (s, e) => this.Close();
            this.Controls.Add(exitButton);
            exitButton.BringToFront();
        }

        // ------------------------------
        // HELPERS
        // ------------------------------

        /// <summary>Formats 6.3 as 6'3" and 5.10 as 5'10". Data stores feet.inches.</summary>
        private static string FormatHeight(double height)
        {
            int feet = (int)height;
            string raw = height.ToString();
            int dot = raw.IndexOf('.');
            string inches = dot >= 0 ? raw.Substring(dot + 1) : "0";
            return $"{feet}'{inches}\"";
        }

        /// <summary>Bio row: italic, two-line wrap, no truncation.</summary>
        private void AddBioStat(Panel shelf, string bio, int x, int y, int width)
        {
            shelf.Controls.Add(new Label
            {
                Text = "BIO",
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
                Text = bio,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Italic),
                ForeColor = TextLight,
                AutoSize = false,
                Width = width,
                Height = 44,
                TextAlign = ContentAlignment.TopLeft,
                Location = new Point(x, y + 14)
            });
        }

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
                else if (s == "hockey")
                {
                    using (var ice = new SolidBrush(Color.FromArgb(210, 225, 238)))
                    using (var puck = new SolidBrush(Color.FromArgb(25, 25, 28)))
                    using (var puckEdge = new Pen(Color.FromArgb(70, 70, 76), 2))
                    {
                        g.FillEllipse(ice, ball);                      // ice circle
                        Rectangle p = new Rectangle(ball.Left + ball.Width / 5, ball.Top + ball.Height / 3,
                                                    ball.Width * 3 / 5, ball.Height / 3);
                        g.FillEllipse(puck, p);                        // puck body
                        g.DrawEllipse(puckEdge, p);
                        g.DrawEllipse(puckEdge, p.Left, p.Top - 6, p.Width, p.Height / 2); // top face
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
