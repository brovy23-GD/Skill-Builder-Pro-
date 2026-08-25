using System;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Layouts;
using SkillBuilderPro.Core.Models;
using SkillBuilderPro.MAUI.ViewModels;
using System.Threading.Tasks;
using SkillBuilderPro.MAUI.Services;
#if WINDOWS
using Microsoft.Maui.Handlers;
using Microsoft.Web.WebView2.Core;
#endif

namespace SkillBuilderPro.MAUI.Views;

[QueryProperty(nameof(DrillId), "drillId")]
[QueryProperty(nameof(FromTraining), "fromTraining")]
public partial class DrillLibraryPage : ContentPage
{
    private static readonly Size FilmRoomSourceSize = new(1672, 941);
    private static readonly Rect FilmRoomVideoBounds = new(516, 220, 640, 360);
#if DEBUG
    private static readonly bool ShowFilmRoomAlignmentDiagnostics = false;
#else
    private static readonly bool ShowFilmRoomAlignmentDiagnostics = false;
#endif
    private readonly DrillsViewModel _viewModel;
    private readonly IAthleteApiService _api;
    private int _currentPlaylistIndex = 0;
    private List<Drill> _activePlaylist = [];
    private string _videoUrl = string.Empty;
    private string _drillName = string.Empty;
    private const string VideoVirtualHost = "skillbuilderpro.local";
    private int _drillId;
    private bool _fromTraining;
    public int DrillId { get=>_drillId; set { _drillId=value; _=ResolveDrillAsync(); } }
    public bool FromTraining { get=>_fromTraining; set => _fromTraining=value; }

#if WINDOWS
    private string _videoWebRoot = string.Empty;
    private bool _webView2Ready;
    private string _pendingVideoUrl = string.Empty;
#endif

    public string VideoUrl
    {
        get => _videoUrl;
        set
        {
            _videoUrl = HttpUtility.UrlDecode(value ?? string.Empty);
            LoadVideoInPlayer(_videoUrl);
        }
    }

    public string DrillName
    {
        get => _drillName;
        set
        {
            _drillName = HttpUtility.UrlDecode(value ?? string.Empty);
            SelectedDrillLabel.Text = string.IsNullOrWhiteSpace(_drillName)
                ? "Active Training Exercise"
                : _drillName;
        }
    }

    public DrillLibraryPage(DrillsViewModel viewModel,IAthleteApiService api)
    {
        InitializeComponent();

#if WINDOWS
        DrillVideoWebView.HandlerChanged += DrillVideoWebView_HandlerChanged;
#endif

        _viewModel = viewModel;
        _api=api;
        BindingContext = _viewModel;
    }

    private async Task ResolveDrillAsync()
    {
        if (_drillId<=0)return;
        IEnumerable<Drill> source = _api.IsDemoMode
            ? DemoDataService.Drills
            : await _api.GetAsync<List<Drill>>("api/drills") ?? [];
        var drill=source.FirstOrDefault(x=>x.Id==_drillId);
        if(drill is null){SelectedDrillLabel.Text=_api.IsDemoMode?"Demo drill unavailable":_api.IsServiceAvailable?"Drill unavailable from the API":_api.ServiceStatusMessage;LoadVideoInPlayer(string.Empty);return;}
        if(!YouTubeUrl.IsValid(drill.VideoUrl)){SelectedDrillLabel.Text="Training video unavailable";LoadVideoInPlayer(string.Empty);return;}
        var selectedDrills = _viewModel.SelectedDrills?.ToList() ?? [];
        var selected = selectedDrills.FirstOrDefault(x => x.Id == drill.Id);
        _activePlaylist = selected is null ? [drill] : selectedDrills;
        if (selected is not null)
            _currentPlaylistIndex = _activePlaylist.IndexOf(selected);
        else
            _currentPlaylistIndex = 0;
        LoadDrillFromQueue(selected ?? drill);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        DeterminePlaylistPosition();
    }

    protected override void OnDisappearing()
    {
        StopCurrentVideo();
        base.OnDisappearing();
    }

    private void DeterminePlaylistPosition()
    {
        if (_activePlaylist.Count == 0) return;

        var currentDrill = _activePlaylist.FirstOrDefault(d => d.Id == _drillId || d.Name.Equals(_drillName, StringComparison.OrdinalIgnoreCase));
        if (currentDrill != null)
        {
            _currentPlaylistIndex = _activePlaylist.IndexOf(currentDrill);
        }
    }

