using System;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace SkillBuilderPro.MAUI.Views;

[QueryProperty(nameof(VideoUrl), "videoUrl")]
[QueryProperty(nameof(DrillName), "drillName")]
public partial class VideoPlayerPage : ContentPage
{
    private string _videoUrl = string.Empty;
    private string _drillName = string.Empty;

    public VideoPlayerPage()
    {
        InitializeComponent();
        UpdateDrillLabels();
    }

    public string VideoUrl
    {
        get => _videoUrl;
        set
        {
            _videoUrl = DecodeQueryValue(value);
            MainThread.BeginInvokeOnMainThread(LoadVideo);
        }
    }

    public string DrillName
    {
        get => _drillName;
        set
        {
            _drillName = DecodeQueryValue(value);
            MainThread.BeginInvokeOnMainThread(UpdateDrillLabels);
        }
    }

    private static string DecodeQueryValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        try
        {
            return Uri.UnescapeDataString(value);
        }
        catch (UriFormatException)
        {
            return value;
        }
    }

    private void UpdateDrillLabels()
    {
        if (DrillNameLabel is null || DrillsListLabel is null)
            return;

        if (string.IsNullOrWhiteSpace(_drillName))
        {
            DrillNameLabel.Text = "ACTIVE DRILL";
            DrillsListLabel.Text = "Selected drill: General Training";
            return;
        }

        DrillNameLabel.Text = _drillName.ToUpperInvariant();
        DrillsListLabel.Text = $"Selected drill: {_drillName}";
    }

    private void LoadVideo()
    {
        if (VideoWebView is null)
            return;

        if (string.IsNullOrWhiteSpace(_videoUrl))
        {
            VideoWebView.Source = new HtmlWebViewSource
            {
                BaseUrl = "https://www.youtube.com/",
                Html = """
                    <!DOCTYPE html>
                    <html>
                    <body style="
                        margin:0;
                        background:#000;
                        color:#fff;
                        display:flex;
                        align-items:center;
                        justify-content:center;
                        height:100vh;
                        font-family:Arial;">
                        <h2>No video target provided.</h2>
                    </body>
                    </html>
                    """
            };

            return;
        }

        string embedUrl = ConvertToEmbedUrl(_videoUrl);
        string safeEmbedUrl =
            System.Net.WebUtility.HtmlEncode(embedUrl);

        var html = $$"""
            <!DOCTYPE html>
            <html>
            <head>
                <base href="https://www.youtube.com/" />

                <meta
                    name="referrer"
                    content="origin" />

                <style>
                    html, body {
                        margin: 0;
                        padding: 0;
                        width: 100%;
                        height: 100%;
                        background: #000;
                        overflow: hidden;
                    }

                    iframe {
                        position: absolute;
                        inset: 0;
                        width: 100%;
                        height: 100%;
                        border: 0;
                    }
                </style>
            </head>

            <body>
                <iframe
                    src="{{safeEmbedUrl}}"
                    referrerpolicy="origin"
                    allow="accelerometer; autoplay; clipboard-write;
                           encrypted-media; gyroscope;
                           picture-in-picture; web-share"
                    allowfullscreen>
                </iframe>
            </body>
            </html>
            """;

        VideoWebView.Source = new HtmlWebViewSource
        {
            BaseUrl = "https://www.youtube.com/",
            Html = html
        };
    }

    private static string ConvertToEmbedUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return string.Empty;

        string videoId = ExtractYouTubeId(url);

        if (string.IsNullOrWhiteSpace(videoId))
            return url;

        return
            $"https://www.youtube.com/embed/{videoId}" +
            "?autoplay=1" +
            "&playsinline=1" +
            "&rel=0" +
            "&controls=1" +
            "&modestbranding=1" +
            "&origin=https%3A%2F%2Fwww.youtube.com";
    }

    private static string ExtractYouTubeId(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return string.Empty;

        if (!Uri.TryCreate(
                url,
                UriKind.Absolute,
                out Uri? uri))
        {
            return string.Empty;
        }

        if (uri.Host.Contains(
                "youtu.be",
                StringComparison.OrdinalIgnoreCase))
        {
            return uri.AbsolutePath
                .Trim('/')
                .Split('/')[0];
        }

        if (!uri.Host.Contains(
                "youtube.com",
                StringComparison.OrdinalIgnoreCase))
        {
            return string.Empty;
        }

        string query = uri.Query.TrimStart('?');

        foreach (string part in query.Split(
            '&',
            StringSplitOptions.RemoveEmptyEntries))
        {
            string[] pair = part.Split(
                '=',
                2,
                StringSplitOptions.None);

            if (pair.Length == 2 &&
                string.Equals(
                    pair[0],
                    "v",
                    StringComparison.OrdinalIgnoreCase))
            {
                return Uri.UnescapeDataString(pair[1]);
            }
        }

        string[] segments = uri.AbsolutePath.Split(
            '/',
            StringSplitOptions.RemoveEmptyEntries);

        for (int index = 0; index < segments.Length - 1; index++)
        {
            if (string.Equals(
                    segments[index],
                    "embed",
                    StringComparison.OrdinalIgnoreCase) ||
                string.Equals(
                    segments[index],
                    "shorts",
                    StringComparison.OrdinalIgnoreCase))
            {
                return segments[index + 1];
            }
        }

        return string.Empty;
    }

    private async void OnPrevClicked(
        object sender,
        EventArgs e)
    {
        await DisplayAlert(
            "Playlist",
            "Previous video functionality is not enabled yet.",
            "OK");
    }

    private async void OnNextClicked(
        object sender,
        EventArgs e)
    {
        await DisplayAlert(
            "Playlist",
            "Next video functionality is not enabled yet.",
            "OK");
    }

    private async void OnOpenExternallyClicked(
        object sender,
        EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_videoUrl))
        {
            await DisplayAlert(
                "Video Unavailable",
                "There is no video URL to open.",
                "OK");

            return;
        }

        if (!Uri.TryCreate(
                _videoUrl,
                UriKind.Absolute,
                out Uri? videoUri))
        {
            await DisplayAlert(
                "Invalid Video URL",
                "The video URL is not valid.",
                "OK");

            return;
        }

        await Launcher.Default.OpenAsync(videoUri);
    }

    private async void OnBackClicked(
        object sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private void OnResponsiveSizeChanged(object? sender, EventArgs e)
    {
        if (VideoStage.Width <= 0) return;
        var phone = VideoStage.Width < 700;
        VideoHeader.HeightRequest = phone ? 60 : 76;
        VideoHeader.Padding = phone ? new Thickness(16, 0) : new Thickness(24, 0);
        VideoContentGrid.Padding = phone
            ? new Thickness(16, 12, 16, 104)
            : new Thickness(24, 24, 24, 40);
        var availableWidth = Math.Max(0, Math.Min(1040, VideoStage.Width - (phone ? 32 : 48)));
        VideoFrame.HeightRequest = phone
            ? Math.Clamp(availableWidth * 9d / 16d, 190, 300)
            : 540;
    }

    private async void OnExitClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//Home");
    }
}
