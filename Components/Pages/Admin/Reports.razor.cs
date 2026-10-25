using Application.DTOs;
using Application.DTOS;
using Application.Services;
using Microsoft.AspNetCore.Components;

namespace Snackis.Components.Pages.Admin;

public partial class Reports
{
    [Inject] private ReportService ReportService { get; set; } = default!;

    private List<ReportDto> reports = new();
    private bool showHandled;

    protected override async Task OnInitializedAsync() => await LoadAsync();

    private async Task LoadAsync() =>
        reports = await ReportService.GetReportsAsync(onlyUnhandled: !showHandled);

    private async Task Ignore(int reportId)
    {
        await ReportService.MarkHandledAsync(reportId);
        await LoadAsync();
    }

    private async Task RemovePost(int postId)
    {
        await ReportService.RemovePostAsync(postId);
        await LoadAsync();
    }
}