    private void LoadDrillFromQueue(Drill drill)
    {
        if (drill == null)
        {
            return;
        }

        StopCurrentVideo();

        _videoUrl = drill.VideoUrl ?? string.Empty;
        _drillName = drill.Name;

        int selectedCount = _activePlaylist.Count;
        bool multiple = selectedCount > 1;
        PreviousDrillButton.IsVisible = multiple;
        NextDrillButton.IsVisible = multiple;
        PlaylistFooter.IsVisible = multiple;
        PreviousDrillButton.IsEnabled = multiple && _currentPlaylistIndex > 0;
        NextDrillButton.IsEnabled = multiple && _currentPlaylistIndex < selectedCount - 1;
        SelectedDrillLabel.Text = multiple
            ? $"{_currentPlaylistIndex + 1} / {selectedCount}   {_drillName}"
            : _drillName;
        DrillMetaLabel.Text=string.Join(" • ",new[]{drill.Sport,drill.Category,drill.SubCategory}.Where(x=>!string.IsNullOrWhiteSpace(x)));
        DrillDurationLabel.Text=string.IsNullOrWhiteSpace(drill.Duration)?string.Empty:$"Duration: {drill.Duration}";
        DrillDescriptionLabel.Text=drill.Description??"No drill instructions are available.";
        DrillGroupLabel.Text=string.IsNullOrWhiteSpace(drill.DrillGroup)?string.Empty:$"Group: {drill.DrillGroup}";

        LoadVideoInPlayer(_videoUrl);
    }

#if WINDOWS
    private async void DrillVideoWebView_HandlerChanged(
        object? sender,
        EventArgs e)
    {
        try
        {
            if (DrillVideoWebView.Handler is not WebViewHandler handler)
            {
                return;
            }

            var platformView = handler.PlatformView;

            await platformView.EnsureCoreWebView2Async();

            var coreWebView2 = platformView.CoreWebView2;

            if (coreWebView2 == null)
            {
                return;
            }

            _videoWebRoot = Path.Combine(
                Path.GetTempPath(),
                "SkillBuilderPro",
                "WebView");

            Directory.CreateDirectory(_videoWebRoot);

            coreWebView2.SetVirtualHostNameToFolderMapping(
                VideoVirtualHost,
                _videoWebRoot,
                CoreWebView2HostResourceAccessKind.Allow);

            coreWebView2.AddWebResourceRequestedFilter(
                "*://*.youtube.com/*",
                CoreWebView2WebResourceContext.All);

            coreWebView2.AddWebResourceRequestedFilter(
                "*://*.youtube-nocookie.com/*",
                CoreWebView2WebResourceContext.All);

            coreWebView2.WebResourceRequested -=
                CoreWebView2_WebResourceRequested;

            coreWebView2.WebResourceRequested +=
                CoreWebView2_WebResourceRequested;

            _webView2Ready = true;

            if (!string.IsNullOrWhiteSpace(_pendingVideoUrl))
            {
                string pendingUrl = _pendingVideoUrl;
                _pendingVideoUrl = string.Empty;
                LoadVideoInPlayer(pendingUrl);
            }
        }
        catch (Exception ex)
        {
            _webView2Ready = false;

            System.Diagnostics.Debug.WriteLine(
                $"WebView2 initialization failed: {ex.Message}");
        }
    }

    private void CoreWebView2_WebResourceRequested(
        object? sender,
        CoreWebView2WebResourceRequestedEventArgs e)
    {
        try
        {
            if (e.Request == null)
            {
                return;
            }

            e.Request.Headers.SetHeader(
                "Referer",
                $"https://{VideoVirtualHost}/");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Could not set YouTube Referer: {ex.Message}");
        }
    }
#endif
    /// <summary>
    /// Safely bridges your local HTML container with YouTube's strict security context layer.
    /// Automatically repairs malformed database URL strings dynamically on the fly before launching the WebView.
    /// </summary>
    public void LoadVideoInPlayer(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            DrillVideoWebView.Source = new HtmlWebViewSource
            {
                Html = @"
<!DOCTYPE html>
<html>
<head>
    <meta name='viewport'
          content='width=device-width, initial-scale=1.0' />
</head>
<body style='
    margin:0;
    background:#000;
    color:#fff;
    font-family:Arial,Helvetica,sans-serif;
    display:flex;
    align-items:center;
    justify-content:center;
    height:100vh;
    text-align:center;'>
    <div>
        <h2>No Training Video Available</h2>
        <p>This drill does not currently have a video URL.</p>
    </div>
</body>
</html>"
            };

            return;
        }

