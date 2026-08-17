namespace SkillBuilderPro.MAUI.Services;

/// <summary>Accepts only absolute YouTube video URLs containing a canonical 11-character video id.</summary>
public static class YouTubeUrl
{
    public static bool IsValid(string? value) => TryGetVideoId(value, out _);

    public static bool TryGetVideoId(string? value, out string videoId)
    {
        videoId = string.Empty;
        if (!Uri.TryCreate(value?.Trim(), UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps)
            return false;

        var host = uri.Host.TrimStart('.').ToLowerInvariant();
        if (host.StartsWith("www.")) host = host[4..];

        string? candidate = null;
        if (host == "youtu.be")
        {
            candidate = uri.AbsolutePath.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        }
        else if (host is "youtube.com" or "m.youtube.com" or "youtube-nocookie.com")
        {
            var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length >= 2 && segments[0] is "embed" or "shorts" or "live")
                candidate = segments[1];
            else if (uri.AbsolutePath.Equals("/watch", StringComparison.OrdinalIgnoreCase))
                candidate = ParseQuery(uri.Query, "v");
        }

        if (candidate?.Length != 11 || candidate.Any(c => !(char.IsLetterOrDigit(c) || c is '-' or '_')))
            return false;

        videoId = candidate;
        return true;
    }

    static string? ParseQuery(string query, string key)
    {
        foreach (var pair in query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var parts = pair.Split('=', 2);
            if (parts.Length == 2 && Uri.UnescapeDataString(parts[0]).Equals(key, StringComparison.OrdinalIgnoreCase))
                return Uri.UnescapeDataString(parts[1]);
        }
        return null;
    }
}
