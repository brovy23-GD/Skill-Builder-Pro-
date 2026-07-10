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
        private Button exitDemoButton;
        private Button createProfileButton;

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

            this.BackgroundImage = Resource1.weight_room;
            this.BackgroundImageLayout = ImageLayout.Zoom;
        }

        private void BuildLoginCard()
        {
            // Locker door plaque / coach's clipboard - dark, matte
            Panel card = new Panel
            {
                Size = new Size(420, 480),
                BackColor = Color.FromArgb(40, 40, 48),
                BorderStyle = BorderStyle.FixedSingle
            };
            void CenterCard() => card.Location = new Point(
                (this.ClientSize.Width - card.Width) / 2,
                (this.ClientSize.Height - card.Height) / 2);
            CenterCard();
            this.Resize += (s, e) => CenterCard();

            Label title = new Label
            {
                Text = "ENTER LOCKER ROOM",
                Font = new Font("Segoe UI Black", 22F),
                ForeColor = Color.White,
                AutoSize = false,
                Height = 60,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter
            };
            card.Controls.Add(title);

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
                Width = 160,
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

            exitDemoButton = new Button
            {
                Text = "EXIT DEMO",
                Width = 160,
                Height = 40,
                Location = new Point(220, 320),
                BackColor = Color.FromArgb(90, 40, 40),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11F)
            };
            exitDemoButton.FlatAppearance.BorderSize = 0;
            exitDemoButton.Click += ExitDemo_Click;
            card.Controls.Add(exitDemoButton);

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
            // Per spec: build the user from the email + default values (for now)
            LoggedInUser = new User
            {
                FullName = "Demo Athlete",
                Sport = "Basketball",
                TargetArea = "Shooting",
                ExperienceLevel = "Intermediate",
                Role = "Athlete",
                IsActive = true,
                Email = emailBox.Text,
                Phone = "000-000-0000",
                Age = 16,
                Height = 70,
                Weight = 150,
                Team = "Skill Builder Elite",
                Bio = "Skill Builder Pro athlete."
            };

            IsDemoMode = false;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void DemoMode_Click(object sender, EventArgs e)
        {
            LoggedInUser = new User
            {
                FullName = "Demo Athlete",
                Sport = "Basketball",
                TargetArea = "Shooting",
                ExperienceLevel = "Intermediate",
                Role = "Athlete",
                IsActive = true,
                Email = "demo@skillbuilderpro.com",
                Phone = "000-000-0000",
                Age = 16,
                Height = 70,
                Weight = 150,
                Team = "Skill Builder Elite",
                Bio = "Demo profile showcasing Skill Builder Pro."
            };

            IsDemoMode = true;
            this.DialogResult = DialogResult.OK;
            this.Close();
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

        private void ExitDemo_Click(object sender, EventArgs e)
        {
            LoggedInUser = null;
            IsDemoMode = false;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}