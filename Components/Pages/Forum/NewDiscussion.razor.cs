using Application.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Snackis.Components.Pages.Forum;

public partial class NewDiscussion
{
    [Inject] private DiscussionService DiscussionService { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    [Parameter] public int TopicId { get; set; }
    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; } = null!;

    private string title = "";
    private string content = "";
    private string? error;

    private async Task Create()
    {
        error = null;
        var user = (await AuthState).User;
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is null)
        {
            error = "Du måste vara inloggad.";
            return;
        }

        try
        {
            await DiscussionService.CreateAsync(TopicId, title, content, userId);
            Navigation.NavigateTo($"topic/{TopicId}");
        }
        catch (ArgumentException ex)
        {
            error = ex.Message;
        }
    }
}
