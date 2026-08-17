// Location: SkillBuilderPro.WinForms/Services/DrillApiClient.cs
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using SkillBuilderPro.Core.Models; // ?? Directly maps to the global shared Drill domain model

namespace SkillBuilderPro.WinForms.Services;

/// <summary>
/// Modernized Data Client providing the WinForms desktop presentation layer
/// with type-safe access to backend RESTful API endpoints.
/// </summary>
public class DrillApiClient
{
    private readonly HttpClient _http;

    /// <summary>
    /// Unified Dependency Injection Constructor.
    /// Automatically ingests the HttpClient managed by your app startup service container framework.
    /// </summary>
    public DrillApiClient(HttpClient http)
    {
        _http = http ?? throw new ArgumentNullException(nameof(http));
    }

    /// <summary>
    /// Asynchronously streams and deserializes athletic drill records from the live backend server.
    /// </summary>
    public async Task<List<Drill>> GetAllDrillsAsync()
    {
        try
        {
            // Leverages the base address routed from your central Program.cs container configuration
            var drills = await _http.GetFromJsonAsync<List<Drill>>("api/drills");
            return drills ?? new List<Drill>();
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
        {
            // WinForms still uses its legacy local user session, which does not
            // issue the JWT required by the protected API. An unauthorized API
            // drill request therefore falls through to DrillProvider's existing
            // local database instead of being reported as a network outage.
            return new List<Drill>();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"API Connection Failure: {ex.Message}", "Network Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return new List<Drill>();
        }
    }

    /// <summary>
    /// ?? FIXED: Fully implements the missing contract lookup endpoint.
    /// Diverts execution safely to clear out the runtime NotImplementedException crash completely.
    /// </summary>
    internal async Task<List<Drill>> GetAllAsync()
    {
        return await GetAllDrillsAsync();
    }
}
