using SkillBuilderPro.WinForms.Models;
using SkillBuilderPro.WinForms.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SkillBuilderPro.WinForms
{
    /// <summary>
    /// Locker-room-style login - the entry tunnel into the app.
    /// Cinematic card over the weight room: email + password,
    /// ENTER LOCKER ROOM, DEMO MODE, EXIT DEMO, CREATE PROFILE.
    /// </summary>
    public partial class LoginForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsDemoMode { get; set; } = false;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public User LoggedInUser { get; set; }

        private TextBox emailBox;
        private TextBox passwordBox;
        private Button enterButton;
        private Button demoButton;
        private Button createProfileButton;
        private Label loginErrorLabel;

        public LoginForm()
        {
            InitializeComponent();
            SetupForm();
            BuildLoginCard();
        }

        private void SetupForm()
        {
            this.Text = "SkillBuilderPro Login";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(1000, 800);

            this.BackgroundImage = ApplyDarkOverlayLogin(Resource1.weight_room, 0.20f);
            this.BackgroundImageLayout = ImageLayout.Stretch;
        }

        private void BuildLoginCard()
        {
            // Locker door plaque / coach's clipboard - dark, matte
            Panel card = new Panel
            {
                Size = new Size(420, 480),
                BackColor = Color.FromArgb(170, 40, 40, 48),   // ~67% opaque — weight room shows through
                BorderStyle = BorderStyle.FixedSingle
            };
            // Positioned in the upper portion of the screen so the branding
            // on the gym floor (lower half of the photo) stays fully visible.
            void CenterCard() => card.Location = new Point(
                (this.ClientSize.Width - card.Width) / 2,
                Math.Max((int)(this.ClientSize.Height * 0.08), 20));
            CenterCard();
            this.Resize += (s, e) => CenterCard();

            Label title = new Label
            {
                Text = "SKILL BUILDER PRO",
                Font = new Font("Segoe UI Black", 22F),
                ForeColor = Color.White,
                AutoSize = false,
                Height = 52,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter
            };
            card.Controls.Add(title);

            // Tagline under the brand
            Label tagline = new Label
            {
                Text = "BUILT FOR ATHLETES. POWERED BY PRECISION.",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Italic),
                ForeColor = Color.FromArgb(155, 170, 190),
                AutoSize = false,
                Width = 380,
                Height = 22,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 56)
            };
            card.Controls.Add(tagline);

            Label emailLabel = new Label
            {
                Text = "Email",
                Font = new Font("Segoe UI", 12F),
                ForeColor = Color.White,
                Location = new Point(40, 90)
            };
            card.Controls.Add(emailLabel);

            emailBox = new TextBox
            {
                Width = 340,
                Location = new Point(40, 120),
                Font = new Font("Segoe UI", 12F)
            };
            card.Controls.Add(emailBox);

            Label passwordLabel = new Label
            {
                Text = "Password",
                Font = new Font("Segoe UI", 12F),
                ForeColor = Color.White,
                Location = new Point(40, 170)
            };
            card.Controls.Add(passwordLabel);

            passwordBox = new TextBox
            {
                Width = 340,
                Location = new Point(40, 200),
                Font = new Font("Segoe UI", 12F),
                UseSystemPasswordChar = true
            };
            card.Controls.Add(passwordBox);

            loginErrorLabel = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(235, 100, 110),
                AutoSize = false,
                Width = 340,
                Height = 20,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(40, 238)
            };
            card.Controls.Add(loginErrorLabel);

            enterButton = new Button
            {
                Text = "ENTER LOCKER ROOM",
                Width = 340,
                Height = 48,
                Location = new Point(40, 260),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 14F)
            };
            enterButton.FlatAppearance.BorderSize = 0;
            enterButton.Click += EnterLockerRoom;
            card.Controls.Add(enterButton);

            demoButton = new Button
            {
                Text = "DEMO MODE",
                Width = 340,
                Height = 40,
                Location = new Point(40, 320),
                BackColor = Color.FromArgb(70, 70, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11F)
            };
            demoButton.FlatAppearance.BorderSize = 0;
            demoButton.Click += DemoMode_Click;
            card.Controls.Add(demoButton);

            // Sign-up path - opens the full CreateProfileForm
            createProfileButton = new Button
            {
                Text = "NEW ATHLETE?  CREATE PROFILE",
                Width = 340,
                Height = 40,
                Location = new Point(40, 380),
                BackColor = Color.FromArgb(150, 15, 45),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold)
            };
            createProfileButton.FlatAppearance.BorderSize = 0;
            createProfileButton.Click += CreateProfile_Click;
            card.Controls.Add(createProfileButton);

            this.Controls.Add(card);
            card.BringToFront();
        }

        private void EnterLockerRoom(object sender, EventArgs e)
        {
            // Real login only - authenticates an existing profile.
            // Demo access is the DEMO MODE button; sign-up is CREATE PROFILE.
            string email = emailBox.Text?.Trim() ?? "";
            string password = passwordBox.Text ?? "";

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                loginErrorLabel.Text = "Enter your email and password.";
                return;
            }

            var auth = new SkillBuilderPro.WinForms.Services.AuthenticationService();
            var result = auth.LogIn(email, password);

            if (!result.success || result.user == null)
            {
                loginErrorLabel.Text = string.IsNullOrWhiteSpace(result.message)
                    ? "Login failed. Check your credentials."
                    : result.message;
                return;
            }

            LoggedInUser = result.user;
            IsDemoMode = false;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void DemoMode_Click(object sender, EventArgs e)
        {
            // DEMO MODE opens the athlete picker - choose anyone from the
            // DummyUsers roster to tour the app as (each sport shows its own
            // theme, backgrounds, and drill library).
            User selected = ShowDemoAthletePicker();
            if (selected == null)
                return;   // picker cancelled - stay on the login screen

            selected.IsActive = true;   // demo athletes always show ACTIVE
            LoggedInUser = selected;
            IsDemoMode = true;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        private static Image ApplyDarkOverlayLogin(Image source, float opacity = 0.20f)
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
        /// <summary>
        /// Small dark modal listing the DummyUsers roster.
        /// Returns the chosen athlete, or null if cancelled.
        /// </summary>
        private User ShowDemoAthletePicker()
        {
            var roster = DummyUsers.GetAllDummyUsers();
            User chosen = null;

            using (Form picker = new Form())
            {
                picker.Text = "Choose Demo Athlete";
                picker.StartPosition = FormStartPosition.CenterParent;
                picker.Size = new Size(430, 460);
                picker.FormBorderStyle = FormBorderStyle.FixedDialog;
                picker.MaximizeBox = false;
                picker.MinimizeBox = false;
                picker.BackColor = Color.FromArgb(32, 38, 50);

                Label header = new Label
                {
                    Text = "WHO ARE YOU TRAINING AS?",
                    Font = new Font("Segoe UI Black", 13F),
                    ForeColor = Color.White,
                    AutoSize = false,
                    Height = 44,
                    Dock = DockStyle.Top,
                    TextAlign = ContentAlignment.MiddleCenter
                };

                ListBox rosterList = new ListBox
                {
                    Font = new Font("Segoe UI", 11F),
                    BackColor = Color.FromArgb(42, 49, 63),
                    ForeColor = Color.FromArgb(235, 238, 243),
                    BorderStyle = BorderStyle.None,
                    Location = new Point(20, 54),
                    Size = new Size(374, 290),
                    ItemHeight = 24
                };
                foreach (User u in roster)
                    rosterList.Items.Add($"{u.FullName}  -  {u.Sport} ({u.ExperienceLevel})");
                rosterList.SelectedIndex = 0;

                Button okButton = new Button
                {
                    Text = "ENTER AS ATHLETE",
                    Size = new Size(200, 42),
                    Location = new Point(20, 358),
                    BackColor = Color.FromArgb(0, 120, 215),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI Semibold", 10.5F)
                };
                okButton.FlatAppearance.BorderSize = 0;
                okButton.Click += (s, e) => { picker.DialogResult = DialogResult.OK; };

                Button cancelButton = new Button
                {
                    Text = "CANCEL",
                    Size = new Size(120, 42),
                    Location = new Point(274, 358),
                    BackColor = Color.FromArgb(70, 70, 80),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 10.5F)
                };
                cancelButton.FlatAppearance.BorderSize = 0;
                cancelButton.Click += (s, e) => { picker.DialogResult = DialogResult.Cancel; };

                // Double-clicking an athlete enters immediately
                rosterList.DoubleClick += (s, e) => { picker.DialogResult = DialogResult.OK; };

                picker.Controls.Add(rosterList);
                picker.Controls.Add(header);
                picker.Controls.Add(okButton);
                picker.Controls.Add(cancelButton);
                picker.AcceptButton = okButton;
                picker.CancelButton = cancelButton;

                if (picker.ShowDialog(this) == DialogResult.OK && rosterList.SelectedIndex >= 0)
                    chosen = roster[rosterList.SelectedIndex];
            }

            return chosen;
        }

        private void CreateProfile_Click(object sender, EventArgs e)
        {
            var profileForm = new CreateProfileForm();
            if (profileForm.ShowDialog() == DialogResult.OK && profileForm.CreatedUser != null)
            {
                LoggedInUser = profileForm.CreatedUser;
                IsDemoMode = false;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

    }
}