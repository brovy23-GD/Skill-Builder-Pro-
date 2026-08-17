using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Diagnostics;
using SkillBuilderPro.MAUI.Models;

namespace SkillBuilderPro.MAUI.Services;

public interface IAthleteApiService
{
    CurrentUser? User { get; }
    bool IsAuthenticated { get; }
    bool IsDemoMode { get; }
    bool IsServiceAvailable { get; }
    string? ServiceStatusMessage { get; }
    string? SelectedRole { get; }
    Task<bool> RestoreAsync();
    Task<(bool Ok, string? Error)> LoginAsync(string email, string password, string selectedRole);
    Task<(bool Ok, string? Error)> RegisterAsync(string email, string password, string fullName, string role);
    void SelectRole(string role);
    void EnterDemoMode();
    Task LogoutAsync();
    Task<T?> GetAsync<T>(string route);
    Task<T?> PostAsync<T>(string route, object? body = null);
}

public sealed class AthleteApiService(HttpClient http) : IAthleteApiService
{
    private const string UnavailableMessage = "Skill Builder Pro services are currently unavailable. Start the API and try again.";
    private const string TokenKey = "sbp_access_token";
    private const string ExpiryKey = "sbp_token_expiry";
    public CurrentUser? User { get; private set; }
    public bool IsAuthenticated => User is not null && !IsDemoMode;
    public bool IsDemoMode { get; private set; }
    public bool IsServiceAvailable { get; private set; } = true;
    public string? ServiceStatusMessage { get; private set; }
    public string? SelectedRole { get; private set; }

    public async Task<bool> RestoreAsync()
    {
        var token = await SecureStorage.Default.GetAsync(TokenKey);
        var expiry = await SecureStorage.Default.GetAsync(ExpiryKey);
        if (string.IsNullOrWhiteSpace(token) || !DateTime.TryParse(expiry, out var at) || at <= DateTime.UtcNow) return false;
        SetToken(token);
        User = await GetAsync<CurrentUser>("api/auth/me");
        if (User is null) await LogoutAsync();
        return User is not null;
    }

    public async Task<(bool, string?)> LoginAsync(string email, string password, string selectedRole)
    {
        try
        {
#if DEBUG
            Debug.WriteLine($"[AUTH] Endpoint={http.BaseAddress}api/auth/login Role={selectedRole}");
#endif
            var response = await http.PostAsJsonAsync("api/auth/login", new LoginRequest(email, password));
            IsServiceAvailable = true; ServiceStatusMessage = null;
            if (!response.IsSuccessStatusCode)
            {
#if DEBUG
                Debug.WriteLine($"[AUTH] HTTP={(int)response.StatusCode} Response={SanitizedLoginStatus(response.StatusCode)} Role={selectedRole} TokenStored=no");
#endif
                return (false, response.StatusCode == HttpStatusCode.Unauthorized ? "Sign-in failed. Check your credentials and role." : $"Sign-in service returned HTTP {(int)response.StatusCode}.");
            }
            var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
            if (auth is null) return (false, "The service returned an invalid sign-in response.");
            if (!auth.User.Roles.Any(x => string.Equals(x, selectedRole, StringComparison.OrdinalIgnoreCase))) return (false, $"This account is not registered as {selectedRole}.");
            await SecureStorage.Default.SetAsync(TokenKey, auth.AccessToken);
            await SecureStorage.Default.SetAsync(ExpiryKey, auth.ExpiresAtUtc.ToString("O"));
            SetToken(auth.AccessToken);
            User = auth.User;
            SelectedRole = selectedRole;
            IsDemoMode = false;
#if DEBUG
            Debug.WriteLine($"[AUTH] HTTP={(int)response.StatusCode} Response=authenticated Role={selectedRole} TokenStored=yes Destination={selectedRole} experience");
#endif
            return (true, null);
        }
        catch (HttpRequestException) { MarkUnavailable(); return (false, UnavailableMessage); }
        catch (TaskCanceledException) { MarkUnavailable(); return (false, UnavailableMessage); }
    }

    public async Task<(bool, string?)> RegisterAsync(string email, string password, string fullName, string role)
    {
        try
        {
            var response = await http.PostAsJsonAsync("api/auth/register", new RegisterRequest(email, password, fullName, role));
            IsServiceAvailable = true; ServiceStatusMessage = null;
            if (!response.IsSuccessStatusCode) return (false, "Profile creation failed. Check the information and try again.");
            var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
            if (auth is null) return (false, "The service returned an invalid registration response.");
            await SecureStorage.Default.SetAsync(TokenKey, auth.AccessToken);
            await SecureStorage.Default.SetAsync(ExpiryKey, auth.ExpiresAtUtc.ToString("O"));
            SetToken(auth.AccessToken);
            User = auth.User;
            SelectedRole = role;
            IsDemoMode = false;
            return (true, null);
        }
        catch (HttpRequestException) { MarkUnavailable(); return (false, UnavailableMessage); }
        catch (TaskCanceledException) { MarkUnavailable(); return (false, UnavailableMessage); }
    }

    public void SelectRole(string role) => SelectedRole = role;
    public void EnterDemoMode() { User = null; IsDemoMode = true; SelectedRole = "Athlete"; http.DefaultRequestHeaders.Authorization = null; SecureStorage.Default.Remove(TokenKey); SecureStorage.Default.Remove(ExpiryKey); }
    public Task LogoutAsync() { User = null; IsDemoMode = false; SelectedRole = null; http.DefaultRequestHeaders.Authorization = null; SecureStorage.Default.Remove(TokenKey); SecureStorage.Default.Remove(ExpiryKey); return Task.CompletedTask; }

    public async Task<T?> GetAsync<T>(string route)
    {
        try
        {
            if (IsDemoMode && string.Equals(route, "api/drills", StringComparison.OrdinalIgnoreCase)) route = "api/drills/demo";
            var response = await http.GetAsync(route);
            IsServiceAvailable = true; ServiceStatusMessage = null;
            if (response.StatusCode == HttpStatusCode.Unauthorized) { await LogoutAsync(); return default; }
            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<T>() : default;
        }
        catch (HttpRequestException) { MarkUnavailable(); return default; }
        catch (TaskCanceledException) { MarkUnavailable(); return default; }
    }

    public async Task<T?> PostAsync<T>(string route, object? body = null)
    {
        try
        {
            var response = await http.PostAsJsonAsync(route, body ?? new { });
            IsServiceAvailable = true; ServiceStatusMessage = null;
            if (response.StatusCode == HttpStatusCode.Unauthorized) { await LogoutAsync(); return default; }
            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<T>() : default;
        }
        catch (HttpRequestException) { MarkUnavailable(); return default; }
        catch (TaskCanceledException) { MarkUnavailable(); return default; }
    }

    private void SetToken(string token) => http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    private void MarkUnavailable() { IsServiceAvailable = false; ServiceStatusMessage = UnavailableMessage; }
    private static string SanitizedLoginStatus(HttpStatusCode status) => status switch
    {
        HttpStatusCode.Unauthorized => "credentials-or-account-rejected",
        HttpStatusCode.BadRequest => "request-validation-failed",
        HttpStatusCode.Forbidden => "account-forbidden",
        _ => "service-error"
    };
}
