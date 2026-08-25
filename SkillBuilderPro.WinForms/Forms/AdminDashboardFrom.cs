using SkillBuilderPro.WinForms.Models;
using SkillBuilderPro.WinForms.Services;
using SkillBuilderPro.WinForms.Properties;
using SkillBuilderPro.Client.ApiClients;
using SkillBuilderPro.Client.Services;
using SkillBuilderPro.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net.Http;
using System.Windows.Forms;
using WinFormsUser = SkillBuilderPro.WinForms.Models.User;
using CoreDrill = SkillBuilderPro.Core.Models.Drill;
using DrillApiClient = SkillBuilderPro.Client.ApiClients.DrillApiClient;

namespace SkillBuilderPro.WinForms.AdminScreens
{
    public partial class AdminDashboardForm : Form
    {
        private readonly WinFormsUser _user;

        private Panel profileDropdownMenu;

        private Panel pageAthletes;
        private Panel pageDrills;
        private Panel pageReports;
        private Panel commandCenter;
        private Label commandCenterTitle;
        private Label commandCenterSubtitle;
        private readonly List<Button> commandCenterButtons = new();

        private Panel drillLibraryBackground;
        private Panel reportsBackground;

        private Panel apiDrillsColumn;
        private Panel localDrillsColumn;

        private ComboBox drillSourceSelector;

        private List<(string Title, string Category, string Description)> drills = new()
        {
            ("Sprint Warmup", "Speed", "High-intensity sprint warmup for acceleration."),
            ("Cone Agility", "Agility", "Quick footwork around cones."),
            ("Vertical Jump", "Power", "Explosive jump training."),
            ("Lateral Shuffle", "Defense", "Side-to-side movement for defensive positioning."),
            ("Endurance Run", "Conditioning", "Long-distance stamina building."),
        };

        private readonly DrillApiClient apiClient;
        private List<CoreDrill> apiDrills = new();

        public AdminDashboardForm(WinFormsUser user)
        {
            _user = user;

            InitializeComponent();

            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1280, 800);

            BackgroundImage = Brand.Hero(DesktopVisualResolver.Current.GetAdministratorBackground());
            BackgroundImageLayout = ImageLayout.Zoom;

            BuildPages();
            BuildCommandCenter();
            BuildTopBar();
            BuildProfileDropdown();

            var http = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5000/")
            };

            IApiClient api = new ApiClient(http);
            apiClient = new DrillApiClient(api);

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
            Controls.Add(topBar);

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
                Location = new Point(Width - 240, 15),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            profileCircle.Paint += (s, e) =>
            {
                e.Graphics.FillEllipse(new SolidBrush(Color.FromArgb(0, 120, 215)), 0, 0, 40, 40);
                TextRenderer.DrawText(
                    e.Graphics,
                    "BR",
                    new Font("Segoe UI", 12, FontStyle.Bold),
                    new Point(8, 10),
                    Color.White);
            };
            profileCircle.Click += (s, e) => ToggleProfileDropdown();
            topBar.Controls.Add(profileCircle);

            Label profileName = new Label
            {
                Text = _user.FullName.ToUpper(),
                ForeColor = Color.White,
                Location = new Point(Width - 190, 18),
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            topBar.Controls.Add(profileName);

            Label profileRole = new Label
            {
                Text = _user.Role.ToUpper(),
                ForeColor = Color.Gray,
                Location = new Point(Width - 190, 38),
                AutoSize = true,
                Font = new Font("Segoe UI", 9)
            };
            topBar.Controls.Add(profileRole);
        }

        private void BuildProfileDropdown()
        {
            profileDropdownMenu = new Panel
            {
                Size = new Size(180, 240),
                BackColor = Color.FromArgb(30, 30, 30),
                Visible = false,
                Location = new Point(Width - 200, 70)
            };
            Controls.Add(profileDropdownMenu);
            profileDropdownMenu.BringToFront();

            AddDropdownItem("COMMAND CENTER", 0);
            AddDropdownItem("ATHLETES", 40);
            AddDropdownItem("DRILL LIBRARY", 80);
            AddDropdownItem("PROFILE", 120);
            AddDropdownItem("SETTINGS", 160);
            AddDropdownItem("LOGOUT", 200);
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
                Padding = new Padding(15, 0, 0, 0),
                Cursor = Cursors.Hand
            };

            EventHandler clickHandler = (s, e) => HandleDropdownSelection(text);

            itemPanel.Click += clickHandler;
            itemLabel.Click += clickHandler;

