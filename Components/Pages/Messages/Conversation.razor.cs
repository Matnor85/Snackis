using Application.DTOS;
using Application.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Snackis.Services;
using System.Security.Claims;

namespace Snackis.Components.Pages.Messages;

public partial class Conversation
{
    [Inject] private MessageService MessageService { get; set; } = default!;
    [Inject] private ProfileService ProfileService { get; set; } = default!;
    [Inject] private UnreadMessagesState UnreadState { get; set; } = default!;

    private bool loading = true;
    [Parameter] public string OtherUserId { get; set; } = "";
    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; } = null!;

    private string userId = "";
    private UserProfileDto? otherUser;
    private List<MessageDto> messages = new();
    private string newMessage = "";
    private string? error;

    protected override async Task OnParametersSetAsync()
    {
        loading = true;
        try
        {
            userId = (await AuthState).User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            otherUser = await ProfileService.GetAsync(OtherUserId);
            await LoadAsync();
        }
        finally
        {
            loading = false;
        }
    }

    private async Task LoadAsync()
    {
        if (otherUser is not null)
        {
            messages = await MessageService.GetConversationAsync(userId, OtherUserId);
            // AI-assisterad: säger till navbaren att räkna om olästa
            UnreadState.NotifyChanged();
        }
    }

    private async Task Send()
    {
        error = null;
        try
        {
            await MessageService.SendAsync(userId, OtherUserId, newMessage);
            newMessage = "";
            await LoadAsync();
        }
        catch (ArgumentException ex)
        {
            error = ex.Message;
        }
    }
}
