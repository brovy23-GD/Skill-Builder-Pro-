using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using SkillBuilderPro.Client.Services;
using SkillBuilderPro.WinForms.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using User = SkillBuilderPro.WinForms.Models.User;

namespace SkillBuilderPro.WinForms.Forms
{
    public partial class VideoPlayerForm : Form
    {
        private readonly User _user;
        private readonly List<string> _selectedDrillNames;
        private readonly List<string> _drillNames;
        private readonly List<string> _videoUrls;
        private int _currentIndex = -1;
        private readonly DrillApiService _drillApiService;

        private TableLayoutPanel mainLayout;
        private Panel pnlDrillList;
        private Label lblDrillListHeader;
        private ListBox lstDrills;

        private Panel pnlVideoHost;
        private WebView2 videoView;

        private Panel pnlControls;
        private Button btnPrev;
        private Button btnStart;
        private Button btnNext;

        public VideoPlayerForm(User user, List<string> drillNames)
        {
            _user = user;
            _selectedDrillNames = drillNames ?? new List<string>();
            _drillNames = new List<string>();
            _videoUrls = new List<string>();

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5000/")
            };

            IApiClient apiClient = new ApiClient(httpClient);
            _drillApiService = new DrillApiService(apiClient);

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            mainLayout = new TableLayoutPanel();
            pnlDrillList = new Panel();
            lblDrillListHeader = new Label();
            lstDrills = new ListBox();

            pnlVideoHost = new Panel();
            videoView = new WebView2();

            pnlControls = new Panel();
            btnPrev = new Button();
            btnStart = new Button();
            btnNext = new Button();

            SuspendLayout();

            // Form
            ClientSize = new Size(1672, 1020);
            StartPosition = FormStartPosition.CenterScreen;
            BackgroundImageLayout = ImageLayout.Stretch;
            Name = "VideoPlayerForm";
            Text = "Training Videos";
            DoubleBuffered = true;
            Load += VideoPlayerForm_Load;

