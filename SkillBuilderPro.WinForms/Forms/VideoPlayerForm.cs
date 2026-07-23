using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SkillBuilderPro.Core.Models;
using SkillBuilderPro.WinForms.Services;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;
using User = SkillBuilderPro.WinForms.Models.User;

namespace SkillBuilderPro.WinForms
{
    public partial class VideoPlayerForm : Form
    {
        private User _user;
        private List<string> _drillNames;
        private List<string> _videoUrls;
        private int _currentIndex = 0;
        private DrillApiService _drillApiService;

        private Label titleLabel;
        private Button btnWatch;
        private Button btnPrev;
        private Button btnNext;
        private Button btnExit;
        private WebView2 videoView;

        public VideoPlayerForm(User user, List<string> drillNames)
        {
            _user = user;
            _drillNames = drillNames ?? new List<string>();
            _videoUrls = new List<string>();
            _drillApiService = new DrillApiService();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.titleLabel = new Label();
            this.btnWatch = new Button();
            this.btnPrev = new Button();
            this.btnNext = new Button();
            this.btnExit = new Button();
            this.videoView = new WebView2();

            this.SuspendLayout();

            // Form
            this.ClientSize = new System.Drawing.Size(1920, 1080);
            this.WindowState = FormWindowState.Maximized;
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.Name = "VideoPlayerForm";
            this.Text = "Training Videos";
            this.Load += new EventHandler(this.VideoPlayerForm_Load);

            // Title - white, centered, fits better
            this.titleLabel.BackColor = System.Drawing.Color.Transparent;
            this.titleLabel.ForeColor = System.Drawing.Color.White;
            this.titleLabel.Location = new System.Drawing.Point(260, 60);
            this.titleLabel.Size = new System.Drawing.Size(1400, 70);
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.titleLabel.Font = new System.Drawing.Font("Arial", 30, System.Drawing.FontStyle.Bold);
            this.titleLabel.AutoEllipsis = true;
            this.titleLabel.Text = "Loading...";

            // WebView2 video player – size/location finalized in Load
            this.videoView.BackColor = System.Drawing.Color.Black;
            this.videoView.DefaultBackgroundColor = System.Drawing.Color.Black;
            this.videoView.Visible = true;

            // Watch Button (under player)
            this.btnWatch.Location = new System.Drawing.Point(810, 840);
            this.btnWatch.Size = new System.Drawing.Size(300, 70);
            this.btnWatch.Text = "► WATCH VIDEO";
            this.btnWatch.Font = new System.Drawing.Font("Arial", 16, System.Drawing.FontStyle.Bold);
            this.btnWatch.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this.btnWatch.ForeColor = System.Drawing.Color.White;
            this.btnWatch.FlatStyle = FlatStyle.Flat;
            this.btnWatch.Click += new EventHandler(this.btnWatch_Click);

            // Prev
            this.btnPrev.Location = new System.Drawing.Point(200, 900);
            this.btnPrev.Size = new System.Drawing.Size(160, 50);
            this.btnPrev.Text = "◄ PREVIOUS";
            this.btnPrev.Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold);
            this.btnPrev.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this.btnPrev.ForeColor = System.Drawing.Color.White;
            this.btnPrev.FlatStyle = FlatStyle.Flat;
            this.btnPrev.Click += new EventHandler(this.btnPrev_Click);

            // Next
            this.btnNext.Location = new System.Drawing.Point(1560, 900);
            this.btnNext.Size = new System.Drawing.Size(160, 50);
            this.btnNext.Text = "NEXT ►";
            this.btnNext.Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold);
            this.btnNext.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this.btnNext.ForeColor = System.Drawing.Color.White;
            this.btnNext.FlatStyle = FlatStyle.Flat;
            this.btnNext.Click += new EventHandler(this.btnNext_Click);

            // Exit
            this.btnExit.Location = new System.Drawing.Point(890, 900);
            this.btnExit.Size = new System.Drawing.Size(160, 50);
            this.btnExit.Text = "EXIT";
            this.btnExit.Font = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold);
            this.btnExit.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this.btnExit.ForeColor = System.Drawing.Color.White;
            this.btnExit.FlatStyle = FlatStyle.Flat;
            this.btnExit.Click += new EventHandler((s, e) => this.Close());

            // Add controls
            this.Controls.Add(this.titleLabel);
            this.Controls.Add(this.videoView);
            this.Controls.Add(this.btnWatch);
            this.Controls.Add(this.btnPrev);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnExit);

            this.ResumeLayout(false);
        }

        private async void VideoPlayerForm_Load(object sender, EventArgs e)
        {
            // Background image
            this.BackgroundImage = Properties.Resource1.drill_library;

            // Initialize WebView2
            await this.videoView.EnsureCoreWebView2Async(null);

            // Center video view now that the form size is final
            int playerWidth = 960;
            int playerHeight = 540;
            int playerX = (this.ClientSize.Width - playerWidth) / 2;
            int playerY = 170; // just below the title

            this.videoView.Location = new System.Drawing.Point(playerX, playerY);
            this.videoView.Size = new System.Drawing.Size(playerWidth, playerHeight);

            // Load drills + videos
            await LoadVideoUrls();

            if (_videoUrls.Count > 0)
                DisplayCurrentVideo();
            else
                titleLabel.Text = "No videos available.";
        }

        private async System.Threading.Tasks.Task LoadVideoUrls()
        {
            var drills = await _drillApiService.GetAllAsync(_user.Sport);

            var drillsWithVideos = drills
                .Where(d => !string.IsNullOrEmpty(d.VideoUrl))
                .Take(5)
                .ToList();

            _videoUrls.Clear();

            foreach (var drill in drillsWithVideos)
            {
                _videoUrls.Add(drill.VideoUrl);
                _drillNames.Add(drill.Name);
            }

            _currentIndex = 0;
        }

        private void DisplayCurrentVideo()
        {
            if (_videoUrls.Count == 0)
                return;

            if (_currentIndex < 0 || _currentIndex >= _videoUrls.Count)
                return;

            titleLabel.Text = _drillNames[_currentIndex];

            btnPrev.Enabled = _currentIndex > 0;
            btnNext.Enabled = _currentIndex < _videoUrls.Count - 1;

            LoadVideoInPlayer(_videoUrls[_currentIndex]);
        }

        private void LoadVideoInPlayer(string url)
        {
            if (string.IsNullOrWhiteSpace(url) || videoView.CoreWebView2 == null)
                return;

            // Prefer normal watch URL, fix embed URLs if they exist
            if (url.Contains("/embed/"))
            {
                // Extract video id after last '/'
                var videoId = url.Substring(url.LastIndexOf('/') + 1);
                url = $"https://www.youtube.com/watch?v={videoId}";
            }

            videoView.CoreWebView2.Navigate(url);
        }

        private void btnWatch_Click(object sender, EventArgs e)
        {
            if (_videoUrls.Count == 0)
                return;

            LoadVideoInPlayer(_videoUrls[_currentIndex]);
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (_currentIndex > 0)
            {
                _currentIndex--;
                DisplayCurrentVideo();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (_currentIndex < _videoUrls.Count - 1)
            {
                _currentIndex++;
                DisplayCurrentVideo();
            }
        }
    }
}