using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using SkillBuilderPro.Client.Services;
using SkillBuilderPro.WinForms.Services;
using SkillBuilderPro.WinForms.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Drill = SkillBuilderPro.Core.Models.Drill;
using User = SkillBuilderPro.WinForms.Models.User;

namespace SkillBuilderPro.WinForms.Forms
{
    public partial class VideoPlayerForm : Form
    {
        private static readonly Size FilmRoomSourceSize = new(1672, 941);
        private static readonly RectangleF FilmRoomVideoBounds = new(516, 220, 640, 360);
        private const bool ShowFilmRoomAlignmentDiagnostics = false;
        private readonly Image _filmRoomBackground = Properties.Resource1.drill_library;
        private readonly User _user;
        private readonly bool _isDemoMode;
        private readonly List<string> _selectedDrillNames;
        private readonly List<Drill> _drills;
        private int _currentIndex = -1;
        private readonly DrillApiService _drillApiService;
        private Task? _webViewReadyTask;
        private int _navigationGeneration;
        private bool _isDisposed;

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

        public VideoPlayerForm(User user, List<string> drillNames, bool isDemoMode = false)
        {
            _user = user;
            _isDemoMode = isDemoMode;
            _selectedDrillNames = drillNames ?? new List<string>();
            _drills = new List<Drill>();

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
            ClientSize = FilmRoomSourceSize;
            StartPosition = FormStartPosition.CenterScreen;
            Name = "VideoPlayerForm";
            Text = "Training Videos";
            DoubleBuffered = true;
            Load += VideoPlayerForm_Load;
            FormClosed += VideoPlayerForm_FormClosed;
            Resize += (_, _) => LayoutFilmRoomControls();

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
            pnlDrillList.Dock = DockStyle.None;
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
            pnlVideoHost.Dock = DockStyle.None;
            pnlVideoHost.Margin = new Padding(0);

            pnlVideoHost.Padding = new Padding(2);
            pnlVideoHost.BackColor = Color.FromArgb(14, 20, 30);

            videoView.Dock = DockStyle.Fill;
            videoView.BackColor = Color.Black;
            videoView.DefaultBackgroundColor = Color.Black;
            videoView.Visible = true;
            videoView.AllowExternalDrop = false;

            pnlVideoHost.Controls.Add(videoView);

            // Controls panel
            pnlControls.Dock = DockStyle.None;
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

            Controls.Add(pnlDrillList);
            Controls.Add(pnlVideoHost);
            Controls.Add(pnlControls);

            LayoutFilmRoomControls();
            CenterBottomControls();

            ResumeLayout(false);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.FromArgb(5, 10, 18));
            BackgroundRenderHelper.DrawAspectFill(e.Graphics, _filmRoomBackground, ClientRectangle);

            if (ShowFilmRoomAlignmentDiagnostics)
            {
                Rectangle rendered = BackgroundRenderHelper.AspectFill(FilmRoomSourceSize, ClientRectangle);
                using var sourcePen = new Pen(Color.Orange, 2);
                using var videoPen = new Pen(Color.DeepSkyBlue, 2);
                e.Graphics.DrawRectangle(sourcePen, rendered);
                e.Graphics.DrawRectangle(videoPen, SourceBoundsToRendered(rendered, FilmRoomVideoBounds));
            }
        }

        private void LayoutFilmRoomControls()
        {
            if (ClientSize.Width <= 0 || ClientSize.Height <= 0) return;

            Rectangle rendered = BackgroundRenderHelper.AspectFill(FilmRoomSourceSize, ClientRectangle);
            Rectangle videoBounds = SourceBoundsToRendered(rendered, FilmRoomVideoBounds);
            pnlVideoHost.Bounds = videoBounds;

            int selectionWidth = Math.Clamp(videoBounds.Left - rendered.Left - 42, 230, 330);
            int selectionX = Math.Max(16, videoBounds.Left - selectionWidth - 22);
            pnlDrillList.Bounds = new Rectangle(selectionX, videoBounds.Top, selectionWidth, videoBounds.Height);

            int controlsY = Math.Min(ClientSize.Height - 66, videoBounds.Bottom + 10);
            pnlControls.Bounds = new Rectangle(videoBounds.Left, controlsY, videoBounds.Width, 60);
            CenterBottomControls();
            Invalidate();
        }

