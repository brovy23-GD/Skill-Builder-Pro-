using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Web;
using SkillBuilderPro.Core.Interfaces;

// ALIAS: WinForms already has its own Drill class (Guid key, Duration, SkillCategory) used by
// DrillDatabase and the local DrillService. Core has a DIFFERENT Drill (int key, Sport, VideoUrl).
// Both are now visible in this project, so writing bare "Drill" is ambiguous — CS0104.
// This alias says: inside this file, CoreDrill means the API's version. The local Drill is untouched.
using CoreDrill = SkillBuilderPro.Core.Models.Drill;

// Namespace stays FLAT — SkillBuilderPro.WinForms, not .Api — per the project standard.
// The Api/ folder is organization on disk; it deliberately does not become a namespace.
namespace SkillBuilderPro.WinForms;

/// <summary>
/// HTTP implementation of IDrillService. Talks to the SkillBuilderPro Web API.
/// The API project has its OWN IDrillService implementation (DrillService) that uses EF Core
/// and SQL Server. Same interface, opposite mechanism: that one reads the database directly,
/// this one asks the API to do it. This class is the client end of the wire.
/// </summary>
public class DrillApiService : IDrillService
{
    // ONE HttpClient for the whole app, created once and never disposed.
    // Why static: every 'new HttpClient()' opens its own connection pool, and a disposed client
    // holds its sockets in TIME_WAIT for ~4 minutes. Create one per button click and you exhaust
    // the machine's ports under load. This is the classic HttpClient interview question.
    // Ideally this would be injected by a DI container (AddHttpClient), but this app constructs
    // forms with 'new' rather than resolving them from a container — so a static shared instance
    // is the honest, correct choice here. Noted in the roadmap as a future refactor.
    private static readonly HttpClient _http = new HttpClient
    {
        // http, NOT the https 62977 port — https needs a trusted dev cert, and we don't need TLS
        // to talk to our own machine. Trailing slash is REQUIRED: BaseAddress + "api/drills"
        // only concatenates correctly with it. Without it, .NET drops the last path segment.
        BaseAddress = new Uri("http://localhost:62978/"),

        // If the API isn't running, fail in 10 seconds instead of hanging the user for 100.
        Timeout = TimeSpan.FromSeconds(10)
    };

    public async Task<List<CoreDrill>> GetAllAsync(string? sport = null, string? category = null)
    {
        // ParseQueryString(string.Empty) returns an empty collection that handles two things
        // hand-rolled string concatenation gets wrong: punctuation (first param '?', rest '&')
        // and URL encoding ("Ball Handling" -> "Ball%20Handling"). Never build query strings by hand.
        var query = HttpUtility.ParseQueryString(string.Empty);

        // IsNullOrWhiteSpace catches three cases at once: null, "", and "   ".
        // An unselected ComboBox gives "" very easily, and sending "?sport=" is noise.
        // The API guards this too — defense in depth: the server can't trust any client,
        // and the client shouldn't send garbage in the first place.
        if (!string.IsNullOrWhiteSpace(sport))
        {
            query["sport"] = sport;
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query["category"] = category;
        }

        // Ternary: condition ? if-true : if-false. No filters means no trailing '?' —
        // "api/drills?" is malformed.
        string url = query.Count > 0 ? $"api/drills?{query}" : "api/drills";

        // GetFromJsonAsync does three jobs in one line: sends the GET, reads the response body,
        // deserializes JSON into List<CoreDrill>. It's case-insensitive by default, so the API's
        // camelCase ("difficultyLevel") binds to our PascalCase (DifficultyLevel) with no config.
        List<CoreDrill>? drills = await _http.GetFromJsonAsync<List<CoreDrill>>(url);

        // ?? is null-coalescing: "use the left unless it's null, then use the right."
        // The signature promised Task<List<CoreDrill>> with no '?' — returning null would break
        // that contract. foreach over an empty list does nothing; foreach over null throws.
        return drills ?? new List<CoreDrill>();
    }

    public async Task<CoreDrill?> GetByIdAsync(int id)
    {
        // Deliberately NOT GetFromJsonAsync here — it throws on any non-success status, including
        // 404. But for GetById a 404 isn't a failure, it's an answer: "no drill with that id."
        // The nullable return type (CoreDrill?) exists precisely to express that outcome.
        HttpResponseMessage response = await _http.GetAsync($"api/drills/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        // Any OTHER failure (500, connection refused) IS a real error. Throw and let it surface.
        // Swallowing a 500 as "not found" would hide a broken API for weeks.
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<CoreDrill>();
    }

    public async Task<CoreDrill> CreateAsync(CoreDrill drill)
    {
        HttpResponseMessage response = await _http.PostAsJsonAsync("api/drills", drill);
        response.EnsureSuccessStatusCode();

        // The API returns CreatedAtAction, whose body is the saved drill — now carrying the
        // database-assigned Id. We sent Id=0; we get back Id=7. ALWAYS use the returned object,
        // never the one you sent, or the UI holds a drill it can't later update or delete.
        CoreDrill? created = await response.Content.ReadFromJsonAsync<CoreDrill>();

        // Signature is Task<CoreDrill>, non-nullable — matching the API's own DrillService, which
        // can only succeed or throw (EF has no "created nothing" path). A 2xx with no body means
        // something is genuinely broken; fail loudly here rather than hand back a null that
        // explodes three call-frames away with no clue where it came from.
        return created ?? throw new InvalidOperationException("API returned success but no drill body.");
    }

    public async Task<bool> UpdateAsync(int id, CoreDrill drill)
    {
        HttpResponseMessage response = await _http.PutAsJsonAsync($"api/drills/{id}", drill);

        // Returns bool, not void, so the caller can react to "that drill is gone."
        // IsSuccessStatusCode is true for 200-299. The API returns 204 NoContent on success and
        // 404 if the id vanished. Both are valid outcomes — neither deserves an exception.
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        HttpResponseMessage response = await _http.DeleteAsync($"api/drills/{id}");
        return response.IsSuccessStatusCode;
    }
}