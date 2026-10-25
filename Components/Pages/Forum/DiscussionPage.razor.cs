using Application.DTOS;
using Application.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Snackis.Components.Pages.Forum;

public partial class DiscussionPage
{
    [Inject] private PostService PostService { get; set; } = default!;
    [Inject] private ReportService ReportService { get; set; } = default!;

    [Parameter]
    public int DiscussionId { get; set; }

    [CascadingParameter]
    private Task<AuthenticationState> AuthState { get; set; } = null!;

    private DiscussionDetailsDto? discussion;
    private string newPost = "";
    private string? error;

    protected override async Task OnParametersSetAsync() => await LoadAsync();

    private async Task LoadAsync() =>
        discussion = await PostService.GetDiscussionAsync(DiscussionId);

    private Task HandleReply((int ParentPostId, string Content) reply) =>
        AddPost(reply.ParentPostId, reply.Content);

    private async Task AddPost(int? parentPostId, string content)
    {
        error = null;
        var userId = (await AuthState).User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return;

        try
        {
            await PostService.AddPostAsync(DiscussionId, content, userId, parentPostId);
            if (parentPostId is null) newPost = "";
            await LoadAsync();
        }
        catch (ArgumentException ex)
        {
            error = ex.Message;
        }
    }

    // AI-assisterad:
    // Anropas från PostItem när någon klickar Svara på ett enskilt inlägg.
    // Skickar vidare till AddPost med inläggets id som förälder, så att svaret
    // hamnar som en sub-diskussion under det inlägget.
    private async Task<string> HandleReport(int postId, string reason)
    {
        var userId = (await AuthState).User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return "Du måste vara inloggad.";

        try
        {
            await ReportService.ReportAsync(postId, userId, reason);
            return "Tack! En admin kommer att granska inlägget.";
        }
        catch (ArgumentException ex)
        {
            return ex.Message;
        }
    }
    // AI-assisterad: admin suddar ut ett inlägg. Rollen kontrolleras även på servern.
    private async Task HandleDelete(int postId)
    {
        // Dubbelkolla på servern att det verkligen är en admin
        var user = (await AuthState).User;
        if (!user.IsInRole("Admin")) return;

        await ReportService.RemovePostAsync(postId);
        await LoadAsync();
        StateHasChanged();
    }
    // AI-assisterad: admin raderar ett utsuddat inlägg och alla svar under det
    private async Task HandleDeletePermanently(int postId)
    {
        var user = (await AuthState).User;
        if (!user.IsInRole("Admin")) return;

        await PostService.DeletePermanentlyAsync(postId);
        await LoadAsync();
        StateHasChanged();
    }
}