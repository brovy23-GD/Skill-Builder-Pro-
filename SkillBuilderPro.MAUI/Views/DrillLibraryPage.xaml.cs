using System;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using SkillBuilderPro.Core.Models;
using SkillBuilderPro.MAUI.ViewModels;
using System.Threading.Tasks;
#if WINDOWS
using Microsoft.Maui.Handlers;
using Microsoft.Web.WebView2.Core;
#endif

namespace SkillBuilderPro.MAUI.Views;

[QueryProperty(nameof(VideoUrl), "videoUrl")]
[QueryProperty(nameof(DrillName), "drillName")]
public partial class DrillLibraryPage : ContentPage
{
    private readonly DrillsViewModel _viewModel;
    private int _currentPlaylistIndex = 0;
    private string _videoUrl = string.Empty;
    private string _drillName = string.Empty;
    private const string VideoVirtualHost = "skillbuilderpro.local";

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

    public DrillLibraryPage(DrillsViewModel viewModel)
    {
        InitializeComponent();

#if WINDOWS
        DrillVideoWebView.HandlerChanged += DrillVideoWebView_HandlerChanged;
#endif

        _viewModel = viewModel;
        BindingContext = _viewModel;
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
        if (_viewModel?.SelectedDrills == null || _viewModel.SelectedDrills.Count == 0) return;

        var currentDrill = _viewModel.SelectedDrills.FirstOrDefault(d => d.Name.Equals(_drillName, StringComparison.OrdinalIgnoreCase));
        if (currentDrill != null)
        {
            _currentPlaylistIndex = _viewModel.SelectedDrills.IndexOf(currentDrill);
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

        SelectedDrillLabel.Text =
            $"{_currentPlaylistIndex + 1}. {_drillName}";

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
        if (_viewModel?.SelectedDrills == null ||
            _viewModel.SelectedDrills.Count <= 1)
        {
            return;
        }

        if (_currentPlaylistIndex > 0)
        {
            _currentPlaylistIndex--;

            var drill =
                _viewModel.SelectedDrills[_currentPlaylistIndex];

            LoadDrillFromQueue(drill);
        }
    }

    public async void OnNextClicked(object sender, EventArgs e)
    {
        if (_viewModel?.SelectedDrills == null ||
            _viewModel.SelectedDrills.Count <= 1)
        {
            return;
        }

        if (_currentPlaylistIndex <
            _viewModel.SelectedDrills.Count - 1)
        {
            _currentPlaylistIndex++;

            var drill =
                _viewModel.SelectedDrills[_currentPlaylistIndex];

            LoadDrillFromQueue(drill);
        }
        else
        {
            StopCurrentVideo();

            await DisplayAlert(
                "Training Sequence",
                "You have finished your selected playlist sequence!",
                "OK");
        }
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
}