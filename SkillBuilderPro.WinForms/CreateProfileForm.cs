using System;
using System.Drawing;
using System.Windows.Forms;
using SkillBuilderPro.WinForms.Models;

namespace SkillBuilderPro.WinForms
{
    public partial class CreateProfileForm : Form
    {
        public User CreatedUser { get; private set; }

        private Label titleLabel;
        private Label nameLabel;
        private Label emailLabel;
        private Label passwordLabel;
        private Label phoneLabel;
        private Label sportLabel;
        private Label focusLabel;

        private TextBox nameTextBox;
        private TextBox emailTextBox;
        private TextBox passwordTextBox;
        private TextBox phoneTextBox;

        private ComboBox sportComboBox;
        private ComboBox focusComboBox;

        private Button createButton;
        private Button cancelButton;

        public CreateProfileForm()
        {
            InitializeCreateProfileForm();
            BuildLayout();
        }

        private void InitializeCreateProfileForm()
        {
            this.SuspendLayout();
            this.ClientSize = new Size(450, 460);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Create Profile";
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
                Text = "Create Athlete Profile",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, 15),
                Size = new Size(450, 30)
            };

            nameLabel = new Label
            {
                Text = "Full Name",
                ForeColor = Color.White,
                Location = new Point(40, 60),
                AutoSize = true
            };

            nameTextBox = new TextBox
            {
                Location = new Point(40, 80),
                Size = new Size(360, 25)
            };

            emailLabel = new Label
            {
                Text = "Email",
                ForeColor = Color.White,
                Location = new Point(40, 115),
                AutoSize = true
            };

            emailTextBox = new TextBox
            {
                Location = new Point(40, 135),
                Size = new Size(360, 25)
            };

            passwordLabel = new Label
            {
                Text = "Password",
                ForeColor = Color.White,
                Location = new Point(40, 170),
                AutoSize = true
            };

            passwordTextBox = new TextBox
            {
                Location = new Point(40, 190),
                Size = new Size(360, 25),
                UseSystemPasswordChar = true
            };

            phoneLabel = new Label
            {
                Text = "Phone Number",
                ForeColor = Color.White,
                Location = new Point(40, 225),
                AutoSize = true
            };

            phoneTextBox = new TextBox
            {
                Location = new Point(40, 245),
                Size = new Size(360, 25)
            };

            sportLabel = new Label
            {
                Text = "Sport",
                ForeColor = Color.White,
                Location = new Point(40, 280),
                AutoSize = true
            };

            sportComboBox = new ComboBox
            {
                Location = new Point(40, 300),
                Size = new Size(180, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            sportComboBox.Items.AddRange(new object[]
            {
                "Basketball",
                "Baseball",
                "Softball",
                "Football",
                "Soccer"
            });
            sportComboBox.SelectedIndexChanged += SportComboBox_SelectedIndexChanged;

            focusLabel = new Label
            {
                Text = "Training Focus",
                ForeColor = Color.White,
                Location = new Point(230, 280),
                AutoSize = true
            };

            focusComboBox = new ComboBox
            {
                Location = new Point(230, 300),
                Size = new Size(170, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            createButton = new Button
            {
                Text = "Create Profile",
                Location = new Point(40, 360),
                Size = new Size(150, 36),
                BackColor = Color.FromArgb(200, 16, 46),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            createButton.FlatAppearance.BorderSize = 0;
            createButton.Click += CreateButton_Click;

            cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(250, 360),
                Size = new Size(150, 36),
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
            this.Controls.Add(nameLabel);
            this.Controls.Add(nameTextBox);
            this.Controls.Add(emailLabel);
            this.Controls.Add(emailTextBox);
            this.Controls.Add(passwordLabel);
            this.Controls.Add(passwordTextBox);
            this.Controls.Add(phoneLabel);
            this.Controls.Add(phoneTextBox);
            this.Controls.Add(sportLabel);
            this.Controls.Add(sportComboBox);
            this.Controls.Add(focusLabel);
            this.Controls.Add(focusComboBox);
            this.Controls.Add(createButton);
            this.Controls.Add(cancelButton);
        }

        private void SportComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            focusComboBox.Items.Clear();

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
                    break;

                case "Baseball":
                case "Softball":
                    focusComboBox.Items.AddRange(new object[]
                    {
                        "Hitting", "Fielding", "Throwing", "Base Running"
                    });
                    break;

                case "Football":
                    focusComboBox.Items.AddRange(new object[]
                    {
                        "Passing", "Route Running", "Blocking", "Tackling"
                    });
                    break;

                case "Soccer":
                    focusComboBox.Items.AddRange(new object[]
                    {
                        "Passing", "Dribbling", "Shooting", "Defense"
                    });
                    break;
            }

            if (focusComboBox.Items.Count > 0)
                focusComboBox.SelectedIndex = 0;
        }

        private void CreateButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nameTextBox.Text) ||
                string.IsNullOrWhiteSpace(emailTextBox.Text) ||
                string.IsNullOrWhiteSpace(passwordTextBox.Text) ||
                sportComboBox.SelectedItem == null ||
                focusComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please fill out all fields.", "Missing Info");
                return;
            }

            CreatedUser = new User
            {
                FullName = nameTextBox.Text.Trim(),
                Email = emailTextBox.Text.Trim(),
                Password = passwordTextBox.Text.Trim(),
                Phone = phoneTextBox.Text.Trim(),
                Sport = sportComboBox.SelectedItem.ToString(),
                TargetArea = focusComboBox.SelectedItem.ToString(),
                ExperienceLevel = "Beginner",
                Role = "Athlete",
                IsActive = true
            };

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}

