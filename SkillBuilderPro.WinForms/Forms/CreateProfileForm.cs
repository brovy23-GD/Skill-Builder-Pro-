using System;
using System.Drawing;
using System.Windows.Forms;
using SkillBuilderPro.WinForms.Models;
using SkillBuilderPro.WinForms.Properties;
using SkillBuilderPro.WinForms.Services;

namespace SkillBuilderPro.WinForms
{
    /// <summary>
    /// CreateProfileForm - full-screen "walk the tunnel" experience.
    /// Tunnel background fills the window; the profile form floats in a
    /// dark card over the quiet center lane of the image.
    /// </summary>
    public partial class CreateProfileForm : Form
    {
        public User CreatedUser { get; private set; }

        private Panel card;

        private TextBox nameTextBox;
        private TextBox emailTextBox;
        private TextBox passwordTextBox;
        private TextBox phoneTextBox;

        private ComboBox sportComboBox;
        private ComboBox focusComboBox;
        private ComboBox goalComboBox;
        private NumericUpDown jerseyNumberUpDown;

        private Button createButton;
        private Button cancelButton;

        // Match LockerRoom / MainForm palette
        private static readonly Color CardDark = Color.FromArgb(32, 38, 50);
        private static readonly Color FieldBack = Color.FromArgb(42, 49, 63);
        private static readonly Color TextLight = Color.FromArgb(235, 238, 243);
        private static readonly Color SubtleText = Color.FromArgb(155, 170, 190);
        private static readonly Color AccentRed = Color.FromArgb(200, 16, 46);

        public CreateProfileForm()
        {
            InitializeCreateProfileForm();
            BuildLayout();
        }

        private void InitializeCreateProfileForm()
        {
            this.SuspendLayout();
            this.Text = "Create Profile - Skill Builder Pro";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(1000, 820);
            this.BackColor = Color.FromArgb(15, 18, 26);

            this.BackgroundImage = ApplyDarkOverlayLocal(Resource1.create_profile, 0.25f);
            this.BackgroundImageLayout = ImageLayout.Zoom;
            this.ResumeLayout(false);
        }

        private void BuildLayout()
        {
            this.Font = new Font("Segoe UI", 10F);

            // Dark card floating in the tunnel's center lane
            card = new Panel
            {
                Size = new Size(560, 700),
                BackColor = Color.FromArgb(235, CardDark.R, CardDark.G, CardDark.B)
            };
            void CenterCard() => card.Location = new Point(
                (this.ClientSize.Width - card.Width) / 2,
                Math.Max((this.ClientSize.Height - card.Height) / 2, 20));
            CenterCard();
            this.Resize += (s, e) => CenterCard();

            Label titleLabel = new Label
            {
                Text = "CREATE ATHLETE PROFILE",
                Font = new Font("Segoe UI Black", 18F),
                ForeColor = Color.White,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 54
            };
            card.Controls.Add(titleLabel);

            Label tagline = new Label
            {
                Text = "YOUR JOURNEY STARTS HERE",
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                ForeColor = SubtleText,
                AutoSize = false,
                Width = 520,
                Height = 20,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 54)
            };
            card.Controls.Add(tagline);

            int leftX = 60;
            int rightX = 300;
            int fullWidth = 440;
            int halfWidth = 200;
            int y = 92;

            // --- Text fields (full width) ---
            nameTextBox = AddTextField("FULL NAME", leftX, ref y, fullWidth);
            emailTextBox = AddTextField("EMAIL", leftX, ref y, fullWidth);
            passwordTextBox = AddTextField("PASSWORD", leftX, ref y, fullWidth);
            passwordTextBox.UseSystemPasswordChar = true;
            phoneTextBox = AddTextField("PHONE NUMBER", leftX, ref y, fullWidth);