            itemPanel.Controls.Add(itemLabel);
            profileDropdownMenu.Controls.Add(itemPanel);
        }

        private void HandleDropdownSelection(string text)
        {
            switch (text)
            {
                case "COMMAND CENTER":
                    pageAthletes.Visible = false;
                    pageDrills.Visible = false;
                    pageReports.Visible = false;
                    drillLibraryBackground.Visible = false;
                    reportsBackground.Visible = false;
                    commandCenter.Visible = true;
                    commandCenter.BringToFront();
                    break;

                case "ATHLETES":
                    commandCenter.Visible = false;
                    pageAthletes.Visible = true;
                    pageDrills.Visible = false;
                    pageReports.Visible = false;
                    pageAthletes.BringToFront();
                    break;

                case "DRILL LIBRARY":
                    commandCenter.Visible = false;
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
                    Close();
                    break;
            }

            profileDropdownMenu.Visible = false;
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
                Size = new Size(Width, Height - 70),
                Location = new Point(0, 70),
                BackColor = Color.FromArgb(25, 25, 25),
                Visible = false
            };
            Controls.Add(pageAthletes);

            pageDrills = new Panel
            {
                Size = new Size(Width, Height - 70),
                Location = new Point(0, 70),
                BackColor = Color.FromArgb(25, 25, 25),
                Visible = false
            };
            Controls.Add(pageDrills);

            BuildDrillLibraryPage();

            pageReports = new Panel
            {
                Size = new Size(Width, Height - 70),
                Location = new Point(0, 70),
                BackColor = Color.FromArgb(25, 25, 25),
                Visible = false
            };
            Controls.Add(pageReports);

            BuildReportsPage();
        }

        private void BuildCommandCenter()
        {
            commandCenter = new Panel
            {
                Location = new Point(0, 70),
                Size = new Size(Width, Height - 70),
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            Controls.Add(commandCenter);

            commandCenterTitle = new Label
            {
                Text = "ADMIN COMMAND CENTER",
                ForeColor = Brand.TextStrong,
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            commandCenter.Controls.Add(commandCenterTitle);
            commandCenterSubtitle = new Label
            {
                Text = "PLATFORM OPERATIONS  •  PERFORMANCE  •  OVERSIGHT",
                ForeColor = Brand.Muted,
                Font = new Font("Segoe UI Semibold", 11F),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            commandCenter.Controls.Add(commandCenterSubtitle);

            string[] modules =
            {
                "USER MANAGEMENT", "DRILL MANAGEMENT", "GOALS & PROGRESSION", "TRAINING WORKFLOWS",
                "ANALYTICS & REPORTS", "SYSTEM HEALTH", "AUDIT LOGS", "SETTINGS"
            };
            for (int index = 0; index < modules.Length; index++)
            {
                string module = modules[index];
                var button = new Button
                {
                    Text = module,
                    Size = new Size(250, 56),
                    BackColor = Color.FromArgb(150, 35, 42, 54),
                    ForeColor = Brand.TextCell,
                    FlatStyle = FlatStyle.Flat,
                    Font = Brand.Btn,
                    Cursor = Cursors.Hand
                };
                button.FlatAppearance.BorderColor = Brand.Steel;
                button.FlatAppearance.BorderSize = 1;
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(210, 38, 64, 92);
                button.Click += (s, e) => OpenAdminModule(module);
                commandCenter.Controls.Add(button);
                commandCenterButtons.Add(button);
            }

            commandCenter.Resize += (s, e) => LayoutCommandCenter();
            LayoutCommandCenter();
        }

        private void LayoutCommandCenter()
        {
            const int buttonWidth = 250;
            const int buttonHeight = 56;
            const int columnGap = 24;
            const int rowGap = 16;
            int gridWidth = buttonWidth * 2 + columnGap;
            int gridLeft = Math.Max((commandCenter.ClientSize.Width - gridWidth) / 2, 24);

            commandCenterTitle.Location = new Point(
                Math.Max((commandCenter.ClientSize.Width - commandCenterTitle.PreferredWidth) / 2, 24), 34);
            commandCenterSubtitle.Location = new Point(
                Math.Max((commandCenter.ClientSize.Width - commandCenterSubtitle.PreferredWidth) / 2, 24), 88);

            for (int index = 0; index < commandCenterButtons.Count; index++)
            {
                commandCenterButtons[index].Bounds = new Rectangle(
                    gridLeft + (index % 2) * (buttonWidth + columnGap),
                    150 + (index / 2) * (buttonHeight + rowGap),
                    buttonWidth,
                    buttonHeight);
            }
        }

        private void OpenAdminModule(string module)
        {
            commandCenter.Visible = false;
            switch (module)
            {
                case "USER MANAGEMENT":
                    pageAthletes.Visible = true;
                    pageAthletes.BringToFront();
                    break;
                case "DRILL MANAGEMENT":
                    drillLibraryBackground.Visible = true;
                    pageDrills.Visible = true;
                    drillLibraryBackground.SendToBack();
                    pageDrills.BringToFront();
                    break;
                case "ANALYTICS & REPORTS":
                    reportsBackground.Visible = true;
                    pageReports.Visible = true;
                    reportsBackground.SendToBack();
                    pageReports.BringToFront();
                    break;
                default:
                    commandCenter.Visible = true;
                    MessageBox.Show(
                        "This dedicated administrator workspace is not implemented yet.",
                        module,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    break;
            }
        }

        private void BuildDrillLibraryPage()
        {
            drillLibraryBackground = new Panel
            {
                Size = pageDrills.Size,
                Location = pageDrills.Location,
                BackgroundImage = Resource1.drill_library,
                BackgroundImageLayout = ImageLayout.Zoom,
                Visible = false
            };
            Controls.Add(drillLibraryBackground);
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
                BackgroundImageLayout = ImageLayout.Zoom,
                Visible = false
            };
            Controls.Add(reportsBackground);
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
            try
            {
                apiDrills = await apiClient.GetAllAsync() ?? new List<CoreDrill>();
                LoadApiDrillCards();
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show(
                    $"Could not load API drills.\n\n{ex.Message}",
                    "API Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Unexpected error loading drills.\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void LoadApiDrillCards()
        {
            apiDrillsColumn.Controls.Clear();
            int y = 10;

            foreach (var drill in apiDrills)
            {
                var card = new Button
                {
                    Text = string.IsNullOrWhiteSpace(drill.Name) ? "(Unnamed Drill)" : drill.Name,
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

        private void OpenDrillVideo(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                MessageBox.Show("This drill does not have a video URL.");
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo
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

            string selected = drillSourceSelector.SelectedItem.ToString() ?? "Both";

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
