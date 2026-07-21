
using SkillBuilderPro.WinForms.Models;
using SkillBuilderPro.WinForms.Properties;
using SkillBuilderPro.WinForms.Services;
using SkillBuilderPro.WinForms.Theming;
using static SkillBuilderPro.WinForms.Theming.TeamThemes;


namespace SkillBuilderPro.WinForms
{
    /// <summary>
    /// MainForm - dashboard after login.
    /// Header + custom nav bar + Training / Goals / Calendar tabs.
    /// ATHLETE PROFILE opens the standalone LockerRoomForm.
    /// </summary>
    public partial class MainForm : Form
    {
        private readonly SkillBuilderPro.WinForms.Models.User _user;
        private readonly bool _isDemoMode;
        public SkillBuilderPro.WinForms.Models.User? NextUser { get; private set; }


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
            public static readonly Color Shelf = Color.FromArgb(52, 60, 76);   // was 42,49,63
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

            Controls.Add(mainTabControl);
            Controls.Add(navPanel);
            Controls.Add(headerPanel);

            SelectNavTab(1);
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
                Height = 90,
                BackColor = theme.Panel
            };

            // TITLE
            Label titleLabel = new Label
            {
                Text = "SKILL BUILDER PRO",
                ForeColor = theme.Text,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(150, 12)
            };

            // SUBTITLE
            Label subtitleLabel = new Label
            {
                Text = $"Sport: {_user.Sport}   |   Focus: {_user.TargetArea}",
                ForeColor = theme.Text,
                Font = new Font("Segoe UI", 11F, FontStyle.Regular),
                AutoSize = true,
                Location = new Point(22, 50)
            };

            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(subtitleLabel);

            // LOG OUT / EXIT DEMO BUTTON
            Button exitButton = new Button
            {
                Text = _isDemoMode ? "EXIT DEMO MODE" : "LOG OUT",
                Size = new Size(145, 40),
                BackColor = theme.Accent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            exitButton.FlatAppearance.BorderSize = 0;
            exitButton.Click += (s, e) =>
            {
                NextUser = null;
                Close();
            };

            headerPanel.Controls.Add(exitButton);

            void PositionExitButton()
            {
                exitButton.Location = new Point(
                    headerPanel.ClientSize.Width - exitButton.Width - 24,
                    25);
            }
            headerPanel.Resize += (s, e) => PositionExitButton();
            PositionExitButton();

            NavHelper.AddBackButton(this, headerPanel, () => { NextUser = null; Close(); });
            NavHelper.AddMuteButton(headerPanel);
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
                Height = 48,               // was 52
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
                    Height = 48,
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

                // Click handlers: ATHLETE PROFILE opens LockerRoomForm,
                // API DRILLS opens ApiDrillsForm, others select the tab.
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
                BackColor = Color.FromArgb(205, AppColors.TrainingCard.R, AppColors.TrainingCard.G, AppColors.TrainingCard.B)
            };

            void PositionCard() => card.Location = new Point(
                Math.Max((tab.ClientSize.Width - width) / 2, 0) + tab.AutoScrollPosition.X,
                topMargin + tab.AutoScrollPosition.Y);

            PositionCard();
            tab.Resize += (s, e) => PositionCard();

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

            tab.Padding = new Padding(0, 30, 0, 0);


            Panel card = CreateCardPanel(tab, 420, 190, 100);

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