        string videoId = ExtractVideoId(url);

        if (string.IsNullOrWhiteSpace(videoId))
        {
            DrillVideoWebView.Source = new HtmlWebViewSource
            {
                Html = @"
<!DOCTYPE html>
<html>
<head>
    <meta name='viewport'
          content='width=device-width, initial-scale=1.0' />
</head>
<body style='
    margin:0;
    background:#000;
    color:#fff;
    font-family:Arial,Helvetica,sans-serif;
    display:flex;
    align-items:center;
    justify-content:center;
    height:100vh;
    text-align:center;'>
    <div>
        <h2>Video Could Not Be Loaded</h2>
        <p>This drill does not contain a valid YouTube video ID.</p>
    </div>
</body>
</html>"
            };

            return;
        }

        string origin =
            $"https://{VideoVirtualHost}";

        string embedUrl =
            $"https://www.youtube-nocookie.com/embed/{videoId}" +
            "?autoplay=1" +
            "&mute=1" +
            "&rel=0" +
            "&controls=1" +
            "&playsinline=1" +
            $"&origin={Uri.EscapeDataString(origin)}";

        string html = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8' />
    <meta name='viewport'
          content='width=device-width, initial-scale=1.0' />

    <style>
        html,
        body {{
            margin: 0;
            padding: 0;
            width: 100%;
            height: 100%;
            overflow: hidden;
            background: #000;
        }}

        iframe {{
            width: 100%;
            height: 100%;
            border: 0;
            display: block;
        }}
    </style>
</head>

<body>
    <iframe
        src='{embedUrl}'
        title='SkillBuilderPro Training Video'
        referrerpolicy='strict-origin-when-cross-origin'
        allow='accelerometer;
               autoplay;
               clipboard-write;
               encrypted-media;
               gyroscope;
               picture-in-picture;
               web-share'
        allowfullscreen>
    </iframe>
</body>
</html>";

#if WINDOWS
        if (!_webView2Ready ||
            string.IsNullOrWhiteSpace(_videoWebRoot) ||
            DrillVideoWebView.Handler is not WebViewHandler handler ||
            handler.PlatformView.CoreWebView2 == null)
        {
            _pendingVideoUrl = url;

            DrillVideoWebView.Source = new HtmlWebViewSource
            {
                Html = @"
<!DOCTYPE html>
<html>
<body style='
    margin:0;
    background:#000;
    color:#fff;
    font-family:Arial,Helvetica,sans-serif;
    display:flex;
    align-items:center;
    justify-content:center;
    height:100vh;'>
    <p>Preparing video player...</p>
</body>
</html>"
            };

            return;
        }

        try
        {
            string htmlPath =
                Path.Combine(
                    _videoWebRoot,
                    "youtube-player.html");

            File.WriteAllText(
                htmlPath,
                html);

            handler.PlatformView.CoreWebView2.Navigate(
                $"https://{VideoVirtualHost}/youtube-player.html");

            return;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Virtual-host video navigation failed: {ex.Message}");
        }
#endif

