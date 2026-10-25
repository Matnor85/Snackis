using Application.DTOS;
using Application.Services;
using Microsoft.AspNetCore.Components;

namespace Snackis.Components.Pages.Forum;

public partial class PostItem
{
    [Parameter, EditorRequired] public PostDto Post { get; set; } = null!;
    [Parameter] public int Depth { get; set; }
    [Parameter] public EventCallback<(int ParentPostId, string Content)> OnReplay { get; set; }
    [Parameter] public Func<int, Task>? OnDelete { get; set; }
    [Parameter] public Func<int, Task>? OnDeletePermanently { get; set; }

    // Returnerar ett meddelande som visas under inlägget
    [Parameter] public Func<int, string, Task<string>>? OnReport { get; set; }

    private bool IsRemoved => Post.Content == ReportService.RemovedText;

    private bool showReplay;
    private bool showReport;
    private string replayText = "";
    private string reportReason = "";
    private string? reportMessage;
    private bool confirmDelete;

    private void ToggleReplay()
    {
        showReplay = !showReplay;
        showReport = false;
    }

    private void ToggleReport()
    {
        showReport = !showReport;
        showReplay = false;
        reportMessage = null;
    }

    private async Task SendReplay()
    {
        if (string.IsNullOrWhiteSpace(replayText)) return;

        await OnReplay.InvokeAsync((Post.Id, replayText));
        replayText = "";
        showReplay = false;
    }

    private async Task SendReport()
    {
        if (OnReport is null) return;

        reportMessage = await OnReport(Post.Id, reportReason);
        reportReason = "";
        showReport = false;
    }

    private async Task Delete()
    {
        if (IsRemoved && OnDeletePermanently is not null)
            await OnDeletePermanently(Post.Id);
        else if (OnDelete is not null)
            await OnDelete(Post.Id);

        confirmDelete = false;
    }
}
