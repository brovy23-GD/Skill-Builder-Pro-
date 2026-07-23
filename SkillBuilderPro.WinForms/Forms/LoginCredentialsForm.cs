using System;
using System.Drawing;
using System.Windows.Forms;
using SkillBuilderPro.WinForms.Models;
using SkillBuilderPro.WinForms.Services;

namespace SkillBuilderPro.WinForms
{
    public partial class LoginCredentialsForm : Form
    {
        public User LoggedInUser { get; private set; }

        private AuthenticationService _authService;

        private Label titleLabel;
        private Label emailLabel;
        private Label passwordLabel;

        private TextBox emailTextBox;
        private TextBox passwordTextBox;

        private Button loginButton;
        private Button cancelButton;

        public LoginCredentialsForm()
        {
            _authService = new AuthenticationService();

            InitializeLoginForm();
            BuildLayout();
        }

        private void InitializeLoginForm()
        {
            this.SuspendLayout();
            this.ClientSize = new Size(400, 260);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Login";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.ResumeLayout(false);
        }

        private void BuildLayout()
        {
            this.BackColor = Color.FromArgb(15, 47, 79);
            this.Font = new Font("Segoe UI", 10F);

            titleLabel = new Label
            {
                Text = "Sign In",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 15),
                Size = new Size(400, 30)
            };

            emailLabel = new Label
            {
                Text = "Email",
                ForeColor = Color.White,
                Location = new Point(40, 65),
                AutoSize = true
            };

            emailTextBox = new TextBox
            {
                Location = new Point(40, 85),
                Size = new Size(320, 25)
            };

            passwordLabel = new Label
            {
                Text = "Password",
                ForeColor = Color.White,
                Location = new Point(40, 120),
                AutoSize = true
            };

            passwordTextBox = new TextBox
            {
                Location = new Point(40, 140),
                Size = new Size(320, 25),
                UseSystemPasswordChar = true
            };

            loginButton = new Button
            {
                Text = "Login",
                Location = new Point(40, 190),
                Size = new Size(120, 32),
                BackColor = Color.FromArgb(65, 182, 230),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            loginButton.FlatAppearance.BorderSize = 0;
            loginButton.Click += LoginButton_Click;

            cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(240, 190),
                Size = new Size(120, 32),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            cancelButton.FlatAppearance.BorderSize = 0;
            cancelButton.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            this.Controls.Add(titleLabel);
            this.Controls.Add(emailLabel);
            this.Controls.Add(emailTextBox);
            this.Controls.Add(passwordLabel);
            this.Controls.Add(passwordTextBox);
            this.Controls.Add(loginButton);
            this.Controls.Add(cancelButton);
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(emailTextBox.Text) ||
                string.IsNullOrWhiteSpace(passwordTextBox.Text))
            {
                MessageBox.Show("Please enter both email and password.", "Missing Info");
                return;
            }

            var result = _authService.LogIn(emailTextBox.Text.Trim(), passwordTextBox.Text.Trim());

            if (!result.success)
            {
                MessageBox.Show(result.message, "Login Failed");
                return;
            }

            LoggedInUser = result.user;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}

