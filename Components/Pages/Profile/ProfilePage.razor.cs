using Application.DTOs;
using Application.DTOS;
using Application.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Snackis.Components.Pages.Profile;

// AI-assisterad (info-sida om sig själv + egen idé: vägg):
// Hämtar profilen och väggens inlägg för användaren i adressen (UserId).
// OnParametersSetAsync används så att sidan laddas om när man klickar
// från en profil direkt till en annan.
public partial class ProfilePage
{
    [Inject] ProfileService ProfileService { get; set; } = default!;
    [Inject] FeedService FeedService { get; set; } = default!;
    [Parameter] public string UserId { get; set; } = "";
    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; } = null!;

    private UserProfileDto? profile;
    private List<WallPostDto> wall = new();
    private string? currentUserId;
    private bool isAdmin;
    private bool loading = true;

    private bool IsOwnProfile => currentUserId == UserId;

    // OnParametersSet (inte OnInitialized) så att sidan laddas om när man klickar från en profil till en annan
    protected override async Task OnParametersSetAsync()
    {
        loading = true;
        try
        {
            var user = (await AuthState).User;
            currentUserId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            isAdmin = user.IsInRole("Admin");

            profile = await ProfileService.GetAsync(UserId);
            await LoadWallAsync();
        }
        finally
        {
            loading = false;
        }
    }

    // AI-assisterad vägg: hämtar en persons vägg till profilsidan
    private async Task LoadWallAsync() => wall = await FeedService.GetWallAsync(UserId);

    private async Task AddComment(int postId, string content)
    {
        if (currentUserId is null) return;
        await FeedService.AddCommentAsync(postId, currentUserId, content);
        await LoadWallAsync();
        StateHasChanged();
    }

    private async Task DeletePost(int postId)
    {
        if (currentUserId is null) return;
        await FeedService.DeleteAsync(postId, currentUserId, isAdmin);
        await LoadWallAsync();
        StateHasChanged();
    }
}
