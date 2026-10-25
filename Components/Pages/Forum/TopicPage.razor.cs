using Application.DTOS;
using Domain.Entities;
using Application.Services;
using Microsoft.AspNetCore.Components;

namespace Snackis.Components.Pages.Forum;

public partial class TopicPage
{
    [Inject] private DiscussionService DiscussionService { get; set; } = default!;
    [Parameter] public int TopicId { get; set; }

    private Topic? topic;
    private List<DiscussionSummaryDto> discussions = new();

    protected override async Task OnParametersSetAsync()
    {
        topic = await DiscussionService.GetTopicAsync(TopicId);
        discussions = await DiscussionService.GetByTopicAsync(TopicId);
    }
}
