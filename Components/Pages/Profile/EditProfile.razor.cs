using Application.DTOS;
using Application.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using System.Security.Claims;

namespace Snackis.Components.Pages.Profile;

public partial class EditProfile
{
    [Inject] private ProfileService ProfileService { get; set; } = default!;
    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; } = null!;

    private const long MaxFileSize = 2 * 1024 * 1024; // 2 MB

    private UserProfileDto? profile;
    private string userId = "";
    private string displayName = "";
    private string? bio;
    private string? message;
    private string? error;

    protected override async Task OnInitializedAsync()
    {
        userId = (await AuthState).User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        profile = await ProfileService.GetAsync(userId);
        if (profile is not null)
        {
            displayName = profile.DisplayName;
            bio = profile.Bio;
        }
    }

    private async Task Save()
    {
        message = error = null;
        try
        {
            await ProfileService.UpdateAsync(userId, displayName, bio);
            await LoadAsync();
            message = "Profilen är sparad!";
        }
        catch (ArgumentException ex) { error = ex.Message; }
    }

    private async Task UploadImage(InputFileChangeEventArgs e)
    {
        message = error = null;
        try
        {
            await using var stream = e.File.OpenReadStream(MaxFileSize);
            await ProfileService.UploadImageAsync(userId, stream, e.File.Name);
            await LoadAsync();
            message = "Ny profilbild sparad!";
        }
        catch (IOException) { error = "Bilden är för stor (max 2 MB)."; }
        catch (ArgumentException ex) { error = ex.Message; }
    }
}
