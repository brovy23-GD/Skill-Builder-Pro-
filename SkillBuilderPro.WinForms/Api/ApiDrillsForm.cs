using SkillBuilderPro.Core.Interfaces;
using CoreDrill = SkillBuilderPro.Core.Models.Drill;

namespace SkillBuilderPro.WinForms;

/// <summary>
/// Displays drills fetched live from the Web API — the vertical slice through every tier:
/// this form -> DrillApiService (HttpClient) -> DrillsController -> DrillService (EF Core) -> SQL Server.
/// Completely isolated from the local Guid-based DrillService and DrillDatabase, which still
/// power the rest of the app. Nothing here touches them.
/// </summary>
public partial class ApiDrillsForm : Form
{
    // Typed as the INTERFACE, not the concrete class. This form has no idea it's talking over
    // HTTP — it only knows something can hand it drills. Swap in a different IDrillService
    // implementation and this form doesn't change by one character. That's dependency inversion,
    // and it's the single most useful thing in this file to be able to explain out loud.
    private readonly IDrillService _drillService;

    public ApiDrillsForm()
    {
        InitializeComponent();

        // Constructor injection would be better here (the caller decides the implementation),
        // but this app creates forms with 'new' rather than resolving from a DI container.
        // Constructing it here is the pragmatic fit. The field is still typed as the interface,
        // so this one line is the ONLY place in the form that knows DrillApiService exists.
        _drillService = new DrillApiService();
    }

    private void ApiDrillsForm_Load(object? sender, EventArgs e)
    {
        // "All Sports" is a sentinel — it maps to null (no filter), not to a real sport value.
        cboSport.Items.AddRange(new object[] { "All Sports", "Basketball", "Softball", "Baseball", "Football", "Soccer", "Hockey" });
        cboSport.SelectedIndex = 0;
    }

    // async void — normally forbidden (you can't await it, and exceptions escape to the
    // UI thread instead of the caller). Event handlers are the ONE legitimate exception:
    // the Click signature returns void, so there's nothing to return a Task to. The rule is
    // that async void requires a try/catch wrapping the whole body — which is exactly what's
    // below. Everything it calls returns Task and is awaited properly.
    private async void btnLoad_Click(object? sender, EventArgs e)
    {
        // Disable during the call. Without this a user double-clicks and fires two overlapping
        // requests; the slower one wins and the grid shows stale data.
        btnLoad.Enabled = false;
        lblStatus.ForeColor = Color.Silver;
        lblStatus.Text = "Loading from API...";

        try
        {
            // Sentinel -> null. The service turns null into "omit the filter entirely."
            string? sport = cboSport.SelectedIndex == 0 ? null : cboSport.SelectedItem?.ToString();

            // THE await THAT MATTERS. Control returns to the message loop while the HTTP
            // round-trip happens, so the window stays responsive — drag it, it moves.
            // Call .Result instead and the UI thread blocks waiting on a task that needs the
            // UI thread to resume: instant deadlock. Same trap from Assignment 11.3.
            List<CoreDrill> drills = await _drillService.GetAllAsync(sport);

            // Rebind from scratch each time — clearing the source first prevents the grid from
            // keeping old columns when the shape changes.
            dgDrills.DataSource = null;
            dgDrills.DataSource = drills;

            // Hide the EF navigation collections. They're on the model for the API's benefit
            // (Include() joins) and render as useless "(Collection)" cells in a grid.
            if (dgDrills.Columns["Schedules"] != null) dgDrills.Columns["Schedules"]!.Visible = false;
            if (dgDrills.Columns["ProgressLogs"] != null) dgDrills.Columns["ProgressLogs"]!.Visible = false;

            if (dgDrills.Columns["Description"] != null)
                dgDrills.Columns["Description"]!.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            lblStatus.ForeColor = Color.LightGreen;
            lblStatus.Text = $"{drills.Count} drill(s) loaded from SQL Server via Web API.";
        }
        catch (HttpRequestException ex)
        {
            // The API isn't running, or the port is wrong. FAIL LOUDLY — no silent fallback to
            // local data. A client that quietly stops using the API when it's down is how you
            // ship a broken integration and don't find out for a month.
            lblStatus.ForeColor = Color.Orange;
            lblStatus.Text = "Cannot reach API.";
            MessageBox.Show(
                $"Could not reach the Web API at http://localhost:62978.\n\n" +
                $"Start SkillBuilderPro.API first, then try again.\n\nDetail: {ex.Message}",
                "API Unavailable", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (TaskCanceledException)
        {
            // HttpClient.Timeout fired — the 10 seconds set in DrillApiService.
            lblStatus.ForeColor = Color.Orange;
            lblStatus.Text = "Request timed out.";
            MessageBox.Show("The API took too long to respond.", "Timeout",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        finally
        {
            // finally ALWAYS runs — success, exception, or early return. Without it, one failed
            // request leaves the button dead forever and the user has to restart the app.
            btnLoad.Enabled = true;
        }
    }

    // ---------------------------------------------------------
    // WATCH VIDEO BUTTON HANDLER (added exactly as requested)
    // ---------------------------------------------------------
    private void BtnWatchVideo_Click(object? sender, EventArgs e)
    {
        if (dgDrills.SelectedRows.Count == 0)
        {
            MessageBox.Show("Please select a drill first.", "No Selection",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var selectedRow = dgDrills.SelectedRows[0];
        string? videoUrl = selectedRow.Cells["VideoUrl"]?.Value?.ToString();

        if (string.IsNullOrWhiteSpace(videoUrl))
        {
            MessageBox.Show("This drill has no video link.", "No Video",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = videoUrl,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to open video: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

}