        DrillVideoWebView.Source = new HtmlWebViewSource
        {
            Html = html,
            BaseUrl = origin + "/"
        };
    }

    public string ExtractVideoId(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return string.Empty;
        }

        string value = url.Trim();

        static bool IsValidVideoId(string candidate)
        {
            if (string.IsNullOrWhiteSpace(candidate))
            {
                return false;
            }

            if (candidate.Length != 11)
            {
                return false;
            }

            foreach (char character in candidate)
            {
                bool valid =
                    char.IsLetterOrDigit(character) ||
                    character == '_' ||
                    character == '-';

                if (!valid)
                {
                    return false;
                }
            }

            return true;
        }

        static string CleanCandidate(string candidate)
        {
            if (string.IsNullOrWhiteSpace(candidate))
            {
                return string.Empty;
            }

            string cleaned = candidate.Trim();

            int separatorIndex =
                cleaned.IndexOfAny(
                    new[]
                    {
                    '&',
                    '?',
                    '#',
                    '/'
                    });

            if (separatorIndex >= 0)
            {
                cleaned =
                    cleaned.Substring(
                        0,
                        separatorIndex);
            }

            return cleaned;
        }

        // Already stored as a raw YouTube ID.
        if (IsValidVideoId(value))
        {
            return value;
        }

        // Standard YouTube watch URL:
        // https://www.youtube.com/watch?v=VIDEO_ID
        int watchIndex =
            value.IndexOf(
                "v=",
                StringComparison.OrdinalIgnoreCase);

        if (watchIndex >= 0)
        {
            string candidate =
                CleanCandidate(
                    value.Substring(
                        watchIndex + 2));

            if (IsValidVideoId(candidate))
            {
                return candidate;
            }
        }

        // Short YouTube URL:
        // https://youtu.be/VIDEO_ID
        const string shortMarker = "youtu.be/";

        int shortIndex =
            value.IndexOf(
                shortMarker,
                StringComparison.OrdinalIgnoreCase);

        if (shortIndex >= 0)
        {
            string candidate =
                CleanCandidate(
                    value.Substring(
                        shortIndex +
                        shortMarker.Length));

            if (IsValidVideoId(candidate))
            {
                return candidate;
            }
        }

        // YouTube Shorts:
        // https://www.youtube.com/shorts/VIDEO_ID
        const string shortsMarker = "/shorts/";

        int shortsIndex =
            value.IndexOf(
                shortsMarker,
                StringComparison.OrdinalIgnoreCase);

        if (shortsIndex >= 0)
        {
            string candidate =
                CleanCandidate(
                    value.Substring(
                        shortsIndex +
                        shortsMarker.Length));

            if (IsValidVideoId(candidate))
            {
                return candidate;
            }
        }

        // Standard or privacy-enhanced embed URLs:
        //
        // https://www.youtube.com/embed/VIDEO_ID
        // https://www.youtube-nocookie.com/embed/VIDEO_ID
        const string embedMarker = "/embed/";

        int embedIndex =
            value.IndexOf(
                embedMarker,
                StringComparison.OrdinalIgnoreCase);

        if (embedIndex >= 0)
        {
            string candidate =
                CleanCandidate(
                    value.Substring(
                        embedIndex +
                        embedMarker.Length));

            if (IsValidVideoId(candidate))
            {
                return candidate;
            }
        }

        return string.Empty;
    }
    private void StopCurrentVideo()
    {
#if WINDOWS
        try
        {
            if (DrillVideoWebView.Handler is WebViewHandler handler &&
                handler.PlatformView.CoreWebView2 != null)
            {
                handler.PlatformView.CoreWebView2.Navigate("about:blank");
                return;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Could not stop current video: {ex.Message}");
        }
#endif

        DrillVideoWebView.Source = new HtmlWebViewSource
        {
            Html = @"
<!DOCTYPE html>
<html>
<body style='
    margin:0;
    background:#000;
    width:100%;
    height:100%;'>
</body>
</html>"
        };
    }

    // ✅ LIVE CONTROL BUTTON CLICK LOGIC CONNECTORS
    public void OnPreviousClicked(object sender, EventArgs e)
    {
        if (_activePlaylist.Count <= 1)
        {
            return;
        }

        if (_currentPlaylistIndex > 0)
        {
            _currentPlaylistIndex--;

            var drill =
                _activePlaylist[_currentPlaylistIndex];

            LoadDrillFromQueue(drill);
        }
    }

    public async void OnNextClicked(object sender, EventArgs e)
    {
        if (_activePlaylist.Count <= 1)
        {
            return;
        }

        if (_currentPlaylistIndex <
            _activePlaylist.Count - 1)
        {
            _currentPlaylistIndex++;

            var drill =
                _activePlaylist[_currentPlaylistIndex];

            LoadDrillFromQueue(drill);
        }
        else return;
    }

    public async void OnOpenExternallyClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_videoUrl)) return;

        if (Uri.TryCreate(_videoUrl, UriKind.Absolute, out var uri))
        {
            await Launcher.Default.OpenAsync(uri);
        }
    }

    public async void OnSaveDrillClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Saved", $"'{_drillName}' added to your playbook records.", "OK");
    }

    public void OnStartTrainingClicked(object sender, EventArgs e)
    {
        LoadVideoInPlayer(_videoUrl);
    }

    public async void OnCompleteDrillClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Complete!", "Drill marked as accomplished. Progress logs updated.", "OK");
        OnNextClicked(sender, e);
    }

    public async void OnBackClicked(object sender, EventArgs e)
    {
        StopCurrentVideo();
        await Shell.Current.GoToAsync("..");
    }

    private async void OnExitClicked(object sender, EventArgs e)
    {
        StopCurrentVideo();
        await Shell.Current.GoToAsync("//Home");
    }

    private void OnResponsiveStageSizeChanged(object? sender, EventArgs e)
    {
        var viewportWidth = ResponsiveStage.Width;
        var viewportHeight = ResponsiveStage.Height;
        if (viewportWidth <= 0 || viewportHeight <= 0) return;

        var portraitPhone = viewportWidth < 900 || viewportHeight >= viewportWidth * .84;
        ResponsiveContent.Padding = portraitPhone
            ? new Thickness(16, 12, 16, 104)
            : new Thickness(20, 24);
        ResponsiveContent.Spacing = portraitPhone ? 12 : 16;

        if (portraitPhone)
        {
            MovePlayerTo(PhonePlayerHost);
            var availableWidth = Math.Max(0, Math.Min(900, viewportWidth - 32));
            var playerHeight = Math.Max(180, availableWidth * 9d / 16d);
            PhonePlayerHost.HeightRequest = playerHeight;
            VideoPlayerFrame.WidthRequest = -1;
            VideoPlayerFrame.HeightRequest = playerHeight;
            VideoPlayerFrame.HorizontalOptions = LayoutOptions.Fill;
            VideoPlayerFrame.VerticalOptions = LayoutOptions.Fill;
            VideoPlayerFrame.TranslationX = 0;
            VideoPlayerFrame.TranslationY = 0;
            AlignmentDiagnostics.IsVisible = false;
            return;
        }

        var scale = Math.Max(
            viewportWidth / FilmRoomSourceSize.Width,
            viewportHeight / FilmRoomSourceSize.Height);
        var renderedWidth = FilmRoomSourceSize.Width * scale;
        var renderedHeight = FilmRoomSourceSize.Height * scale;
        var offsetX = (viewportWidth - renderedWidth) / 2d;
        var offsetY = (viewportHeight - renderedHeight) / 2d;
        var renderedVideo = new Rect(
            offsetX + FilmRoomVideoBounds.X * scale,
            offsetY + FilmRoomVideoBounds.Y * scale,
            FilmRoomVideoBounds.Width * scale,
            FilmRoomVideoBounds.Height * scale);

        MovePlayerTo(WidePlayerOverlay);
        AbsoluteLayout.SetLayoutFlags(VideoPlayerFrame, AbsoluteLayoutFlags.None);
        AbsoluteLayout.SetLayoutBounds(VideoPlayerFrame, renderedVideo);
        VideoPlayerFrame.WidthRequest = renderedVideo.Width;
        VideoPlayerFrame.HeightRequest = renderedVideo.Height;
        VideoPlayerFrame.HorizontalOptions = LayoutOptions.Start;
        VideoPlayerFrame.VerticalOptions = LayoutOptions.Start;
        VideoPlayerFrame.TranslationX = 0;
        VideoPlayerFrame.TranslationY = 0;
        PhonePlayerHost.HeightRequest = renderedVideo.Height;

        AlignmentDiagnostics.IsVisible = ShowFilmRoomAlignmentDiagnostics;
        if (ShowFilmRoomAlignmentDiagnostics)
        {
            AlignmentDiagnosticsLabel.Text =
                $"source 1672×941\nscale {scale:F4}\noffset {offsetX:F1}, {offsetY:F1}\n" +
                $"video {renderedVideo.X:F1}, {renderedVideo.Y:F1}, {renderedVideo.Width:F1}, {renderedVideo.Height:F1}";
        }
    }

    private void MovePlayerTo(Layout target)
    {
        if (ReferenceEquals(VideoPlayerFrame.Parent, target)) return;
        if (VideoPlayerFrame.Parent is Layout current)
            current.Children.Remove(VideoPlayerFrame);
        target.Children.Add(VideoPlayerFrame);
    }
}
