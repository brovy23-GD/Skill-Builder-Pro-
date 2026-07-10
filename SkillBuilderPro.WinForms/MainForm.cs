using SkillBuilderPro.WinForms.Models;
using SkillBuilderPro.WinForms.Properties;
using SkillBuilderPro.WinForms.Services;
using SkillBuilderPro.WinForms.Theming;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SkillBuilderPro.WinForms
{
    /// <summary>
    /// MainForm - dashboard after login.
    /// Header + custom nav bar + Training / Goals / Calendar tabs.
    /// ATHLETE PROFILE opens the standalone LockerRoomForm.
    /// </summary>
    public partial class MainForm : Form
    {
        private readonly User _user;
        private readonly bool _isDemoMode;

        // Layout
        private Panel headerPanel;
        private Panel navPanel;
        private TabControl mainTabControl;
        private readonly List<Button> navButtons = new List<Button>();

        // Training tab
        private ComboBox focusComboBox;
        private CheckedListBox drillCheckedListBox;
        private ListBox scheduleListBox;
        private Button generateScheduleButton;
        private Button clearSelectionButton;
        private Button generateVideoButton;
        private Panel scheduleCard;   // right-side preview; hidden until generated
        private List<Drill> currentSportDrills = new List<Drill>();

        // Professional dark palette
        private static class AppColors
        {
            public static readonly Color TrainingBackground = Color.FromArgb(20, 24, 32);
            public static readonly Color TrainingCard = Color.FromArgb(32, 38, 50);
            public static readonly Color Shelf = Color.FromArgb(42, 49, 63);
            public static readonly Color TrainingText = Color.FromArgb(230, 235, 240);
            public static readonly Color SubtleText = Color.FromArgb(155, 170, 190);
        }

        public MainForm(User user) : this(user, false) { }

        public MainForm(User user, bool isDemoMode)
        {
            _user = user;
            _isDemoMode = isDemoMode;

            InitializeComponent();
            SetupForm();
            BuildHeader();
            BuildTabControl();
            BuildNavBar();

            // Dock order: last added is laid out first - header on top,
            // nav under it, tabs filling the rest.
            this.Controls.Add(mainTabControl);
            this.Controls.Add(navPanel);
            this.Controls.Add(headerPanel);

            SelectNavTab(1); // land on Training
        }

        private void SetupForm()
        {
            this.Text = "SkillBuilderPro";
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(1200, 800);
            this.BackColor = AppColors.TrainingBackground;
        }

        // ------------------------------
        // HEADER
        // ------------------------------

        private void BuildHeader()
        {
            var theme = TeamThemes.GetThemeForSport(_user.Sport);

            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 84,
                BackColor = theme.Panel
            };

            headerPanel.Controls.Add(new Label
            {
                Text = "SKILL BUILDER PRO",
                ForeColor = theme.Text,
                Font = new Font("Segoe UI", 19F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 12)
            });

            headerPanel.Controls.Add(new Label
            {
                Text = $"Welcome {_user.FullName}   |   Sport: {_user.Sport}   |   Focus: {_user.TargetArea}",
                ForeColor = theme.Text,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(22, 50)
            });

            Button exitButton = new Button
            {
                Text = _isDemoMode ? "EXIT DEMO" : "LOG OUT",
                Size = new Size(130, 40),
                BackColor = theme.Accent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            exitButton.FlatAppearance.BorderSize = 0;
            exitButton.Click += (s, e) => this.Close();
            headerPanel.Controls.Add(exitButton);

            headerPanel.Resize += (s, e) =>
            {
                exitButton.Location = new Point(headerPanel.Width - 154, 22);
            };
        }

        // ------------------------------
        // NAV BAR + TABS
        // ------------------------------

        private void BuildTabControl()
        {
            mainTabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            // Hide the native tab headers - the custom nav bar navigates.
            mainTabControl.Appearance = TabAppearance.FlatButtons;
            mainTabControl.ItemSize = new Size(0, 1);
            mainTabControl.SizeMode = TabSizeMode.Fixed;

            TabPage profileTab = new TabPage("Athlete Profile");
            TabPage trainingTab = new TabPage("Training");
            TabPage goalsTab = new TabPage("Goals");
            TabPage calendarTab = new TabPage("Calendar");

            SetupProfileLandingTab(profileTab);
            SetupTrainingTab(trainingTab);
            SetupGoalsTab(goalsTab);
            SetupCalendarTab(calendarTab);

            mainTabControl.TabPages.Add(profileTab);
            mainTabControl.TabPages.Add(trainingTab);
            mainTabControl.TabPages.Add(goalsTab);
            mainTabControl.TabPages.Add(calendarTab);
        }

        private void BuildNavBar()
        {
            navPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 52,
                BackColor = AppColors.TrainingCard
            };

            string[] sections = { "ATHLETE PROFILE", "TRAINING", "GOALS", "CALENDAR" };

            int x = 20;
            for (int i = 0; i < sections.Length; i++)
            {
                int index = i;

                Button navBtn = new Button
                {
                    Text = sections[i],
                    Width = i == 0 ? 190 : 140,
                    Height = 52,
                    Location = new Point(x, 0),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = AppColors.TrainingCard,
                    ForeColor = AppColors.SubtleText,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    Cursor = Cursors.Hand,
                    TabStop = false
                };
                navBtn.FlatAppearance.BorderSize = 0;
                navBtn.FlatAppearance.MouseOverBackColor = AppColors.Shelf;

                // ATHLETE PROFILE opens the standalone locker room;
                // everything else switches tabs.
                if (sections[i] == "ATHLETE PROFILE")
                    navBtn.Click += (s, e) => OpenAthleteProfile();
                else
                    navBtn.Click += (s, e) => SelectNavTab(index);

                navButtons.Add(navBtn);
                navPanel.Controls.Add(navBtn);
                x += navBtn.Width + 4;
            }
        }

        private void OpenAthleteProfile()
        {
            LockerRoomForm lockerForm = new LockerRoomForm(_user);
            lockerForm.Show();
        }

        private void SelectNavTab(int index)
        {
            var theme = TeamThemes.GetThemeForSport(_user.Sport);

            mainTabControl.SelectedIndex = index;

            for (int i = 0; i < navButtons.Count; i++)
            {
                bool selected = i == index;
                navButtons[i].BackColor = selected ? theme.Accent : AppColors.TrainingCard;
                navButtons[i].ForeColor = selected ? Color.White : AppColors.SubtleText;
            }
        }

        // ------------------------------
        // SHARED CARD + LABEL HELPERS
        // ------------------------------

        private Panel CreateCardPanel(TabPage tab, int width, int height, int topMargin = 24)
        {
            Panel card = new Panel
            {
                Size = new Size(width, height),
                BackColor = AppColors.TrainingCard
            };
            card.Location = new Point(Math.Max((tab.Width - width) / 2, 0), topMargin);
            tab.Resize += (s, e) =>
            {
                card.Location = new Point(Math.Max((tab.Width - width) / 2, 0), topMargin);
            };
            tab.Controls.Add(card);
            card.BringToFront();
            return card;
        }

        private Label CreateCardLabel(Panel card, string text, float fontSize, FontStyle style,
                                      Color foreColor, int y, int height, Color? backColor = null)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", fontSize, style),
                ForeColor = foreColor,
                BackColor = backColor ?? Color.Transparent,
                AutoSize = false,
                Width = card.Width - 40,
                Height = height,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, y)
            };
        }

        private void CenterInCard(Panel card, Control control, int y)
        {
            control.Location = new Point((card.Width - control.Width) / 2, y);
        }

        private Button CreateModernButton(string text, int width = 220, int height = 42)
        {
            var theme = TeamThemes.GetThemeForSport(_user.Sport);

            Button btn = new Button
            {
                Text = text,
                Width = width,
                Height = height,
                FlatStyle = FlatStyle.Flat,
                BackColor = theme.Accent,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            btn.FlatAppearance.BorderSize = 0;

            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(
                Math.Min(theme.Accent.R + 20, 255),
                Math.Min(theme.Accent.G + 20, 255),
                Math.Min(theme.Accent.B + 20, 255));
            btn.MouseLeave += (s, e) => btn.BackColor = theme.Accent;

            return btn;
        }

        // ------------------------------
        // ATHLETE PROFILE LANDING TAB
        // ------------------------------

        private void SetupProfileLandingTab(TabPage tab)
        {
            SetTabBackground(tab);

            Panel card = CreateCardPanel(tab, 420, 190, 60);

            Label msg = CreateCardLabel(card,
                "Your profile lives in the locker room.",
                13F, FontStyle.Bold, AppColors.TrainingText, 26, 30);

            Button enterButton = CreateModernButton("ENTER LOCKER ROOM", 280, 44);
            CenterInCard(card, enterButton, 96);
            enterButton.Click += (s, e) => OpenAthleteProfile();

            card.Controls.Add(msg);
            card.Controls.Add(enterButton);
        }

        // ------------------------------
        // TRAINING TAB - builder left, preview right (edge-pinned)
        // ------------------------------

        private void SetupTrainingTab(TabPage tab)
        {
            SetTabBackground(tab);
            tab.AutoScroll = true;

            const int leftWidth = 400;
            const int leftHeight = 560;
            const int rightWidth = 400;
            const int rightHeight = 560;
            const int gap = 28;
            const int topMargin = 36;

            Panel leftCard = new Panel
            {
                Size = new Size(leftWidth, leftHeight),
                BackColor = AppColors.TrainingCard
            };
            Panel rightCard = new Panel
            {
                Size = new Size(rightWidth, rightHeight),
                BackColor = AppColors.TrainingCard,
                Visible = false            // hidden until a schedule is generated
            };
            scheduleCard = rightCard;

            void PositionCards()
            {
                // Pin cards to the outer edges so the center-court branding
                // in the background stays fully visible between them.
                const int edgeMargin = 40;
                leftCard.Location = new Point(edgeMargin, topMargin);
                rightCard.Location = new Point(Math.Max(tab.Width - rightWidth - edgeMargin, leftWidth + edgeMargin + gap), topMargin);
            }
            PositionCards();
            tab.Resize += (s, e) => PositionCards();

            // ----- LEFT: TRAINING BUILDER -----

            Label title = CreateCardLabel(leftCard, "Training Builder",
                18F, FontStyle.Bold, AppColors.TrainingText, 16, 34);

            Label focusLabel = CreateCardLabel(leftCard, "Training Focus",
                11F, FontStyle.Bold, AppColors.SubtleText, 58, 22);

            focusComboBox = new ComboBox
            {
                Width = 340,
                Height = 30,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 11F),
                BackColor = AppColors.Shelf,
                ForeColor = AppColors.TrainingText,
                FlatStyle = FlatStyle.Flat
            };
            CenterInCard(leftCard, focusComboBox, 84);
            focusComboBox.SelectedIndexChanged += (s, e) => LoadDrillsForSelectedFocus();

            Label drillLabel = CreateCardLabel(leftCard, "Check One or More Drills",
                11F, FontStyle.Bold, AppColors.SubtleText, 124, 22);

            drillCheckedListBox = new CheckedListBox
            {
                Width = 340,
                Height = 250,
                Font = new Font("Segoe UI", 11F),
                BackColor = AppColors.Shelf,
                ForeColor = AppColors.TrainingText,
                BorderStyle = BorderStyle.None,
                CheckOnClick = true
            };
            CenterInCard(leftCard, drillCheckedListBox, 150);

            generateScheduleButton = CreateModernButton("GENERATE SCHEDULE", 340, 38);
            clearSelectionButton = CreateModernButton("CLEAR DRILLS", 164, 38);
            generateVideoButton = CreateModernButton("TRAINING VIDEO", 164, 38);

            CenterInCard(leftCard, generateScheduleButton, 420);
            clearSelectionButton.Location = new Point(30, 468);
            generateVideoButton.Location = new Point(206, 468);

            generateScheduleButton.Click += GenerateScheduleButton_Click;
            clearSelectionButton.Click += ClearSelectionButton_Click;
            generateVideoButton.Click += (s, e) => MessageBox.Show(
                "In a future version, this will generate a customized training video " +
                "based on the drills and schedule you’ve created.",
                "Generate Training Video (Coming Soon)");

            leftCard.Controls.Add(title);
            leftCard.Controls.Add(focusLabel);
            leftCard.Controls.Add(focusComboBox);
            leftCard.Controls.Add(drillLabel);
            leftCard.Controls.Add(drillCheckedListBox);
            leftCard.Controls.Add(generateScheduleButton);
            leftCard.Controls.Add(clearSelectionButton);
            leftCard.Controls.Add(generateVideoButton);

            // ----- RIGHT: SCHEDULE PREVIEW (hidden until generated) -----

            Label previewTitle = CreateCardLabel(rightCard, "Schedule Preview",
                18F, FontStyle.Bold, AppColors.TrainingText, 16, 34);

            scheduleListBox = new ListBox
            {
                Width = 340,
                Height = 470,
                Font = new Font("Consolas", 10F),
                BackColor = AppColors.Shelf,
                ForeColor = AppColors.TrainingText,
                BorderStyle = BorderStyle.None
            };
            CenterInCard(rightCard, scheduleListBox, 62);

            rightCard.Controls.Add(previewTitle);
            rightCard.Controls.Add(scheduleListBox);

            tab.Controls.Add(leftCard);
            tab.Controls.Add(rightCard);
            leftCard.BringToFront();
            rightCard.BringToFront();

            LoadTrainingFocusOptions();
            LoadDrillsForSelectedFocus();
        }

        // ------------------------------
        // GOALS TAB
        // ------------------------------

        private void SetupGoalsTab(TabPage tab)
        {
            SetTabBackground(tab);

            Panel card = CreateCardPanel(tab, 480, 400, 36);

            Label title = CreateCardLabel(card, "Goals",
                20F, FontStyle.Bold, AppColors.TrainingText, 20, 40);

            Label goalText = CreateCardLabel(card,
                "Current Goal Ideas\n\n" +
                "- Improve consistency in your primary focus area.\n" +
                "- Complete 3 training sessions this week.\n" +
                "- Build a routine around skill repetition.\n" +
                "- Review your schedule each weekend.",
                12F, FontStyle.Regular, AppColors.TrainingText, 80, 180, AppColors.Shelf);

            Button addGoalButton = CreateModernButton("ADD GOAL");
            CenterInCard(card, addGoalButton, 292);

            card.Controls.Add(title);
            card.Controls.Add(goalText);
            card.Controls.Add(addGoalButton);
        }

        // ------------------------------
        // CALENDAR TAB - custom month grid, training days in team color
        // ------------------------------

        private DateTime calendarMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        private Panel calendarGrid;
        private Label calendarMonthLabel;

        private void SetupCalendarTab(TabPage tab)
        {
            // Sport-specific calendar background (calendar_* image set)
            Image calBg = GetCalendarBackground(_user.Sport);
            if (calBg != null)
            {
                tab.BackgroundImage = ApplyDarkOverlay(calBg, 0.5f);
                tab.BackgroundImageLayout = ImageLayout.Stretch;
            }

            var theme = TeamThemes.GetThemeForSport(_user.Sport);

            Panel card = CreateCardPanel(tab, 700, 620, 30);

            // Month header with prev / next
            calendarMonthLabel = CreateCardLabel(card, "", 18F, FontStyle.Bold, AppColors.TrainingText, 18, 34);

            Button prevBtn = CreateModernButton("<", 44, 34);
            prevBtn.Location = new Point(24, 18);
            prevBtn.Click += (s, e) => { calendarMonth = calendarMonth.AddMonths(-1); RenderCalendar(); };

            Button nextBtn = CreateModernButton(">", 44, 34);
            nextBtn.Location = new Point(card.Width - 68, 18);
            nextBtn.Click += (s, e) => { calendarMonth = calendarMonth.AddMonths(1); RenderCalendar(); };

            // Day-of-week header row
            string[] dayNames = { "SUN", "MON", "TUE", "WED", "THU", "FRI", "SAT" };
            for (int i = 0; i < 7; i++)
            {
                card.Controls.Add(new Label
                {
                    Text = dayNames[i],
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    ForeColor = AppColors.SubtleText,
                    BackColor = Color.Transparent,
                    AutoSize = false,
                    Size = new Size(90, 22),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Location = new Point(24 + i * 94, 62)
                });
            }

            // Grid host - repainted per month
            calendarGrid = new Panel
            {
                Size = new Size(662, 452),
                Location = new Point(24, 88),
                BackColor = Color.Transparent
            };
            card.Controls.Add(calendarGrid);

            Label legend = CreateCardLabel(card,
                "Team-color days = your training schedule (Mon / Wed / Fri)",
                10F, FontStyle.Regular, AppColors.SubtleText, 552, 24);

            card.Controls.Add(calendarMonthLabel);
            card.Controls.Add(prevBtn);
            card.Controls.Add(nextBtn);
            card.Controls.Add(legend);

            RenderCalendar();
        }

        /// <summary>
        /// Draws the month: one tile per day; Mon/Wed/Fri tiles fill with the
        /// team accent (training days), today gets a highlighted border.
        /// </summary>
        private void RenderCalendar()
        {
            var theme = TeamThemes.GetThemeForSport(_user.Sport);

            calendarMonthLabel.Text = calendarMonth.ToString("MMMM yyyy").ToUpper();
            calendarGrid.Controls.Clear();

            int daysInMonth = DateTime.DaysInMonth(calendarMonth.Year, calendarMonth.Month);
            int startCol = (int)calendarMonth.DayOfWeek;   // Sunday = 0

            for (int day = 1; day <= daysInMonth; day++)
            {
                DateTime date = new DateTime(calendarMonth.Year, calendarMonth.Month, day);
                int cell = startCol + day - 1;
                int col = cell % 7;
                int row = cell / 7;

                bool isTrainingDay = date.DayOfWeek == DayOfWeek.Monday ||
                                     date.DayOfWeek == DayOfWeek.Wednesday ||
                                     date.DayOfWeek == DayOfWeek.Friday;
                bool isToday = date.Date == DateTime.Today;

                Panel tile = new Panel
                {
                    Size = new Size(90, 68),
                    Location = new Point(col * 94, row * 74),
                    BackColor = isTrainingDay ? theme.Accent : AppColors.Shelf
                };

                tile.Controls.Add(new Label
                {
                    Text = day.ToString(),
                    Font = new Font("Segoe UI", 11F, isToday ? FontStyle.Bold | FontStyle.Underline : FontStyle.Bold),
                    ForeColor = Color.White,
                    AutoSize = false,
                    Size = new Size(84, 22),
                    TextAlign = ContentAlignment.TopLeft,
                    Location = new Point(6, 4),
                    BackColor = Color.Transparent
                });

                if (isTrainingDay)
                {
                    tile.Controls.Add(new Label
                    {
                        Text = "TRAIN",
                        Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(235, 238, 243),
                        AutoSize = false,
                        Size = new Size(84, 16),
                        TextAlign = ContentAlignment.BottomLeft,
                        Location = new Point(6, 46),
                        BackColor = Color.Transparent
                    });
                }

                if (isToday)
                {
                    // Bright ring around today
                    tile.Paint += (s, e) =>
                    {
                        using (Pen ring = new Pen(Color.White, 3))
                            e.Graphics.DrawRectangle(ring, 1, 1, tile.Width - 3, tile.Height - 3);
                    };
                }

                calendarGrid.Controls.Add(tile);
            }
        }

        // ------------------------------
        // TRAINING DATA
        // ------------------------------

        private void LoadTrainingFocusOptions()
        {
            focusComboBox.Items.Clear();

            currentSportDrills = DrillDatabase.GetDrillsBySport(_user.Sport);

            var focusOptions = currentSportDrills
                .Select(d => d.SkillCategory)
                .Distinct()
                .OrderBy(f => f)
                .ToList();

            foreach (string focus in focusOptions)
                focusComboBox.Items.Add(focus);

            if (focusComboBox.Items.Count > 0)
            {
                int existingIndex = focusComboBox.Items.IndexOf(_user.TargetArea);
                focusComboBox.SelectedIndex = existingIndex >= 0 ? existingIndex : 0;
            }
        }

        private void LoadDrillsForSelectedFocus()
        {
            drillCheckedListBox.Items.Clear();

            string selectedFocus = focusComboBox.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(selectedFocus))
                return;

            var filteredDrills = currentSportDrills
                .Where(d => d.SkillCategory.Equals(selectedFocus, StringComparison.OrdinalIgnoreCase))
                .OrderBy(d => d.Name)
                .ToList();

            foreach (Drill drill in filteredDrills)
                drillCheckedListBox.Items.Add($"{drill.Name} ({drill.Duration} min)");
        }

        private void GenerateScheduleButton_Click(object sender, EventArgs e)
        {
            scheduleListBox.Items.Clear();

            List<string> selectedDrills = new List<string>();
            foreach (var item in drillCheckedListBox.CheckedItems)
                selectedDrills.Add(item.ToString());

            if (selectedDrills.Count == 0)
            {
                scheduleCard.Visible = false;
                MessageBox.Show("Please check at least one drill before generating a schedule.");
                return;
            }

            scheduleCard.Visible = true;

            string[] daysPattern = { "Monday", "Wednesday", "Friday" };
            var scheduleByDay = new Dictionary<string, List<string>>();
            foreach (string day in daysPattern)
                scheduleByDay[day] = new List<string>();

            int totalMinutes = 0;

            for (int i = 0; i < selectedDrills.Count; i++)
            {
                string drillText = selectedDrills[i];
                string day = daysPattern[i % daysPattern.Length];
                scheduleByDay[day].Add(drillText);

                int start = drillText.LastIndexOf("(");
                int end = drillText.LastIndexOf(" min)");
                if (start >= 0 && end > start)
                {
                    string minuteText = drillText.Substring(start + 1, end - start - 1);
                    if (int.TryParse(minuteText, out int minutes))
                        totalMinutes += minutes;
                }
            }

            scheduleListBox.Items.Add("SKILL BUILDER PRO WEEKLY SCHEDULE");
            scheduleListBox.Items.Add("----------------------------------------");
            scheduleListBox.Items.Add($"Athlete: {_user.FullName}");
            scheduleListBox.Items.Add($"Sport:   {_user.Sport}");
            scheduleListBox.Items.Add($"Focus:   {focusComboBox.SelectedItem}");
            scheduleListBox.Items.Add("");

            foreach (string day in daysPattern)
            {
                scheduleListBox.Items.Add($"{day}:");
                scheduleListBox.Items.Add("----------------------------------------");

                if (scheduleByDay[day].Count == 0)
                    scheduleListBox.Items.Add("  (Rest)");
                else
                    foreach (string drill in scheduleByDay[day])
                        scheduleListBox.Items.Add("  " + drill);

                scheduleListBox.Items.Add("");
            }

            scheduleListBox.Items.Add("Summary");
            scheduleListBox.Items.Add("----------------------------------------");
            scheduleListBox.Items.Add($"Total Drills: {selectedDrills.Count}");
            scheduleListBox.Items.Add($"Estimated Time: {totalMinutes} minutes");
        }

        private void ClearSelectionButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < drillCheckedListBox.Items.Count; i++)
                drillCheckedListBox.SetItemChecked(i, false);

            scheduleListBox.Items.Clear();
            scheduleCard.Visible = false;
        }

        // ------------------------------
        // BACKGROUNDS (your sport mapping, kept verbatim)
        // ------------------------------

        private void SetTabBackground(TabPage tab)
        {
            Image bg = GetFieldBackground(_user.Sport);
            if (bg != null)
            {
                tab.BackgroundImage = ApplyDarkOverlay(bg);
                tab.BackgroundImageLayout = ImageLayout.Stretch;
            }
        }

        private Image GetFieldBackground(string sport)
        {
            switch (sport.Trim().ToLower())
            {
                case "basketball":
                    return Resource1.BasketballCourt;

                case "football":
                    return Resource1.FootballField;

                case "baseball":
                    return Resource1.BaseballDiamond;

                case "softball":
                    return Resource1.softball_field;

                case "soccer":
                    return Resource1.soccer_field;

                case "hockey":
                    return Resource1.hockey_rink;

                default:
                    return Resource1.weight_room;
            }
        }

        /// <summary>
        /// Sport-specific calendar backgrounds from the calendar_* image set.
        /// </summary>
        private Image GetCalendarBackground(string sport)
        {
            switch ((sport ?? "").Trim().ToLower())
            {
                case "basketball":
                    return Resource1.calendar_basketball;

                case "football":
                    return Resource1.calendar_football;

                case "baseball":
                    return Resource1.calendar_baseball;

                case "softball":
                    return Resource1.calendar_softball;

                case "soccer":
                    return Resource1.calendar_soccer;

                case "hockey":
                    return Resource1.calendar_hockey;

                default:
                    return Resource1.calendar_gym;
            }
        }

        /// <summary>
        /// Returns a darkened copy of an image (45% black overlay) so cards
        /// and text stay readable over the busy photo backgrounds.
        /// </summary>
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