            // --- Sport + Focus side by side ---
            int rowY = y;
            AddCaption("SPORT", leftX, rowY);
            sportComboBox = AddCombo(leftX, rowY + 18, halfWidth);
            sportComboBox.Items.AddRange(new object[]
            {
                "Basketball", "Baseball", "Softball", "Football", "Soccer", "Hockey"
            });
            sportComboBox.SelectedIndexChanged += SportComboBox_SelectedIndexChanged;

            AddCaption("TRAINING FOCUS", rightX, rowY);
            focusComboBox = AddCombo(rightX, rowY + 18, halfWidth);
            y = rowY + 62;

            // --- Jersey + Goal side by side ---
            rowY = y;
            AddCaption("JERSEY NUMBER", leftX, rowY);
            jerseyNumberUpDown = new NumericUpDown
            {
                Location = new Point(leftX, rowY + 18),
                Size = new Size(halfWidth, 28),
                Minimum = 0,
                Maximum = 999,
                Font = new Font("Segoe UI", 11F),
                BackColor = FieldBack,
                ForeColor = TextLight,
                BorderStyle = BorderStyle.FixedSingle
            };
            card.Controls.Add(jerseyNumberUpDown);

            AddCaption("PRIMARY GOAL", rightX, rowY);
            goalComboBox = AddCombo(rightX, rowY + 18, halfWidth);
            // Goals are sport-specific — populated in SportComboBox_SelectedIndexChanged
            y = rowY + 74;

            // --- Buttons ---
            createButton = new Button
            {
                Text = "CREATE PROFILE",
                Location = new Point(leftX, y),
                Size = new Size(halfWidth, 44),
                BackColor = AccentRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 11F)
            };
            createButton.FlatAppearance.BorderSize = 0;
            createButton.Click += CreateButton_Click;
            card.Controls.Add(createButton);

