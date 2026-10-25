using Application.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Snackis.Services;
using System.Security.Claims;

namespace Snackis.Components.Layout;
// AI-assisterad olästa-räknare:
// Räknar om antalet olästa när man byter sida (LocationChanged) och när en
// konversation har lästs (UnreadState.Changed). Händelserna kopplas bort i Dispose.
public partial class NavMenu
{
    [Inject] NavigationManager NavigationManager { get; set; } = default!;
    [Inject] MessageService MessageService { get; set; } = default!;
    [Inject] UnreadMessagesState UnreadState { get; set; } = default!;
    [CascadingParameter] private Task<AuthenticationState>? AuthState { get; set; }

    private string? currentUrl;
    private int unreadCount;

    protected override async Task OnInitializedAsync()
    {
        currentUrl = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
        NavigationManager.LocationChanged += OnLocationChanged;
        UnreadState.Changed += OnUnreadChanged;

        await UpdateUnreadAsync();
    }

    private async Task UpdateUnreadAsync()
    {
        if (AuthState is null) return;

        var userId = (await AuthState).User.FindFirstValue(ClaimTypes.NameIdentifier);
        unreadCount = userId is null ? 0 : await MessageService.CountUnreadAsync(userId);
    }

    // Räkna om varje gång man byter sida
    private async void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        currentUrl = NavigationManager.ToBaseRelativePath(e.Location);
        await RefreshAsync();
    }

    // Räkna om när en sida säger till (t.ex. när en konversation har lästs)
    private async void OnUnreadChanged() => await RefreshAsync();

    private async Task RefreshAsync()
    {
        try
        {
            await InvokeAsync(async () =>
            {
                await UpdateUnreadAsync();
                StateHasChanged();
            });
        }
        catch
        {
            // Navbaren får aldrig krascha sidan, då hoppar vi hellre över en uppdatering
        }
    }

    public void Dispose()
    {
        NavigationManager.LocationChanged -= OnLocationChanged;
        UnreadState.Changed -= OnUnreadChanged;
    }
}