        // ============ ASYNC VERSION — wire MainForm.SetupTrainingTab and LoadTrainingFocusOptions to DrillProvider ============
        private async void SetupTrainingTab(TabPage tab)
        {
            SetTabBackground(tab);
            tab.AutoScroll = true;

            const int leftWidth = 400;
            const int leftHeight = 580;
            const int rightWidth = 400;
            const int rightHeight = 560;
            const int gap = 28;
            const int topMargin = 24;

            Panel leftCard = new Panel
            {
                Size = new Size(leftWidth, leftHeight),
                BackColor = Color.FromArgb(150, AppColors.TrainingCard.R, AppColors.TrainingCard.G, AppColors.TrainingCard.B)
            };
            Panel rightCard = new Panel
            {
                Size = new Size(rightWidth, rightHeight),
                BackColor = Color.FromArgb(150, AppColors.TrainingCard.R, AppColors.TrainingCard.G, AppColors.TrainingCard.B),
                Visible = false
            };
            scheduleCard = rightCard;

            void PositionCards()
            {
                const int edgeMargin = 40;
                leftCard.Location = new Point(
                    edgeMargin + tab.AutoScrollPosition.X,
                    topMargin + tab.AutoScrollPosition.Y);
                rightCard.Location = new Point(
                    Math.Max(tab.ClientSize.Width - rightWidth - edgeMargin, leftWidth + edgeMargin + gap) + tab.AutoScrollPosition.X,
                    topMargin + tab.AutoScrollPosition.Y);
            }
            PositionCards();
            tab.Resize += (s, e) => PositionCards();

            // ----- LEFT: TRAINING BUILDER -----

            Label title = CreateCardLabel(leftCard, "Training Builder",
                18F, FontStyle.Bold, Brand.TextStrong, 14, 44);

            // CATEGORY LABEL & COMBO (Offense/Defense or Hitting/Fielding, etc.)
            Label categoryLabel = CreateCardLabel(leftCard, "Training Category",
                11F, FontStyle.Bold, Brand.TextCell, 64, 28);

            focusComboBox = new ComboBox
            {
                Width = 340,
                Height = 30,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                BackColor = AppColors.Shelf,
                ForeColor = Brand.TextStrong,
                FlatStyle = FlatStyle.Flat
            };
            CenterInCard(leftCard, focusComboBox, 96);

            // SOURCE LABEL (API/Offline)
            Label sourceLabel = CreateCardLabel(leftCard, "Drills: —",
                8F, FontStyle.Regular, Brand.TextCell, 124, 16);

            // DRILLS LABEL & LIST
            Label drillLabel = CreateCardLabel(leftCard, "Select Drills (Max 5)",
                11F, FontStyle.Bold, Brand.TextCell, 136, 28);

            drillCheckedListBox = new CheckedListBox
            {
                Width = 340,
                Height = 220,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                BackColor = AppColors.Shelf,
                ForeColor = Brand.TextStrong,
                BorderStyle = BorderStyle.None,
                CheckOnClick = true
            };
            CenterInCard(leftCard, drillCheckedListBox, 168);

            // EVENT: Category changed → load drills for that category
            focusComboBox.SelectedIndexChanged += (s, e) => LoadDrillsForSelectedCategory();

            // EVENT: Drill checked → enforce max 5 limit
            drillCheckedListBox.ItemCheck += (s, e) =>
            {
                if (e.NewValue == CheckState.Checked)
                {
                    int checkedCount = drillCheckedListBox.CheckedItems.Count;

                    // If already 5 checked, block the new check
                    if (checkedCount >= 5)
                    {
                        e.NewValue = e.CurrentValue;  // ✔ Prevent the change
                        MessageBox.Show("You can select a maximum of 5 drills.", "Limit Reached");
                    }
                }
            };


            // TRAINING DAYS
            Label daysLabel = CreateCardLabel(leftCard, "Training Days",
                11F, FontStyle.Bold, Brand.TextCell, 396, 28);

            ComboBox daysPresetComboBox = new ComboBox
            {
                Width = 340,
                Height = 30,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                BackColor = AppColors.Shelf,
                ForeColor = Brand.TextStrong,
                FlatStyle = FlatStyle.Flat
            };
            daysPresetComboBox.Items.AddRange(new object[]
            {
                "Mon / Wed / Fri",
                "Tue / Thu",
                "Mon – Fri (Weekdays)",
                "Sat / Sun (Weekends)",
                "Every Day"
            });
            daysPresetComboBox.SelectedIndex = 0;
            CenterInCard(leftCard, daysPresetComboBox, 428);

            daysPresetComboBox.SelectedIndexChanged += (s, e) =>
            {
                trainingDays.Clear();
                switch (daysPresetComboBox.SelectedIndex)
                {
                    case 0:
                        trainingDays.Add(DayOfWeek.Monday);
                        trainingDays.Add(DayOfWeek.Wednesday);
                        trainingDays.Add(DayOfWeek.Friday);
                        break;
                    case 1:
                        trainingDays.Add(DayOfWeek.Tuesday);
                        trainingDays.Add(DayOfWeek.Thursday);
                        break;
                    case 2:
                        for (DayOfWeek d = DayOfWeek.Monday; d <= DayOfWeek.Friday; d++)
                            trainingDays.Add(d);
                        break;
                    case 3:
                        trainingDays.Add(DayOfWeek.Saturday);
                        trainingDays.Add(DayOfWeek.Sunday);
                        break;
                    case 4:
                        foreach (DayOfWeek d in Enum.GetValues(typeof(DayOfWeek)))
                            trainingDays.Add(d);
                        break;
                }
                if (calendarGrid != null)
                    RenderCalendar();
            };

            leftCard.Controls.Add(daysLabel);
            leftCard.Controls.Add(daysPresetComboBox);

            // BUTTONS
            generateScheduleButton = CreateModernButton("GENERATE SCHEDULE", 340, 38);
            clearSelectionButton = CreateModernButton("CLEAR DRILLS", 164, 38);
            generateVideoButton = CreateModernButton("TRAINING VIDEO", 164, 38);

            CenterInCard(leftCard, generateScheduleButton, 472);
            clearSelectionButton.Location = new Point(30, 520);
            generateVideoButton.Location = new Point(206, 520);

            generateScheduleButton.Click += GenerateScheduleButton_Click;
            clearSelectionButton.Click += ClearSelectionButton_Click;
            generateVideoButton.Click += (s, e) => OpenVideoPlaylist();

            leftCard.Controls.Add(title);
            leftCard.Controls.Add(categoryLabel);
            leftCard.Controls.Add(focusComboBox);
            leftCard.Controls.Add(sourceLabel);
            leftCard.Controls.Add(drillLabel);
            leftCard.Controls.Add(drillCheckedListBox);
            leftCard.Controls.Add(generateScheduleButton);
            leftCard.Controls.Add(clearSelectionButton);
            leftCard.Controls.Add(generateVideoButton);

            // ----- RIGHT: SCHEDULE PREVIEW -----

            Label previewTitle = CreateCardLabel(rightCard, "Schedule Preview",
                18F, FontStyle.Bold, Brand.TextStrong, 14, 44);

            scheduleListBox = new ListBox
            {
                Width = 340,
                Height = 470,
                Font = new Font("Consolas", 10F, FontStyle.Bold),
                BackColor = AppColors.Shelf,
                ForeColor = Brand.TextStrong,
                BorderStyle = BorderStyle.None
            };
            CenterInCard(rightCard, scheduleListBox, 68);

            rightCard.Controls.Add(previewTitle);
            rightCard.Controls.Add(scheduleListBox);

            tab.Controls.Add(leftCard);
            tab.Controls.Add(rightCard);
            leftCard.BringToFront();
            rightCard.BringToFront();

            // Load categories and populate combo
            try
            {
                await LoadTrainingCategoriesAsync();
                sourceLabel.Text = $"Drills: {DrillProvider.LastSource}";
            }
            catch (Exception ex)
            {
                sourceLabel.Text = $"Drills: ERROR - {ex.GetType().Name}";
                System.Diagnostics.Debug.WriteLine($"[SetupTrainingTab] {ex.GetType().Name}: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack: {ex.StackTrace}");
            }
        }