            // Main layout
            mainLayout.BackColor = Color.Transparent;
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.Padding = new Padding(320, 24, 320, 12);
            mainLayout.ColumnCount = 1;
            mainLayout.RowCount = 3;
            mainLayout.ColumnStyles.Clear();
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainLayout.RowStyles.Clear();
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 150F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 78F));

            // Drill panel
            pnlDrillList.Dock = DockStyle.Fill;
            pnlDrillList.Margin = new Padding(40, 0, 40, 10);
            pnlDrillList.Padding = new Padding(18, 10, 18, 10);
            pnlDrillList.BackColor = Color.FromArgb(10, 22, 40);

            lblDrillListHeader.Dock = DockStyle.Top;
            lblDrillListHeader.Height = 30;
            lblDrillListHeader.BackColor = Color.Transparent;
            lblDrillListHeader.ForeColor = Color.FromArgb(242, 246, 252);
            lblDrillListHeader.TextAlign = ContentAlignment.MiddleCenter;
            lblDrillListHeader.Font = new Font("Segoe UI Semibold", 16, FontStyle.Bold);
            lblDrillListHeader.Text = "SELECTED DRILLS";

            lstDrills.Dock = DockStyle.Fill;
            lstDrills.Margin = new Padding(0);
            lstDrills.BackColor = Color.FromArgb(17, 31, 54);
            lstDrills.ForeColor = Color.White;
            lstDrills.BorderStyle = BorderStyle.None;
            lstDrills.Font = new Font("Segoe UI", 11, FontStyle.Regular);
            lstDrills.IntegralHeight = false;
            lstDrills.ItemHeight = 30;
            lstDrills.DrawMode = DrawMode.OwnerDrawFixed;
            lstDrills.HorizontalScrollbar = false;
            lstDrills.MultiColumn = false;
            lstDrills.ScrollAlwaysVisible = false;
            lstDrills.DrawItem += lstDrills_DrawItem;
            lstDrills.SelectedIndexChanged += lstDrills_SelectedIndexChanged;

            pnlDrillList.Controls.Add(lstDrills);
            pnlDrillList.Controls.Add(lblDrillListHeader);

            // Video host
            pnlVideoHost.Dock = DockStyle.Fill;
            pnlVideoHost.Margin = new Padding(140, 0, 140, 12);

            pnlVideoHost.Padding = new Padding(6);
            pnlVideoHost.BackColor = Color.FromArgb(14, 20, 30);

            videoView.Dock = DockStyle.Fill;
            videoView.BackColor = Color.Black;
            videoView.DefaultBackgroundColor = Color.Black;
            videoView.Visible = true;
            videoView.AllowExternalDrop = false;

            pnlVideoHost.Controls.Add(videoView);

            // Controls panel
            pnlControls.Dock = DockStyle.Fill;
            pnlControls.Margin = new Padding(0);
            pnlControls.BackColor = Color.Transparent;

            btnPrev.Size = new Size(72, 48);
            btnPrev.Text = "◀";
            btnPrev.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            btnPrev.BackColor = Color.Transparent;
            btnPrev.ForeColor = Color.White;
            btnPrev.FlatStyle = FlatStyle.Flat;
            btnPrev.FlatAppearance.BorderSize = 0;
            btnPrev.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btnPrev.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btnPrev.UseVisualStyleBackColor = false;
            btnPrev.TabStop = false;
            btnPrev.Click += btnPrev_Click;

            btnStart.Size = new Size(330, 50);
            btnStart.Text = "START VIDEO";
            btnStart.Font = new Font("Segoe UI Semibold", 13, FontStyle.Bold);
            btnStart.BackColor = Color.FromArgb(0, 120, 215);
            btnStart.ForeColor = Color.White;
            btnStart.FlatStyle = FlatStyle.Flat;
            btnStart.FlatAppearance.BorderSize = 0;
            btnStart.Click += btnStart_Click;

            btnNext.Size = new Size(72, 48);
            btnNext.Text = "▶";
            btnNext.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            btnNext.BackColor = Color.Transparent;
            btnNext.ForeColor = Color.White;
            btnNext.FlatStyle = FlatStyle.Flat;
            btnNext.FlatAppearance.BorderSize = 0;
            btnNext.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btnNext.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btnNext.UseVisualStyleBackColor = false;
            btnNext.TabStop = false;
            btnNext.Click += btnNext_Click;

            pnlControls.Controls.Add(btnPrev);
            pnlControls.Controls.Add(btnStart);
            pnlControls.Controls.Add(btnNext);
            pnlControls.Resize += (s, e) => CenterBottomControls();

            mainLayout.Controls.Add(pnlDrillList, 0, 0);
            mainLayout.Controls.Add(pnlVideoHost, 0, 1);
            mainLayout.Controls.Add(pnlControls, 0, 2);

            Controls.Add(mainLayout);

            CenterBottomControls();

            ResumeLayout(false);
        }

        private void CenterBottomControls()
        {
            int gap = 24;
            int groupWidth = btnPrev.Width + gap + btnStart.Width + gap + btnNext.Width;
            int startX = (pnlControls.ClientSize.Width - groupWidth) / 2;
            int topY = 2;

            btnPrev.Location = new Point(startX, topY + 1);
            btnStart.Location = new Point(startX + btnPrev.Width + gap, topY);
            btnNext.Location = new Point(startX + btnPrev.Width + gap + btnStart.Width + gap, topY + 1);
        }

        private async void VideoPlayerForm_Load(object sender, EventArgs e)
        {
            BackgroundImage = Properties.Resource1.drill_library;

            await videoView.EnsureCoreWebView2Async(null);

            string localAppFolder = Application.StartupPath;
            videoView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                "myapp.local",
                localAppFolder,
                CoreWebView2HostResourceAccessKind.Allow);

            await LoadVideoUrls();
            PopulateDrillList();
            AdjustDrillPanelHeight();
            CenterBottomControls();

            if (_drillNames.Count > 0)
            {
                _currentIndex = 0;
                lstDrills.SelectedIndex = 0;
                btnStart.Enabled = true;
                btnPrev.Enabled = false;
                btnNext.Enabled = _drillNames.Count > 1;
            }
            else
            {
                btnStart.Enabled = false;
                btnPrev.Enabled = false;
                btnNext.Enabled = false;
            }
        }

        private void AdjustDrillPanelHeight()
        {
            int visibleRows = Math.Min(Math.Max(_drillNames.Count, 1), 4);
            int headerHeight = 30;
            int topPadding = 10;
            int bottomPadding = 10;
            int listHeight = visibleRows * lstDrills.ItemHeight + 6;
            int totalHeight = headerHeight + topPadding + bottomPadding + listHeight;

            mainLayout.RowStyles[0].Height = Math.Max(120, totalHeight);
        }

        private async Task LoadVideoUrls()
        {
            var drills = await _drillApiService.GetAllAsync(_user.Sport);

            var filtered = drills
                .Where(d => !string.IsNullOrWhiteSpace(d.VideoUrl))
                .ToList();

            _drillNames.Clear();
            _videoUrls.Clear();

            if (_selectedDrillNames.Count > 0)
            {
                foreach (var selectedNameRaw in _selectedDrillNames)
                {
                    string selectedName = selectedNameRaw?.Trim() ?? "";

                    selectedName = Regex.Replace(
                        selectedName,
                        @"\s*\(\d+\s*min\)\s*$",
                        "",
                        RegexOptions.IgnoreCase).Trim();

                    var match = filtered.FirstOrDefault(d =>
                        !string.IsNullOrWhiteSpace(d.Name) &&
                        d.Name.Trim().Equals(selectedName, StringComparison.OrdinalIgnoreCase));

                    if (match == null)
                    {
                        match = filtered.FirstOrDefault(d =>
                            !string.IsNullOrWhiteSpace(d.Name) &&
                            selectedName.Contains(d.Name.Trim(), StringComparison.OrdinalIgnoreCase));
                    }

                    if (match != null)
                    {
                        _drillNames.Add(match.Name);
                        _videoUrls.Add(match.VideoUrl);
                    }
                }
            }
            else
            {
                var grouped = filtered
                    .GroupBy(d => d.Category)
                    .SelectMany(g => g.Take(5));

                foreach (var drill in grouped)
                {
                    _drillNames.Add(drill.Name);
                    _videoUrls.Add(drill.VideoUrl);
                }
            }

            _currentIndex = _drillNames.Count > 0 ? 0 : -1;
        }

        private void PopulateDrillList()
        {
            lstDrills.Items.Clear();

            for (int i = 0; i < _drillNames.Count; i++)
            {
                lstDrills.Items.Add($"{i + 1}. {_drillNames[i]}");
            }
        }

        private void lstDrills_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstDrills.SelectedIndex < 0)
                return;

            _currentIndex = lstDrills.SelectedIndex;
            UpdateSelectionUi();
        }

        private void UpdateSelectionUi()
        {
            if (_currentIndex < 0 || _currentIndex >= _drillNames.Count)
                return;

            btnStart.Enabled = true;
            btnPrev.Enabled = _currentIndex > 0;
            btnNext.Enabled = _currentIndex < _drillNames.Count - 1;
            lstDrills.Invalidate();
        }

        private void lstDrills_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= lstDrills.Items.Count)
                return;

            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            bool isCurrent = e.Index == _currentIndex;

            Color backColor = isSelected
                ? Color.FromArgb(32, 124, 229)
                : Color.FromArgb(17, 31, 54);

            Color textColor = isSelected
                ? Color.White
                : (isCurrent ? Color.FromArgb(190, 220, 255) : Color.White);

            using (SolidBrush backBrush = new SolidBrush(backColor))
            using (SolidBrush textBrush = new SolidBrush(textColor))
            using (Font itemFont = new Font("Segoe UI", isCurrent ? 11f : 10.5f, isCurrent ? FontStyle.Bold : FontStyle.Regular))
            using (StringFormat sf = new StringFormat { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Near })
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);

                Rectangle textRect = new Rectangle(
                    e.Bounds.X + 16,
                    e.Bounds.Y + 2,
                    e.Bounds.Width - 24,
                    e.Bounds.Height - 4);

                e.Graphics.DrawString(
                    lstDrills.Items[e.Index].ToString(),
                    itemFont,
                    textBrush,
                    textRect,
                    sf);
            }

            e.DrawFocusRectangle();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (lstDrills.SelectedIndex >= 0)
            {
                _currentIndex = lstDrills.SelectedIndex;
            }

            if (_currentIndex < 0 || _currentIndex >= _videoUrls.Count)
            {
                MessageBox.Show(
                    $"No valid video found.\nCurrent Index: {_currentIndex}\nVideo Count: {_videoUrls.Count}",
                    "Video Debug",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            LoadVideoInPlayer(_videoUrls[_currentIndex]);
            UpdateSelectionUi();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (_drillNames.Count == 0 || _currentIndex <= 0)
                return;

            _currentIndex--;
            lstDrills.SelectedIndex = _currentIndex;
            LoadVideoInPlayer(_videoUrls[_currentIndex]);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (_drillNames.Count == 0 || _currentIndex >= _drillNames.Count - 1)
                return;

            _currentIndex++;
            lstDrills.SelectedIndex = _currentIndex;
            LoadVideoInPlayer(_videoUrls[_currentIndex]);
        }

        private async void LoadVideoInPlayer(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                MessageBox.Show("Video URL is empty.", "Video Debug");
                return;
            }

            if (videoView.CoreWebView2 == null)
            {
                await videoView.EnsureCoreWebView2Async(null);
            }

            string videoId = ExtractVideoId(url);
            if (string.IsNullOrWhiteSpace(videoId))
            {
                MessageBox.Show($"Could not extract video ID from URL:\n{url}", "Video Debug");
                return;
            }

            string embedUrl = $"https://www.youtube-nocookie.com/embed/{videoId}?autoplay=1&mute=1&rel=0&controls=1";

            string tempHtmlPath = Path.Combine(Application.StartupPath, "youtube-player.html");

            string htmlContent = $@"<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='referrer' content='strict-origin-when-cross-origin'>
    <style>
        html, body {{
            margin: 0;
            padding: 0;
            width: 100%;
            height: 100%;
            overflow: hidden;
            background-color: #000;
        }}
        iframe {{
            width: 100%;
            height: 100%;
            border: none;
            display: block;
        }}
    </style>
</head>
<body>
    <iframe
        src='{embedUrl}'
        referrerpolicy='strict-origin-when-cross-origin'
        allow='autoplay; encrypted-media; picture-in-picture'
        allowfullscreen>
    </iframe>
</body>
</html>";

            try
            {
                File.WriteAllText(tempHtmlPath, htmlContent);
                videoView.CoreWebView2.Navigate("https://myapp.local/youtube-player.html");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Embedded playback failed.\n{ex.Message}", "Video Debug");
                OpenInBrowser(url);
            }
        }

        private string ExtractVideoId(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return "";

            if (url.Contains("youtube.com/watch?v=", StringComparison.OrdinalIgnoreCase))
            {
                return url.Split(new[] { "v=" }, StringSplitOptions.None)[1].Split('&')[0];
            }
            else if (url.Contains("youtu.be/", StringComparison.OrdinalIgnoreCase))
            {
                return url.Split('/').Last().Split('?')[0];
            }
            else if (url.Contains("youtube.com/shorts/", StringComparison.OrdinalIgnoreCase))
            {
                return url.Split(new[] { "shorts/" }, StringSplitOptions.None)[1].Split('?')[0];
            }
            else if (url.Contains("/embed/", StringComparison.OrdinalIgnoreCase))
            {
                return url.Split(new[] { "/embed/" }, StringSplitOptions.None)[1].Split('?')[0];
            }

            return "";
        }

        private void OpenInBrowser(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not open video in browser.\n{ex.Message}", "Video Debug");
            }
        }
    }
}