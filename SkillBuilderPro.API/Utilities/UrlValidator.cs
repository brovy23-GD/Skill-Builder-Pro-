using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace SkillBuilderPro.API.Utilities
{
    public static class UrlValidator
    {
        private static readonly HttpClient _http = new HttpClient()
        {
            Timeout = TimeSpan.FromSeconds(5)
        };

        public static async Task<bool> IsValidYouTubeUrlAsync(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            url = url.Trim();

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                return false;

            if (!IsYouTubeDomain(uri.Host))
                return false;

            if (!HasYouTubeVideoId(uri))
                return false;

            return await UrlReachableAsync(uri);
        }

        private static bool IsYouTubeDomain(string host)
        {
            host = host.ToLower();

            return host == "youtube.com"
                || host == "www.youtube.com"
                || host == "youtu.be"
                || host == "www.youtu.be"
                || host == "youtube-nocookie.com"
                || host == "www.youtube-nocookie.com";
        }

        private static bool HasYouTubeVideoId(Uri uri)
        {
            if (uri.Host.Contains("youtu.be"))
                return uri.AbsolutePath.Length > 1;

            var query = HttpUtility.ParseQueryString(uri.Query);
            var id = query["v"];

            return !string.IsNullOrWhiteSpace(id);
        }

        private static async Task<bool> UrlReachableAsync(Uri uri)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Head, uri);
                using var response = await _http.SendAsync(request);

                return response.IsSuccessStatusCode ||
                       response.StatusCode == System.Net.HttpStatusCode.Redirect;
            }
            catch
            {
                return false;
            }
        }
    }
}