        private async Task LoadTrainingCategoriesAsync()
        {
            focusComboBox.Items.Clear();

            currentSportDrills = await DrillProvider.GetBySportAsync(_user.Sport);

            var categories = currentSportDrills
                .Select(d => d.SkillCategory)  // ← Use SkillCategory
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            foreach (string category in categories)
                focusComboBox.Items.Add(category);

            if (focusComboBox.Items.Count > 0)
                focusComboBox.SelectedIndex = 0;
        }

        private void LoadDrillsForSelectedCategory()
        {
            drillCheckedListBox.Items.Clear();

            string selectedCategory = focusComboBox.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(selectedCategory))
                return;

            var filteredDrills = currentSportDrills
                .Where(d => d.SkillCategory.Equals(selectedCategory, StringComparison.OrdinalIgnoreCase))  // ← Use SkillCategory
                .OrderBy(d => d.Name)
                .ToList();

            foreach (Drill drill in filteredDrills)
                drillCheckedListBox.Items.Add($"{drill.Name} ({drill.Duration} min)");
        }
        private void OpenVideoPlaylist()
        {
            List<string> selectedDrills = new List<string>();
            foreach (var item in drillCheckedListBox.CheckedItems)
                selectedDrills.Add(item.ToString());

            if (selectedDrills.Count == 0)
            {
                MessageBox.Show("Please check at least one drill before opening the video playlist.",
                    "No Drills Selected");
                return;
            }

            using (var playerForm = new VideoPlayerForm(_user, selectedDrills))
            {
                playerForm.ShowDialog(this);
            }
        }

        // ------------------------------
        // GOALS TAB - branded goal roadmap + progress
        // ------------------------------

        private void SetupGoalsTab(TabPage tab)
        {
            SetTabBackground(tab);
            tab.AutoScroll = true;              // scroll if window is short

            var theme = TeamThemes.GetThemeForSport(_user.Sport);

            Panel card = CreateCardPanel(tab, 700, 540, 150);

            Label title = CreateCardLabel(card, "GOAL ROADMAP",
            20F, FontStyle.Bold, AppColors.TrainingText, 12, 46);   // y=14, height=36

            Label brand = CreateCardLabel(card,
                $"SKILL BUILDER PRO  •  {(_user.Sport ?? "").ToUpper()}",
                10F, FontStyle.Regular, AppColors.SubtleText, 58, 22);  // y=52, height=18

            Label goalLabel = new Label
            {
                Text = string.IsNullOrWhiteSpace(_user.Goal) ? "No goal set yet" : $"\"{_user.Goal}\"",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold | FontStyle.Italic),
                ForeColor = Color.White,
                BackColor = theme.Accent,
                AutoSize = false,
                Width = card.Width - 80,
                Height = 46,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(40, 84)
            };
            card.Controls.Add(goalLabel);

            string[] stages = { "COMMIT", "TRAIN", "COMPETE", "DOMINATE" };
            int currentStage = GetGoalStage(_user.ExperienceLevel);
            int progressPercent = currentStage * 100 / stages.Length;

            BufferedGoalPanel roadmap = new BufferedGoalPanel
            {
                Size = new Size(card.Width - 80, 190),
                Location = new Point(40, 154),
                BackColor = AppColors.Shelf
            };
            roadmap.Paint += (s, e) =>
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                int n = stages.Length;
                int nodeSize = 54;
                int marginX = 50;
                int cy = 78;
                int usable = roadmap.Width - marginX * 2;
                int gap = usable / (n - 1);

                using (Pen track = new Pen(Color.FromArgb(70, 78, 94), 6))
                    g.DrawLine(track, marginX, cy, marginX + usable, cy);

                if (currentStage > 1)
                {
                    using (Pen done = new Pen(theme.Accent, 6))
                        g.DrawLine(done, marginX, cy, marginX + gap * (currentStage - 1), cy);
                }

                for (int i = 0; i < n; i++)
                {
                    int cx = marginX + gap * i;
                    Rectangle node = new Rectangle(cx - nodeSize / 2, cy - nodeSize / 2, nodeSize, nodeSize);

                    bool reached = (i + 1) <= currentStage;
                    bool isCurrent = (i + 1) == currentStage;

                    using (SolidBrush fill = new SolidBrush(reached ? theme.Accent : Color.FromArgb(58, 66, 82)))
                        g.FillEllipse(fill, node);

                    if (isCurrent)
                    {
                        using (Pen ring = new Pen(Color.White, 3))
                            g.DrawEllipse(ring, Rectangle.Inflate(node, 4, 4));
                    }

                    string mark = (i + 1) < currentStage ? "✓" : (i + 1).ToString();
                    using (Font markFont = new Font("Segoe UI", 16F, FontStyle.Bold))
                    using (SolidBrush markBrush = new SolidBrush(Color.White))
                    {
                        SizeF ms = g.MeasureString(mark, markFont);
                        g.DrawString(mark, markFont, markBrush, cx - ms.Width / 2, cy - ms.Height / 2);
                    }

                    using (Font capFont = new Font("Segoe UI", 9F, FontStyle.Bold))
                    using (SolidBrush capBrush = new SolidBrush(reached ? AppColors.TrainingText : AppColors.SubtleText))
                    {
                        SizeF cs = g.MeasureString(stages[i], capFont);
                        g.DrawString(stages[i], capFont, capBrush, cx - cs.Width / 2, cy + nodeSize / 2 + 10);
                    }
                }
            };
            card.Controls.Add(roadmap);

            Label progressLabel = CreateCardLabel(card,
                $"PROGRESS: {progressPercent}%  —  STAGE {currentStage} OF {stages.Length} ({stages[currentStage - 1]})",
                10F, FontStyle.Bold, AppColors.TrainingText, 362, 22);

            BufferedGoalPanel progressBar = new BufferedGoalPanel
            {
                Size = new Size(card.Width - 80, 22),
                Location = new Point(40, 392),
                BackColor = Color.FromArgb(58, 68, 86)
            };
            progressBar.Paint += (s, e) =>
            {
                int fillWidth = progressBar.Width * progressPercent / 100;
                using (SolidBrush fill = new SolidBrush(theme.Accent))
                    e.Graphics.FillRectangle(fill, 0, 0, fillWidth, progressBar.Height);

                using (Font pctFont = new Font("Segoe UI", 9F, FontStyle.Bold))
                using (SolidBrush white = new SolidBrush(Color.White))
                {
                    string pct = $"{progressPercent}%";
                    SizeF ps = e.Graphics.MeasureString(pct, pctFont);
                    e.Graphics.DrawString(pct, pctFont, white,
                        (progressBar.Width - ps.Width) / 2, (progressBar.Height - ps.Height) / 2);
                }
            };
            card.Controls.Add(progressBar);

            Label hint = CreateCardLabel(card,
                "Your stage advances with your experience level. Complete training sessions to level up.",
                9F, FontStyle.Regular, AppColors.SubtleText, 430, 20);

            Button viewPlanButton = CreateModernButton("VIEW TRAINING PLAN", 240, 42);
            CenterInCard(card, viewPlanButton, 476);   // was 470 — fine, or leave
            viewPlanButton.Click += (s, e) => SelectNavTab(1);

            card.Controls.Add(title);
            card.Controls.Add(brand);
            card.Controls.Add(progressLabel);
            card.Controls.Add(hint);
            card.Controls.Add(viewPlanButton);
        }

        /// <summary>Maps experience level to roadmap stage (1-based).</summary>
        private static int GetGoalStage(string experienceLevel)
        {
            switch ((experienceLevel ?? "").Trim().ToLower())
            {
                case "beginner": return 2;
                case "intermediate": return 3;
                case "advanced": return 4;
                default: return 1;
            }
        }

        // ------------------------------
        // CALENDAR TAB - custom month grid, training days in team color
        // ------------------------------

        private DateTime calendarMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        private Panel calendarGrid;
        private Label calendarMonthLabel;

        // Athlete-selected training days — defaults to Mon/Wed/Fri
        private readonly HashSet<DayOfWeek> trainingDays = new HashSet<DayOfWeek>
        {
            DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday
        };

        private void SetupCalendarTab(TabPage tab)
        {
            // Sport-specific calendar background (calendar_* image set)
            Image calBg = GetCalendarBackground(_user.Sport);
            if (calBg != null)
            {
                tab.BackgroundImage = Brand.Hero(calBg);
                tab.BackgroundImageLayout = ImageLayout.Stretch;
            }

            var theme = TeamThemes.GetThemeForSport(_user.Sport);

            Panel card = CreateCardPanel(tab, 700, 730, 100);




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

            // Day picker — check the days you train; calendar + export follow
            DayOfWeek[] allDays = { DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday,
                            DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday };
            string[] dayShort = { "SUN", "MON", "TUE", "WED", "THU", "FRI", "SAT" };

            for (int i = 0; i < 7; i++)
            {
                DayOfWeek d = allDays[i];
                CheckBox dayCheck = new CheckBox
                {
                    Text = dayShort[i],
                    Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    ForeColor = AppColors.TrainingText,
                    BackColor = Color.Transparent,
                    AutoSize = false,
                    Size = new Size(70, 24),
                    Location = new Point(48 + i * 88, 548),
                    Checked = trainingDays.Contains(d)
                };
                dayCheck.CheckedChanged += (s, e) =>
                {
                    if (dayCheck.Checked) trainingDays.Add(d);
                    else trainingDays.Remove(d);
                    RenderCalendar();
                };
                card.Controls.Add(dayCheck);
            }

            Label legend = CreateCardLabel(card,
                "Team-color days = your training schedule (pick your days above)",
                10F, FontStyle.Regular, AppColors.SubtleText, 578, 24);

            // Export button — universal .ics for Google / Apple / Outlook
            Button exportButton = CreateModernButton("ADD TO MY CALENDAR  (Google / Apple / Outlook)", 380, 42);
            CenterInCard(card, exportButton, 646);
            exportButton.Click += ExportCalendarButton_Click;
            card.Controls.Add(exportButton);

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

                bool isTrainingDay = trainingDays.Contains(date.DayOfWeek);
                bool isToday = date.Date == DateTime.Today;

                Panel tile = new Panel
                {
                    Size = new Size(90, 68),
                    Location = new Point(col * 94, row * 74),
                    BackColor = isTrainingDay
                    ? Color.FromArgb(200, theme.Accent.R, theme.Accent.G, theme.Accent.B)
                    : Color.FromArgb(140, AppColors.Shelf.R, AppColors.Shelf.G, AppColors.Shelf.B)
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
        /// <summary>
        /// Exports the selected training days for the displayed month as a
        /// universal .ics file — importable by Google Calendar, Apple Calendar,
        /// and Outlook.
        /// </summary>
        private void ExportCalendarButton_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = "Save Training Schedule";
                sfd.Filter = "Calendar File (*.ics)|*.ics";
                sfd.FileName = $"SkillBuilderPro_{_user.Sport}_{calendarMonth:yyyy_MM}.ics";

                if (sfd.ShowDialog() != DialogResult.OK)
                    return;

                System.IO.File.WriteAllText(sfd.FileName, BuildTrainingIcs());

                MessageBox.Show(
                    "Training schedule saved!\n\n" +
                    "Google Calendar: Settings → Import & Export → Import\n" +
                    "Apple Calendar: double-click the file (or AirDrop to iPhone)\n" +
                    "Outlook: double-click the file",
                    "Added to Calendar",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Builds iCalendar (RFC 5545) content: one 1-hour training event for every
        /// selected training day in the displayed month, 5:00-6:00 PM local time.
        /// </summary>
        private string BuildTrainingIcs()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("PRODID:-//SkillBuilderPro//Training Schedule//EN");

            int daysInMonth = DateTime.DaysInMonth(calendarMonth.Year, calendarMonth.Month);

            for (int day = 1; day <= daysInMonth; day++)
            {
                DateTime date = new DateTime(calendarMonth.Year, calendarMonth.Month, day);

                if (!trainingDays.Contains(date.DayOfWeek))
                    continue;

                DateTime start = date.AddHours(17);   // 5:00 PM
                DateTime end = date.AddHours(18);     // 6:00 PM

                sb.AppendLine("BEGIN:VEVENT");
                sb.AppendLine($"UID:{Guid.NewGuid()}@skillbuilderpro");
                sb.AppendLine($"DTSTAMP:{DateTime.UtcNow:yyyyMMddTHHmmssZ}");
                sb.AppendLine($"DTSTART:{start:yyyyMMddTHHmmss}");
                sb.AppendLine($"DTEND:{end:yyyyMMddTHHmmss}");
                sb.AppendLine($"SUMMARY:{_user.Sport} Training — {_user.TargetArea} (Skill Builder Pro)");
                sb.AppendLine($"DESCRIPTION:Athlete: {_user.FullName}\\nFocus: {_user.TargetArea}\\nGoal: {_user.Goal}");
                sb.AppendLine("END:VEVENT");
            }

            sb.AppendLine("END:VCALENDAR");
            return sb.ToString();
        }
        // ------------------------------
        // TRAINING DATA
        // ------------------------------

      
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

            string[] daysPattern = trainingDays
                .OrderBy(d => (int)d)
                .Select(d => d.ToString())
                .ToArray();

            if (daysPattern.Length == 0)
            {
                MessageBox.Show("Pick at least one training day on the Calendar tab first.");
                return;
            }
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
            tab.Padding = new Padding(0, 20, 0, 0);

            Image bg = GetFieldBackground(_user.Sport);
            if (bg != null)
            {
                tab.BackgroundImage = Brand.Hero(bg);
                tab.BackgroundImageLayout = ImageLayout.Stretch;
            }
        }
        private Image GetFieldBackground(string? sport)
        {
            switch ((sport ?? "").Trim().ToLowerInvariant())
            {
                case "basketball":
                    return Resource1.Chicago_Basketball;

                case "football":
                    return Resource1.Chicago_Football;

                case "baseball":
                    return Resource1.Chicago_Baseball;

                case "softball":
                    return Resource1.softball_field;

                case "soccer":
                    return Resource1.Chicago_Soccer;

                case "hockey":
                    return Resource1.Chicago_Hockey;

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
        private static Image ResizeImageToFill(Image source, Size targetSize)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (targetSize.Width <= 0 || targetSize.Height <= 0)
                return new Bitmap(source);

            Bitmap result = new Bitmap(targetSize.Width, targetSize.Height);

            float sourceRatio = (float)source.Width / source.Height;
            float targetRatio = (float)targetSize.Width / targetSize.Height;

            Rectangle sourceRectangle;

            if (sourceRatio > targetRatio)
            {
                int croppedWidth = (int)(source.Height * targetRatio);
                int cropX = (source.Width - croppedWidth) / 2;

                sourceRectangle = new Rectangle(
                    cropX,
                    0,
                    croppedWidth,
                    source.Height);
            }
            else
            {
                int croppedHeight = (int)(source.Width / targetRatio);
                int cropY = (source.Height - croppedHeight) / 2;

                sourceRectangle = new Rectangle(
                    0,
                    cropY,
                    source.Width,
                    croppedHeight);
            }

            using (Graphics graphics = Graphics.FromImage(result))
            {
                graphics.InterpolationMode =
                    System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                graphics.SmoothingMode =
                    System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                graphics.PixelOffsetMode =
                    System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                graphics.DrawImage(
                    source,
                    new Rectangle(0, 0, targetSize.Width, targetSize.Height),
                    sourceRectangle,
                    GraphicsUnit.Pixel);
            }

            return result;
        }

        private static Image ApplyDarkOverlay(
    Image source,
    float opacity = 0.45f)
        {
            Bitmap darkened = new Bitmap(source.Width, source.Height);

            using (Graphics g = Graphics.FromImage(darkened))
            using (SolidBrush overlay = new SolidBrush(
                Color.FromArgb((int)(opacity * 255), 0, 0, 0)))
            {
                g.DrawImage(source, 0, 0, source.Width, source.Height);
                g.FillRectangle(overlay, 0, 0, source.Width, source.Height);
            }

            return darkened;
        }

        private void OpenApiDrills()
        {
            using (var form = new ApiDrillsForm())
            {
                form.ShowDialog(this);
            }
        }
        public class BufferedGoalPanel : Panel
        {
            public BufferedGoalPanel()
            {
                DoubleBuffered = true;
            }
        }
    }
}