using SkillBuilderPro.WinForms.Models;
using SkillBuilderPro.WinForms.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

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

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SelectedRole { get; set; } = "Athlete";   // ← ADD THIS

        private TextBox emailBox;
        private TextBox passwordBox;
        private Button enterButton;
        private Button demoButton;
        private Button createProfileButton;
        private Label loginErrorLabel;

        public LoginForm() : this("Athlete") { }

        public LoginForm(string role)
        {
            SelectedRole = string.IsNullOrWhiteSpace(role) ? "Athlete" : role;

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
           
            bool isAdmin = SelectedRole == "Admin";

            Panel card = new Panel
            {
                Size = new Size(420, isAdmin ? 410 : 462),
                BackColor = Color.FromArgb(170, 40, 40, 48),
                BorderStyle = BorderStyle.FixedSingle
            };

            void CenterCard() => card.Location = new Point(
                (this.ClientSize.Width - card.Width) / 2,
                Math.Max((int)(this.ClientSize.Height * 0.12), 20));
            CenterCard();
            this.Resize += (s, e) => CenterCard();

            card.Controls.Add(new Label
            {
                Text = "SKILL BUILDER PRO",
                Font = new Font("Segoe UI Black", 22F),
                ForeColor = Color.White,
                AutoSize = false,
                Height = 52,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter
            });

            card.Controls.Add(new Label
            {
                Text = "BUILT FOR ATHLETES. POWERED BY PRECISION.",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Italic),
                ForeColor = Color.FromArgb(155, 170, 190),
                AutoSize = false,
                Width = 380,
                Height = 20,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 54)
            });

            // ROLE BANNER
            card.Controls.Add(new Label
            {
                Text = SelectedRole.ToUpper(),
                Font = new Font("Segoe UI Black", 15F),
                ForeColor = Color.White,
                BackColor = Brand.RoleColor(SelectedRole),
                AutoSize = false,
                Width = 340,
                Height = 34,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(40, 80)
            });

            card.Controls.Add(new Label
            {
                Text = "Email",
                Font = new Font("Segoe UI", 12F),
                ForeColor = Color.White,
                Location = new Point(40, 126)
            });

            emailBox = new TextBox
            {
                Width = 340,
                Location = new Point(40, 154),
                Font = new Font("Segoe UI", 12F)
            };
            card.Controls.Add(emailBox);

            card.Controls.Add(new Label
            {
                Text = "Password",
                Font = new Font("Segoe UI", 12F),
                ForeColor = Color.White,
                Location = new Point(40, 194)
            });

            passwordBox = new TextBox
            {
                Width = 340,
                Location = new Point(40, 222),
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
                Location = new Point(40, 258)
            };
            card.Controls.Add(loginErrorLabel);

            enterButton = new Button
            {
                Text = EnterText(SelectedRole),
                Width = 340,
                Height = 48,
                Location = new Point(40, 282),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 13F),
                Cursor = Cursors.Hand
            };
            enterButton.FlatAppearance.BorderSize = 0;
            enterButton.Click += EnterLockerRoom;
            card.Controls.Add(enterButton);

            demoButton = new Button
            {
                Text = $"DEMO AS {SelectedRole.ToUpper()}",
                Width = 340,
                Height = 40,
                Location = new Point(40, 340),
                BackColor = Color.FromArgb(70, 70, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            demoButton.FlatAppearance.BorderSize = 0;
            demoButton.Click += DemoMode_Click;
            card.Controls.Add(demoButton);

            if (!isAdmin)
            {
                createProfileButton = new Button
                {
                    Text = $"NEW {SelectedRole.ToUpper()}?  CREATE PROFILE",
                    Width = 340,
                    Height = 40,
                    Location = new Point(40, 390),
                    BackColor = Color.FromArgb(150, 15, 45),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                createProfileButton.FlatAppearance.BorderSize = 0;
                createProfileButton.Click += CreateProfile_Click;
                card.Controls.Add(createProfileButton);
            }

            this.AcceptButton = enterButton;
            this.Controls.Add(card);
            card.BringToFront();

            NavHelper.AddBackButton(this, this);
        }

        private static string EnterText(string role)
        {
            switch (role)
            {
                case "Coach": return "ENTER COACH'S OFFICE";
                case "Parent": return "ENTER PARENT PORTAL";
                case "Admin": return "ENTER ADMIN CONSOLE";
                default: return "ENTER LOCKER ROOM";
            }
        }

       
        private void EnterLockerRoom(object sender, EventArgs e)
        {
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

            result.user.Role = SelectedRole;

            LoggedInUser = result.user;
            IsDemoMode = false;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        private void DemoMode_Click(object sender, EventArgs e)
        {
            User selected = ShowDemoAthletePicker();
            if (selected == null)
                return;

            selected.IsActive = true;
            selected.Role = SelectedRole;
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

        private User ShowDemoAthletePicker()
        {
            var roster = DummyUsers.GetAllDummyUsers()
                .Where(u => u.Role == SelectedRole)
                .ToList();

            if (roster.Count == 0)
            {
                MessageBox.Show($"No demo users for role: {SelectedRole}");
                return null;
            }

            User chosen = null;   // ← this line got lost

            using (Form picker = new Form())
            {
                // ... rest unchanged ...
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
                profileForm.CreatedUser.Role = SelectedRole;   // ← ADD
                LoggedInUser = profileForm.CreatedUser;
                IsDemoMode = false;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
