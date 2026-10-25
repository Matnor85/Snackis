using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using System.Security.Claims;

namespace Snackis.Components.Pages;

// AI-assisterad flöde: hämtar flödet och hanterar kommentarer och borttagning.
// StateHasChanged behövs efter AddComment/DeletePost eftersom de anropas via Func
// från FeedPostCard, och då ritas inte sidan om automatiskt.
public partial class Home
{
    [Inject] private FeedService FeedService { get; set; } = null!;

    [CascadingParameter] private Task<AuthenticationState> AuthState { get; set; } = null!;

    private List<WallPostDto> posts = new();
    private string? userId;
    private bool isAdmin;

    private string newContent = "";
    private IBrowserFile? selectedFile;
    private Guid fileInputKey = Guid.NewGuid();
    private bool isPublishing;
    private string? error;

    protected override async Task OnInitializedAsync()
    {
        var user = (await AuthState).User;
        userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        isAdmin = user.IsInRole("Admin");
        await LoadAsync();
    }

    private async Task LoadAsync() => posts = await FeedService.GetFeedAsync();

    private void OnFileSelected(InputFileChangeEventArgs e)
    {
        selectedFile = e.File;
        error = null;
    }

    private async Task Publish()
    {
        if (userId is null) return;

        error = null;
        isPublishing = true;
        try
        {
            if (selectedFile is null)
            {
                await FeedService.CreatePostAsync(userId, newContent, null, null);
            }
            else
            {
                var maxSize = FeedService.GetMaxSize(selectedFile.Name);
                await using var stream = selectedFile.OpenReadStream(maxSize);
                await FeedService.CreatePostAsync(userId, newContent, stream, selectedFile.Name);
            }

            // Töm formuläret. Ny key gör att filväljaren nollställs.
            newContent = "";
            selectedFile = null;
            fileInputKey = Guid.NewGuid();
            await LoadAsync();
        }
        catch (IOException) { error = "Filen är för stor."; }
        catch (ArgumentException ex) { error = ex.Message; }
        finally { isPublishing = false; }
    }

    private async Task AddComment(int postId, string content)
    {
        if (userId is null) return;
        await FeedService.AddCommentAsync(postId, userId, content);
        await LoadAsync();
        StateHasChanged();
    }

    private async Task DeletePost(int postId)
    {
        if (userId is null) return;
        await FeedService.DeleteAsync(postId, userId, isAdmin);
        await LoadAsync();
        StateHasChanged();
    }
}