        private static Rectangle SourceBoundsToRendered(Rectangle rendered, RectangleF sourceBounds)
        {
            double scale = (double)rendered.Width / FilmRoomSourceSize.Width;
            return new Rectangle(
                rendered.Left + (int)Math.Round(sourceBounds.X * scale),
                rendered.Top + (int)Math.Round(sourceBounds.Y * scale),
                (int)Math.Round(sourceBounds.Width * scale),
                (int)Math.Round(sourceBounds.Height * scale));
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
            LayoutFilmRoomControls();

            if (!await EnsurePlayerReadyAsync())
                return;

            if (!_isDemoMode && !await EnsureServicesAvailableAsync())
            {
                ShowPlayerMessage("Skill Builder Pro services are currently unavailable. Start the API and try again.");
                return;
            }

            await LoadVideoUrls();
            PopulateDrillList();
            AdjustDrillPanelHeight();
            CenterBottomControls();

            if (_drills.Count > 0)
            {
                _currentIndex = 0;
                lstDrills.SelectedIndex = 0;
                btnStart.Enabled = true;
                btnPrev.Enabled = false;
                btnNext.Enabled = _drills.Count > 1;
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
            int visibleRows = Math.Min(Math.Max(_drills.Count, 1), 4);
            int headerHeight = 30;
            int topPadding = 10;
            int bottomPadding = 10;
            int listHeight = visibleRows * lstDrills.ItemHeight + 6;
            int totalHeight = headerHeight + topPadding + bottomPadding + listHeight;

            mainLayout.RowStyles[0].Height = Math.Max(120, totalHeight);
        }

        private async Task LoadVideoUrls()
        {
            IEnumerable<Drill> drills;
            if (_isDemoMode)
            {
                drills = DrillDatabase.GetDrillsBySport(_user.Sport).Select((drill, index) => new Drill
                {
                    Id = 900000 + index,
                    Name = drill.Name,
                    Sport = drill.Sport,
                    Category = drill.SkillCategory,
                    Description = drill.Description,
                    Difficulty = drill.Difficulty,
                    Duration = drill.Duration > 0 ? $"{drill.Duration}:00" : "10:00",
                    VideoUrl = drill.VideoUrl
                });
            }
            else
            {
                drills = await _drillApiService.GetAllAsync(_user.Sport);
            }

            var filtered = drills
                .Where(d => !string.IsNullOrWhiteSpace(d.VideoUrl))
                .ToList();

            _drills.Clear();

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
                        _drills.Add(match);
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
                    _drills.Add(drill);
                }
            }