            cancelButton = new Button
            {
                Text = "CANCEL",
                Location = new Point(rightX, y),
                Size = new Size(halfWidth, 44),
                BackColor = Color.FromArgb(70, 70, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11F)
            };
            cancelButton.FlatAppearance.BorderSize = 0;
            cancelButton.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };
            card.Controls.Add(cancelButton);

            this.Controls.Add(card);
            card.BringToFront();
            this.AcceptButton = createButton;
            this.CancelButton = cancelButton;
        }

        // ------------------------------
        // FIELD BUILDER HELPERS
        // ------------------------------

        private void AddCaption(string text, int x, int y)
        {
            card.Controls.Add(new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = SubtleText,
                AutoSize = false,
                Size = new Size(220, 16),
                Location = new Point(x, y)
            });
        }

        private TextBox AddTextField(string caption, int x, ref int y, int width)
        {
            AddCaption(caption, x, y);
            TextBox box = new TextBox
            {
                Location = new Point(x, y + 18),
                Size = new Size(width, 28),
                Font = new Font("Segoe UI", 11F),
                BackColor = FieldBack,
                ForeColor = TextLight,
                BorderStyle = BorderStyle.FixedSingle
            };
            card.Controls.Add(box);
            y += 62;
            return box;
        }

        private ComboBox AddCombo(int x, int y, int width)
        {
            ComboBox combo = new ComboBox
            {
                Location = new Point(x, y),
                Size = new Size(width, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 11F),
                BackColor = FieldBack,
                ForeColor = TextLight,
                FlatStyle = FlatStyle.Flat
            };
            card.Controls.Add(combo);
            return combo;
        }

        // ------------------------------
        // SPORT → FOCUS + GOALS
        // ------------------------------

        private void SportComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            focusComboBox.Items.Clear();
            goalComboBox.Items.Clear();

            string sport = sportComboBox.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(sport))
                return;

            switch (sport)
            {
                case "Basketball":
                    focusComboBox.Items.AddRange(new object[]
                    {
                        "Shooting", "Ball Handling", "Defense", "Footwork"
                    });
                    goalComboBox.Items.AddRange(new object[]
                    {
                        "Raise shooting percentage",
                        "Improve ball handling under pressure",
                        "Become a lockdown defender",
                        "Increase vertical and explosiveness",
                        "Earn a starting spot"
                    });
                    break;

                case "Baseball":
                case "Softball":
                    focusComboBox.Items.AddRange(new object[]
                    {
                        "Hitting", "Fielding", "Throwing", "Base Running"
                    });
                    goalComboBox.Items.AddRange(new object[]
                    {
                        "Improve hitting consistency",
                        "Increase power at the plate",
                        "Sharpen fielding fundamentals",
                        "Improve throwing accuracy",
                        "Make the travel / select roster"
                    });
                    break;

                case "Football":
                    focusComboBox.Items.AddRange(new object[]
                    {
                        "Passing", "Route Running", "Blocking", "Tackling"
                    });
                    goalComboBox.Items.AddRange(new object[]
                    {
                        "Increase passing accuracy",
                        "Run sharper routes",
                        "Improve tackling technique",
                        "Build competitive conditioning",
                        "Earn a starting spot"
                    });
                    break;

                case "Soccer":
                    focusComboBox.Items.AddRange(new object[]
                    {
                        "Passing", "Dribbling", "Shooting", "Defense"
                    });
                    goalComboBox.Items.AddRange(new object[]
                    {
                        "Create and finish more chances",
                        "Improve first touch and control",
                        "Increase quickness and creativity",
                        "Become a stronger defender",
                        "Make the club / select roster"
                    });
                    break;

                case "Hockey":
                    focusComboBox.Items.AddRange(new object[]
                    {
                        "Shooting", "Stick Handling", "Skating", "Passing"
                    });
                    goalComboBox.Items.AddRange(new object[]
                    {
                        "Improve shot power and accuracy",
                        "Develop quicker hands",
                        "Increase skating speed and edges",
                        "Create more scoring opportunities",
                        "Earn top-line minutes"
                    });
                    break;
            }

            if (focusComboBox.Items.Count > 0)
                focusComboBox.SelectedIndex = 0;
            if (goalComboBox.Items.Count > 0)
                goalComboBox.SelectedIndex = 0;
        }

        // ------------------------------
        // CREATE
        // ------------------------------

        private void CreateButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nameTextBox.Text) ||
                string.IsNullOrWhiteSpace(emailTextBox.Text) ||
                string.IsNullOrWhiteSpace(passwordTextBox.Text) ||
                sportComboBox.SelectedItem == null ||
                focusComboBox.SelectedItem == null ||
                goalComboBox.SelectedItem == null)
            {
                MessageBox.Show(
                    "Please fill out all required fields.",
                    "Missing Info",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            string fullName = nameTextBox.Text.Trim();
            string email = emailTextBox.Text.Trim();
            string password = passwordTextBox.Text.Trim();
            string phone = phoneTextBox.Text.Trim();
            string sport = sportComboBox.SelectedItem.ToString();
            string targetArea = focusComboBox.SelectedItem.ToString();
            string experienceLevel = "Beginner";
            string role = "Athlete";
            int jerseyNumber = (int)jerseyNumberUpDown.Value;
            string goal = goalComboBox.SelectedItem.ToString();

            var auth = new AuthenticationService();
            var result = auth.SignUp(
                email,
                password,
                fullName,
                role,
                sport,
                targetArea,
                experienceLevel,
                phone,
                jerseyNumber,
                goal);

            if (!result.success)
            {
                MessageBox.Show(
                    result.message,
                    "Profile Not Created",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            CreatedUser = new User
            {
                FullName = fullName,
                Email = email,
                Password = password,
                Phone = phone,
                Sport = sport,
                TargetArea = targetArea,
                ExperienceLevel = experienceLevel,
                Role = role,
                IsActive = true,
                JerseyNumber = jerseyNumber,
                Goal = goal
            };

            MessageBox.Show(
                result.message,
                "Profile Created",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // ------------------------------
        // HELPERS
        // ------------------------------

        private static Image ApplyDarkOverlayLocal(Image source, float opacity = 0.25f)
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