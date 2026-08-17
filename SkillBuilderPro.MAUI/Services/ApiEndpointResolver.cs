using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;

namespace SkillBuilderPro.MAUI.Services;

public static class ApiEndpointResolver
{
    public const string PreferenceKey = "SkillBuilderPro.ApiBaseUrl";
    private const string DevelopmentLanEndpoint = "http://192.168.1.126:5000/";

    public static Uri Resolve()
    {
        string? configured = Preferences.Default.Get<string?>(PreferenceKey, null);
        if (Uri.TryCreate(configured, UriKind.Absolute, out Uri? endpoint))
            return EnsureTrailingSlash(endpoint);

#if DEBUG
        if (DeviceInfo.Platform == DevicePlatform.Android && DeviceInfo.DeviceType == DeviceType.Virtual)
            return new Uri("http://10.0.2.2:5000/");

        if (DeviceInfo.Platform == DevicePlatform.WinUI)
            return new Uri("http://127.0.0.1:5000/");

        // Physical Android/iOS devices and a remote iOS simulator must reach
        // the development PC over its LAN address, not their own localhost.
        return new Uri(DevelopmentLanEndpoint);
#else
        return new Uri("https://api.skillbuilderpro.com/");
#endif
    }

    public static void Configure(string endpoint)
    {
        if (!Uri.TryCreate(endpoint, UriKind.Absolute, out Uri? uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            throw new ArgumentException("A valid HTTP or HTTPS API endpoint is required.", nameof(endpoint));

        Preferences.Default.Set(PreferenceKey, EnsureTrailingSlash(uri).AbsoluteUri);
    }

    private static Uri EnsureTrailingSlash(Uri uri) =>
        uri.AbsoluteUri.EndsWith("/", StringComparison.Ordinal) ? uri : new Uri(uri.AbsoluteUri + "/");
}