            _currentIndex = _drills.Count > 0 ? 0 : -1;
        }

        private void PopulateDrillList()
        {
            lstDrills.Items.Clear();

            for (int i = 0; i < _drills.Count; i++)
            {
                lstDrills.Items.Add($"{i + 1}. {_drills[i].Name}");
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
            if (_currentIndex < 0 || _currentIndex >= _drills.Count)
                return;

            btnStart.Enabled = true;
            btnPrev.Enabled = _drills.Count > 1;
            btnNext.Enabled = _drills.Count > 1;
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

        private async void btnStart_Click(object sender, EventArgs e)
        {
            if (lstDrills.SelectedIndex >= 0)
            {
                _currentIndex = lstDrills.SelectedIndex;
            }

            if (_currentIndex < 0 || _currentIndex >= _drills.Count)
            {
                MessageBox.Show("Select a drill before starting its training video.", "Training Video",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            await LoadVideoIntoPlayerAsync(_drills[_currentIndex]);
            UpdateSelectionUi();
        }

        private static async Task<bool> EnsureServicesAvailableAsync()
        {
            using var healthClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5000/"),
                Timeout = TimeSpan.FromSeconds(3)
            };

            while (true)
            {
                try
                {
                    using var response = await healthClient.GetAsync("health");
                    if (response.IsSuccessStatusCode) return true;
                }
                catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
                {
                    // The restrained message below intentionally hides socket details.
                }

                if (MessageBox.Show(
                        "Skill Builder Pro services are currently unavailable. Start the API and try again.",
                        "Services Unavailable",
                        MessageBoxButtons.RetryCancel,
                        MessageBoxIcon.Information) != DialogResult.Retry)
                    return false;
            }
        }

        private async void btnPrev_Click(object sender, EventArgs e)
        {
            if (_drills.Count == 0)
                return;

            _currentIndex = (_currentIndex - 1 + _drills.Count) % _drills.Count;
            lstDrills.SelectedIndex = _currentIndex;
            await LoadVideoIntoPlayerAsync(_drills[_currentIndex]);
        }

        private async void btnNext_Click(object sender, EventArgs e)
        {
            if (_drills.Count == 0)
                return;

            _currentIndex = (_currentIndex + 1) % _drills.Count;
            lstDrills.SelectedIndex = _currentIndex;
            await LoadVideoIntoPlayerAsync(_drills[_currentIndex]);
        }

        private async Task LoadVideoIntoPlayerAsync(Drill drill)
        {
            int requestGeneration = Interlocked.Increment(ref _navigationGeneration);

            if (string.IsNullOrWhiteSpace(drill.VideoUrl))
            {
                ShowPlayerMessage("This drill does not have a training video.");
                return;
            }

            if (!TryExtractYouTubeVideoId(drill.VideoUrl, out string videoId))
            {
                ShowPlayerMessage("This drill contains an invalid YouTube video URL.");
                return;
            }

            if (!await EnsurePlayerReadyAsync() || requestGeneration != _navigationGeneration || _isDisposed)
                return;

            string origin = Uri.EscapeDataString("https://player.skillbuilderpro.local");
            string embedUrl = $"https://www.youtube-nocookie.com/embed/{videoId}?autoplay=1&mute=1&rel=0&controls=1&playsinline=1&origin={origin}";
            string scriptArgument = JsonSerializer.Serialize(embedUrl);

            try
            {
                Debug.WriteLine($"Loading embedded training video for Drill {drill.Id} '{drill.Name}', YouTube ID {videoId}.");
                await videoView.CoreWebView2.ExecuteScriptAsync($"window.sbpLoadVideo({scriptArgument});");
            }
            catch (Exception ex) when (!_isDisposed)
            {
                Debug.WriteLine($"Embedded video navigation failed: {ex}");
                ShowPlayerMessage("The training video could not be loaded. Select another drill and try again.");
            }
        }

        private Task<bool> EnsurePlayerReadyAsync()
        {
            _webViewReadyTask ??= InitializePlayerAsync();
            return AwaitPlayerInitializationAsync();
        }

        private async Task<bool> AwaitPlayerInitializationAsync()
        {
            try
            {
                await _webViewReadyTask!;
                return !_isDisposed && videoView.CoreWebView2 is not null;
            }
            catch (Exception ex)
            {
                _webViewReadyTask = null;
                Debug.WriteLine($"WebView2 initialization failed: {ex}");
                MessageBox.Show("The training video player could not be initialized.", "Training Video",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        private async Task InitializePlayerAsync()
        {
            await videoView.EnsureCoreWebView2Async(null);
            if (_isDisposed || videoView.CoreWebView2 is null)
                return;

            string playerFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "SkillBuilderPro", "WebPlayer");
            Directory.CreateDirectory(playerFolder);
            string playerPath = Path.Combine(playerFolder, "player.html");
            File.WriteAllText(playerPath, PlayerHtml);

            videoView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                "player.skillbuilderpro.local",
                playerFolder,
                CoreWebView2HostResourceAccessKind.DenyCors);

            var ready = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            void NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs args)
            {
                videoView.CoreWebView2.NavigationCompleted -= NavigationCompleted;
                if (args.IsSuccess)
                    ready.TrySetResult();
                else
                    ready.TrySetException(new InvalidOperationException($"Player navigation failed: {args.WebErrorStatus}"));
            }

            videoView.CoreWebView2.NavigationCompleted += NavigationCompleted;
            videoView.CoreWebView2.Navigate("https://player.skillbuilderpro.local/player.html");
            await ready.Task;
        }

        internal static bool TryExtractYouTubeVideoId(string? value, out string videoId)
        {
            videoId = string.Empty;
            string candidate = value?.Trim() ?? string.Empty;
            if (Regex.IsMatch(candidate, "^[A-Za-z0-9_-]{11}$"))
            {
                videoId = candidate;
                return true;
            }

            if (!Uri.TryCreate(candidate, UriKind.Absolute, out Uri? uri) ||
                (uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp))
                return false;

            string host = uri.Host.ToLowerInvariant();
            if (host.StartsWith("www.")) host = host[4..];
            if (host.StartsWith("m.")) host = host[2..];

            string? extracted = null;
            if (host == "youtu.be")
                extracted = uri.AbsolutePath.Trim('/').Split('/')[0];
            else if (host is "youtube.com" or "youtube-nocookie.com")
            {
                string[] segments = uri.AbsolutePath.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (uri.AbsolutePath.Equals("/watch", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (string part in uri.Query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
                    {
                        string[] pair = part.Split('=', 2);
                        if (pair.Length == 2 && pair[0].Equals("v", StringComparison.OrdinalIgnoreCase))
                        {
                            extracted = Uri.UnescapeDataString(pair[1]);
                            break;
                        }
                    }
                }
                else if (segments.Length >= 2 &&
                         (segments[0].Equals("embed", StringComparison.OrdinalIgnoreCase) ||
                          segments[0].Equals("shorts", StringComparison.OrdinalIgnoreCase)))
                    extracted = segments[1];
            }

            if (extracted is null || !Regex.IsMatch(extracted, "^[A-Za-z0-9_-]{11}$"))
                return false;

            videoId = extracted;
            return true;
        }

        private void ShowPlayerMessage(string message)
        {
            if (_isDisposed) return;
            if (videoView.CoreWebView2 is not null)
            {
                string argument = JsonSerializer.Serialize(message);
                _ = videoView.CoreWebView2.ExecuteScriptAsync($"window.sbpShowMessage({argument});");
            }
            else
            {
                MessageBox.Show(message, "Training Video", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void VideoPlayerForm_FormClosed(object? sender, FormClosedEventArgs e)
        {
            _isDisposed = true;
            Interlocked.Increment(ref _navigationGeneration);
        }

        private const string PlayerHtml = """
<!doctype html><html><head><meta charset="utf-8"><meta name="referrer" content="strict-origin-when-cross-origin">
<style>html,body{margin:0;width:100%;height:100%;overflow:hidden;background:#000;color:#dce7f5;font:16px Segoe UI,sans-serif}#host{width:100%;height:100%}iframe{width:100%;height:100%;border:0;display:block}.message{height:100%;display:flex;align-items:center;justify-content:center;text-align:center;padding:24px;box-sizing:border-box}</style>
</head><body><div id="host"><div class="message">Select a drill and choose START VIDEO.</div></div>
<script>window.sbpShowMessage=function(m){document.getElementById('host').innerHTML='';const d=document.createElement('div');d.className='message';d.textContent=m;document.getElementById('host').appendChild(d)};window.sbpLoadVideo=function(url){const h=document.getElementById('host');h.innerHTML='';const f=document.createElement('iframe');f.src=url;f.title='Skill Builder Pro training video';f.referrerPolicy='strict-origin-when-cross-origin';f.allow='autoplay; encrypted-media; picture-in-picture; fullscreen';f.allowFullscreen=true;h.appendChild(f)};</script></body></html>
""";
    }
}
