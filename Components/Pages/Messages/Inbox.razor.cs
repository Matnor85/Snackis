using Application.DTOS;
using Application.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Snackis.Components.Pages.Messages;

public partial class Inbox
{
    [Inject] private MessageService MessageService { get; set; } = default!;
    [Inject] private ProfileService ProfileService { get; set; } = default!;

    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; } = null!;

    private string userId = "";
    private List<ConversationDto> conversations = new();
    private List<UserProfileDto> searchResults = new();
    private string searchTerm = "";

    protected override async Task OnInitializedAsync()
    {
        userId = (await AuthState).User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        conversations = await MessageService.GetConversationsAsync(userId);
    }

    private async Task Search() =>
        searchResults = await ProfileService.SearchAsync(searchTerm, userId);
}
