using SkillBuilderPro.WinForms.Models;
using SkillBuilderPro.WinForms.Services;
using SkillBuilderPro.WinForms.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SkillBuilderPro.WinForms.AdminScreens
{
    public partial class AdminDashboardForm : Form
    {
        private readonly User _user;

        private Panel profileDropdownMenu;
      

        private Panel pageAthletes;
        private Panel pageDrills;
        private Panel pageReports;

        private Panel drillLibraryBackground;
        private Panel reportsBackground;

        private Panel apiDrillsColumn;
        private Panel localDrillsColumn;

        private ComboBox drillSourceSelector;

        private List<(string Title, string Category, string Description)> drills = new()
        {
            ("Sprint Warmup", "Speed", "High‑intensity sprint warmup for acceleration."),
            ("Cone Agility", "Agility", "Quick footwork around cones."),
            ("Vertical Jump", "Power", "Explosive jump training."),
            ("Lateral Shuffle", "Defense", "Side‑to‑side movement for defensive positioning."),
            ("Endurance Run", "Conditioning", "Long‑distance stamina building."),
        };

        private DrillApiClient apiClient;
        private List<ApiDrill> apiDrills = new();

        public AdminDashboardForm(User user)
        {
            _user = user;

            InitializeComponent();

            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(1280, 800);

            this.BackgroundImage = Brand.Hero(Resource1.NewestAdminDash);
            this.BackgroundImageLayout = ImageLayout.None;

            BuildTopBar();
            BuildProfileDropdown();
           
            BuildPages();

            apiClient = new DrillApiClient("http://localhost:5000/");
            LoadApiDrillsAsync();
        }

        private void BuildTopBar()
        {
            Panel topBar = new Panel
            {
                Height = 70,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(20, 20, 20)
            };
            this.Controls.Add(topBar);

            PictureBox sbProIcon = new PictureBox
            {
                Image = Resource1.sb_pro_logo,
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(20, 7),
                Size = new Size(56, 56),
                Cursor = Cursors.Hand
            };
            topBar.Controls.Add(sbProIcon);

            Panel profileCircle = new Panel
            {
                Size = new Size(40, 40),
                Location = new Point(this.Width - 240, 15),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            profileCircle.Paint += (s, e) =>
            {
                e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(0, 120, 215)), 0, 0, 40, 40);
                TextRenderer.DrawText(e.Graphics, "BR", new Font("Segoe UI", 12, FontStyle.Bold), new Point(8, 10), Color.White);
            };
            profileCircle.Click += (s, e) => ToggleProfileDropdown();
            topBar.Controls.Add(profileCircle);

            Label profileName = new Label
            {
                Text = _user.FullName.ToUpper(),
                ForeColor = Color.White,
                Location = new Point(this.Width - 190, 18),
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            topBar.Controls.Add(profileName);

            Label profileRole = new Label
            {
                Text = _user.Role.ToUpper(),
                ForeColor = Color.Gray,
                Location = new Point(this.Width - 190, 38),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };
            topBar.Controls.Add(profileRole);
        }

       
        private void BuildProfileDropdown()
        {
            profileDropdownMenu = new Panel
            {
                Size = new Size(180, 200),   // fixed height
                BackColor = Color.FromArgb(30, 30, 30),
                Visible = false,
                Location = new Point(this.Width - 200, 70)
            };
            this.Controls.Add(profileDropdownMenu);
            profileDropdownMenu.BringToFront();

            AddDropdownItem("ATHLETES", 0);
            AddDropdownItem("DRILL LIBRARY", 40);
            AddDropdownItem("PROFILE", 80);
            AddDropdownItem("SETTINGS", 120);
            AddDropdownItem("LOGOUT", 160);
        }


        private void AddDropdownItem(string text, int y)
        {
            Panel itemPanel = new Panel
            {
                Size = new Size(180, 40),
                Location = new Point(0, y),
                BackColor = Color.FromArgb(30, 30, 30),
                Cursor = Cursors.Hand
            };

            Label itemLabel = new Label
            {
                Text = text,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = false,
                Size = new Size(180, 40),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0)
            };

            itemPanel.Controls.Add(itemLabel);

            itemPanel.Click += (s, e) =>
            {
                switch (text)
                {
                    case "ATHLETES":
                        pageAthletes.Visible = true;
                        pageDrills.Visible = false;
                        pageReports.Visible = false;
                        pageAthletes.BringToFront();
                        break;

                    case "DRILL LIBRARY":
                        pageAthletes.Visible = false;
                        pageReports.Visible = false;

                        drillLibraryBackground.Visible = true;
                        pageDrills.Visible = true;

                        drillLibraryBackground.SendToBack();
                        pageDrills.BringToFront();
                        break;

                    case "PROFILE":
                        MessageBox.Show("Profile clicked");
                        break;

                    case "SETTINGS":
                        MessageBox.Show("Settings clicked");
                        break;

                    case "LOGOUT":
                        this.Close();
                        break;
                }
            };

            profileDropdownMenu.Controls.Add(itemPanel);
        }

        private void ToggleProfileDropdown()
        {
            profileDropdownMenu.Visible = !profileDropdownMenu.Visible;
            profileDropdownMenu.BringToFront();
        }




        private void BuildPages()
        {
            pageAthletes = new Panel
            {
                Size = new Size(this.Width, this.Height - 70),
                Location = new Point(0, 70),
                BackColor = Color.FromArgb(25, 25, 25),
                Visible = false
            };
            this.Controls.Add(pageAthletes);

            pageDrills = new Panel
            {
                Size = new Size(this.Width, this.Height - 70),
                Location = new Point(0, 70),
                BackColor = Color.FromArgb(25, 25, 25),
                Visible = false
            };
            this.Controls.Add(pageDrills);

            BuildDrillLibraryPage();

            pageReports = new Panel
            {
                Size = new Size(this.Width, this.Height - 70),
                Location = new Point(0, 70),
                BackColor = Color.FromArgb(25, 25, 25),
                Visible = false
            };
            this.Controls.Add(pageReports);

            BuildReportsPage();
        }

        private void BuildDrillLibraryPage()
        {
            drillLibraryBackground = new Panel
            {
                Size = pageDrills.Size,
                Location = pageDrills.Location,
                BackgroundImage = Resource1.drill_library,
                BackgroundImageLayout = ImageLayout.Stretch,
                Visible = false
            };
            this.Controls.Add(drillLibraryBackground);
            drillLibraryBackground.SendToBack();

            pageDrills.Controls.Add(new Label
            {
                Text = "DRILL LIBRARY",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                Location = new Point(40, 20),
                AutoSize = true
            });

            drillSourceSelector = new ComboBox
            {
                Location = new Point(40, 80),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 12),
                BackColor = Color.FromArgb(40, 40, 40),
                ForeColor = Color.White
            };
            drillSourceSelector.Items.Add("API Drills");
            drillSourceSelector.Items.Add("Local Drills");
            drillSourceSelector.Items.Add("Both");
            drillSourceSelector.SelectedIndex = 2;
            drillSourceSelector.SelectedIndexChanged += DrillSourceSelector_SelectedIndexChanged;
            pageDrills.Controls.Add(drillSourceSelector);

            apiDrillsColumn = new Panel
            {
                Location = new Point(40, 140),
                Size = new Size(500, pageDrills.Height - 200),
                BackColor = Color.FromArgb(30, 30, 30),
                AutoScroll = true
            };
            pageDrills.Controls.Add(apiDrillsColumn);

            localDrillsColumn = new Panel
            {
                Location = new Point(580, 140),
                Size = new Size(500, pageDrills.Height - 200),
                BackColor = Color.FromArgb(30, 30, 30),
                AutoScroll = true
            };
            pageDrills.Controls.Add(localDrillsColumn);

            LoadApiDrillCards();
            LoadLocalDrillCards();
        }

        private void BuildReportsPage()
        {
            reportsBackground = new Panel
            {
                Size = pageReports.Size,
                Location = pageReports.Location,
                BackgroundImage = Resource1.weight_room,
                BackgroundImageLayout = ImageLayout.Stretch,
                Visible = false
            };
            this.Controls.Add(reportsBackground);
            reportsBackground.SendToBack();

            pageReports.Controls.Add(new Label
            {
                Text = "REPORTS",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                Location = new Point(40, 20),
                AutoSize = true
            });

            pageReports.Controls.Add(new Label
            {
                Text = "Reports dashboard coming soon...",
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 14),
                Location = new Point(40, 80),
                AutoSize = true
            });
        }

        private async void LoadApiDrillsAsync()
        {
            apiDrills = await apiClient.GetAllDrillsAsync();
            LoadApiDrillCards();
        }

        private void LoadApiDrillCards()
        {
            apiDrillsColumn.Controls.Clear();
            int y = 10;

            foreach (var drill in apiDrills)
            {
                var card = new Button
                {
                    Text = drill.Title,
                    Location = new Point(10, y),
                    Size = new Size(460, 80),
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    BackColor = Color.FromArgb(40, 40, 40),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };

                card.FlatAppearance.BorderSize = 0;

                card.Click += (s, e) => OpenDrillVideo(drill.VideoUrl);

                apiDrillsColumn.Controls.Add(card);
                y += 90;
            }
        }

        private void LoadLocalDrillCards()
        {
            localDrillsColumn.Controls.Clear();
            int y = 10;

            foreach (var drill in drills)
            {
                var cardPanel = new Panel
                {
                    Location = new Point(10, y),
                    Size = new Size(460, 100),
                    BackColor = Color.FromArgb(40, 40, 40)
                };

                cardPanel.Controls.Add(new Label
                {
                    Text = $"{drill.Title} — {drill.Category}",
                    Location = new Point(10, 10),
                    Size = new Size(440, 25),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 12, FontStyle.Bold)
                });

                var btnView = new Button
                {
                    Text = "View",
                    Location = new Point(10, 50),
                    Size = new Size(80, 35),
                    BackColor = Color.FromArgb(60, 60, 60),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnView.Click += (s, e) => MessageBox.Show(drill.Description, drill.Title);
                cardPanel.Controls.Add(btnView);

                var btnEdit = new Button
                {
                    Text = "Edit",
                    Location = new Point(100, 50),
                    Size = new Size(80, 35),
                    BackColor = Color.FromArgb(80, 80, 80),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnEdit.Click += (s, e) => EditLocalDrill(drill);
                cardPanel.Controls.Add(btnEdit);

                var btnDelete = new Button
                {
                    Text = "Delete",
                    Location = new Point(190, 50),
                    Size = new Size(80, 35),
                    BackColor = Color.FromArgb(120, 40, 40),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnDelete.Click += (s, e) => DeleteLocalDrill(drill);
                cardPanel.Controls.Add(btnDelete);

                localDrillsColumn.Controls.Add(cardPanel);
                y += 110;
            }
        }

        private void EditLocalDrill((string Title, string Category, string Description) drill)
        {
            var form = new Form
            {
                Text = "Edit Drill",
                Size = new Size(400, 400),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.FromArgb(30, 30, 30)
            };

            var txtTitle = new TextBox { Text = drill.Title, Location = new Point(20, 20), Width = 340 };
            var txtCategory = new TextBox { Text = drill.Category, Location = new Point(20, 70), Width = 340 };
            var txtDescription = new TextBox { Text = drill.Description, Location = new Point(20, 120), Width = 340, Height = 100, Multiline = true };

            var btnSave = new Button
            {
                Text = "Save Changes",
                Location = new Point(20, 300),
                Size = new Size(340, 40),
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            btnSave.Click += (s, e) =>
            {
                drills.Remove(drill);
                drills.Add((txtTitle.Text, txtCategory.Text, txtDescription.Text));
                LoadLocalDrillCards();
                form.Close();
            };

            form.Controls.Add(txtTitle);
            form.Controls.Add(txtCategory);
            form.Controls.Add(txtDescription);
            form.Controls.Add(btnSave);

            form.ShowDialog();
        }

        private void DeleteLocalDrill((string Title, string Category, string Description) drill)
        {
            var result = MessageBox.Show(
                $"Delete drill '{drill.Title}'?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                drills.Remove(drill);
                LoadLocalDrillCards();
            }
        }

        private void OpenDrillVideo(string url)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch
            {
                MessageBox.Show("Unable to open video.");
            }
        }

        private void DrillSourceSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (drillSourceSelector.SelectedItem == null)
                return;

            string selected = drillSourceSelector.SelectedItem.ToString();

            switch (selected)
            {
                case "API Drills":
                    apiDrillsColumn.Visible = true;
                    localDrillsColumn.Visible = false;
                    break;

                case "Local Drills":
                    apiDrillsColumn.Visible = false;
                    localDrillsColumn.Visible = true;
                    break;

                case "Both":
                    apiDrillsColumn.Visible = true;
                    localDrillsColumn.Visible = true;
                    break;
            }
        }
    }
}
