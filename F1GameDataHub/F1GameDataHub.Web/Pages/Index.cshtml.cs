using F1GameDataHub.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace F1GameDataHub.Web.Pages;

public class IndexModel(
    SessionSummaryService sessionSummaryService,
    DumpImportJobService importJobService,
    LiveListenerJobService listenerJobService,
    JobStatusStore jobStatusStore) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public long? SelectedSessionUid { get; set; }

    [BindProperty]
    public string DumpPath { get; set; } = string.Empty;

    [BindProperty]
    public bool ResetBeforeImport { get; set; }

    public IReadOnlyList<SessionSummaryItem> Sessions { get; private set; } = [];
    public IReadOnlyList<SelectListItem> SessionOptions { get; private set; } = [];
    public SessionSummaryItem? SelectedSession { get; private set; }
    public JobStatusSnapshot ImportStatus { get; private set; } = new(JobRunState.Idle, "Not started.", DateTimeOffset.UtcNow);
    public JobStatusSnapshot ListenerStatus { get; private set; } = new(JobRunState.Idle, "Not started.", DateTimeOffset.UtcNow);

    [TempData]
    public string? ActionMessage { get; set; }

    public async Task OnGetAsync()
    {
        await LoadAsync();
    }

    public async Task<IActionResult> OnPostSelectSessionAsync()
    {
        await LoadAsync();
        return Page();
    }

    public IActionResult OnPostStartImport()
    {
        if (string.IsNullOrWhiteSpace(DumpPath))
        {
            ActionMessage = "Please provide a dump path.";
            return RedirectToPage(new { SelectedSessionUid });
        }

        if (!importJobService.TryStart(DumpPath.Trim(), ResetBeforeImport, out var message))
        {
            ActionMessage = message;
            return RedirectToPage(new { SelectedSessionUid });
        }

        ActionMessage = message;
        return RedirectToPage(new { SelectedSessionUid });
    }

    public IActionResult OnPostStartListener()
    {
        if (!listenerJobService.TryStart(out var message))
        {
            ActionMessage = message;
            return RedirectToPage(new { SelectedSessionUid });
        }

        ActionMessage = message;
        return RedirectToPage(new { SelectedSessionUid });
    }

    public IActionResult OnPostStopListener()
    {
        if (!listenerJobService.TryStop(out var message))
        {
            ActionMessage = message;
            return RedirectToPage(new { SelectedSessionUid });
        }

        ActionMessage = message;
        return RedirectToPage(new { SelectedSessionUid });
    }

    private async Task LoadAsync()
    {
        try
        {
            Sessions = await sessionSummaryService.GetSessionsAsync(200, HttpContext.RequestAborted);
        }
        catch (Exception ex)
        {
            Sessions = [];
            ActionMessage ??= $"Unable to load sessions: {ex.Message}";
        }

        if (SelectedSessionUid is null && Sessions.Count > 0)
        {
            SelectedSessionUid = Sessions[0].SessionUid;
        }

        if (SelectedSessionUid is not null)
        {
            try
            {
                SelectedSession = Sessions.FirstOrDefault(s => s.SessionUid == SelectedSessionUid.Value)
                    ?? await sessionSummaryService.GetSessionAsync(SelectedSessionUid.Value, HttpContext.RequestAborted);
            }
            catch (Exception ex)
            {
                SelectedSession = null;
                ActionMessage ??= $"Unable to load selected session: {ex.Message}";
            }
        }

        SessionOptions = Sessions
            .Select(s => new SelectListItem(
                $"{s.SessionUid} | {s.TrackLabel} | {s.SessionType} | {s.SessionStartTimeUtc.ToLocalTime():yyyy-MM-dd HH:mm}",
                s.SessionUid.ToString()))
            .ToList();

        ImportStatus = jobStatusStore.ImportStatus;
        ListenerStatus = jobStatusStore.ListenerStatus;
    }
}
