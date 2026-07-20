using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SkillBuilderPro.WinForms.Models;
using SkillBuilderPro.WinForms.Services;
using CoreDrill = SkillBuilderPro.Core.Models.Drill;

namespace SkillBuilderPro.WinForms
{
    /// <summary>
    /// Modal video player form with playlist support.
    /// Fetches VideoUrl from the API for each drill and plays them in sequence.
    /// Auto-advances to the next video when the current one finishes.
    /// </summary>
    public partial class VideoPlayerForm : Form
    {
        private readonly User _user;
        private readonly List<string> _drillNames;
        private List<string> _videoUrls = new List<string>();
        private int _currentIndex = 0;

        private Label titleLabel;
        private Label statusLabel;
        private Panel videoPanel;
        private Button prevButton;
        private Button nextButton;
        private Button closeButton;

        public VideoPlayerForm(User user, List<string> drillNames)
        {
            _user = user;
            _drillNames = drillNames ?? new List<string>();
            InitializeComponent();
            BuildUI();
        }

        private void InitializeComponent()
        {
            this.Text = "Training Video Playlist";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(20, 24, 32);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.Icon = null;
        }

        private void BuildUI()
        {
            // TITLE LABEL
            titleLabel = new Label
            {
                Text = "Loading drills...",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = false,
                Height = 50,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(10)
            };
            this.Controls.Add(titleLabel);

            // STATUS LABEL (drill X of Y)
            statusLabel = new Label
            {
                Text = "Preparing...",
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = Color.FromArgb(155, 170, 190),
                BackColor = Color.Transparent,
                AutoSize = false,
                Height = 30,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(statusLabel);

            // VIDEO PANEL (placeholder for WMP or black screen)
            videoPanel = new Panel
            {
                BackColor = Color.Black,
                Dock = DockStyle.Fill
            };
            this.Controls.Add(videoPanel);

            // BUTTON PANEL
            Panel buttonPanel = new Panel
            {
                Height = 60,
                Dock = DockStyle.Bottom,
                BackColor = Color.FromArgb(32, 38, 50),
                Padding = new Padding(20)
            };
            this.Controls.Add(buttonPanel);

            // PREV BUTTON
            prevButton = new Button
            {
                Text = "< PREVIOUS",
                Size = new Size(120, 40),
                Location = new Point(20, 10),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(52, 60, 76),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            prevButton.FlatAppearance.BorderSize = 0;
            prevButton.Click += (s, e) => PreviousVideo();
            buttonPanel.Controls.Add(prevButton);

            // NEXT BUTTON
            nextButton = new Button
            {
                Text = "NEXT >",
                Size = new Size(120, 40),
                Location = new Point(150, 10),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(52, 60, 76),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            nextButton.FlatAppearance.BorderSize = 0;
            nextButton.Click += (s, e) => NextVideo();
            buttonPanel.Controls.Add(nextButton);

            // CLOSE BUTTON
            closeButton = new Button
            {
                Text = "CLOSE",
                Size = new Size(100, 40),
                Location = new Point(buttonPanel.Width - 120, 10),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(180, 50, 50),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.Click += (s, e) => this.Close();
            buttonPanel.Controls.Add(closeButton);
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            await LoadPlaylistAsync();
            if (_videoUrls.Count > 0)
            {
                PlayCurrentVideo();
            }
            else
            {
                titleLabel.Text = "No videos found for selected drills.";
                statusLabel.Text = "Try again or select different drills.";
            }
        }

        private async Task LoadPlaylistAsync()
        {
            titleLabel.Text = "Loading drills...";
            statusLabel.Text = "Fetching video URLs from API...";

            _videoUrls.Clear();

            // Extract drill names from the input (e.g., "Hitting Drills (20 min)" → "Hitting Drills")
            var cleanedDrillNames = _drillNames
                .Select(name =>
                {
                    int idx = name.LastIndexOf("(");
                    return idx > 0 ? name.Substring(0, idx).Trim() : name;
                })
                .ToList();

            try
            {
                // Fetch all drills from the API for the user's sport
                var apiService = new DrillApiService();
                var allDrills = await apiService.GetAllAsync(_user.Sport);
               // MessageBox.Show(_user.Sport);


                // Match by name and extract VideoUrl
                foreach (var drillName in cleanedDrillNames)
                {
                    var match = allDrills.FirstOrDefault(d =>
                        d.Name.Equals(drillName, StringComparison.OrdinalIgnoreCase));

                    if (match != null && !string.IsNullOrWhiteSpace(match.VideoUrl))
                    {
                        _videoUrls.Add(match.VideoUrl);
                    }
                    else
                    {
                        // If no match or no VideoUrl, add a placeholder or skip
                        _videoUrls.Add(""); // Empty URL means "not available"
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading video URLs: {ex.Message}", "Video Load Error");
            }

            _currentIndex = 0;
            UpdateStatus();
        }

        private void PlayCurrentVideo()
        {
            if (_currentIndex < 0 || _currentIndex >= _videoUrls.Count)
                return;

            string url = _videoUrls[_currentIndex];

            if (string.IsNullOrWhiteSpace(url))
            {
                titleLabel.Text = $"Video {_currentIndex + 1} of {_videoUrls.Count}";
                statusLabel.Text = "No video URL available for this drill.";
                videoPanel.BackColor = Color.Black;
                return;
            }

            // Extract drill name from the original list
            string drillName = _drillNames[_currentIndex];
            int idx = drillName.LastIndexOf("(");
            if (idx > 0)
                drillName = drillName.Substring(0, idx).Trim();

            titleLabel.Text = drillName;
            statusLabel.Text = $"Video {_currentIndex + 1} of {_videoUrls.Count}";

            // Open URL in default browser (simple approach)
            // For a full in-app player, use AxWindowsMediaPlayer or similar
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening video: {ex.Message}", "Playback Error");
            }
        }

        private void PreviousVideo()
        {
            if (_currentIndex > 0)
            {
                _currentIndex--;
                PlayCurrentVideo();
            }
        }

        private void NextVideo()
        {
            if (_currentIndex < _videoUrls.Count - 1)
            {
                _currentIndex++;
                PlayCurrentVideo();
            }
        }

        private void UpdateStatus()
        {
            statusLabel.Text = _videoUrls.Count > 0
                ? $"Video {_currentIndex + 1} of {_videoUrls.Count}"
                : "No videos loaded.";
        }
    }
}