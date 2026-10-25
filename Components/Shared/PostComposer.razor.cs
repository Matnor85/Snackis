using Application.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Snackis.Components.Shared;

public partial class PostComposer
{
    [Inject] public FeedService FeedService { get; set; } = null!;

    [Parameter, EditorRequired] public string UserId { get; set; } = "";
    [Parameter] public string? ProfileOwnerId { get; set; }   // null = egen vägg
    [Parameter] public string Placeholder { get; set; } = "Vad vill du dela?";
    [Parameter] public EventCallback OnPosted { get; set; }

    private string content = "";
    private IBrowserFile? selectedFile;
    private Guid fileInputKey = Guid.NewGuid();
    private bool isPublishing;
    private string? error;

    private void OnFileSelected(InputFileChangeEventArgs e)
    {
        selectedFile = e.File;
        error = null;
    }

    private async Task Publish()
    {
        error = null;
        isPublishing = true;
        try
        {
            if (selectedFile is null)
            {
                await FeedService.CreatePostAsync(UserId, content, null, null, ProfileOwnerId);
            }
            else
            {
                var maxSize = FeedService.GetMaxSize(selectedFile.Name);
                await using var stream = selectedFile.OpenReadStream(maxSize);
                await FeedService.CreatePostAsync(UserId, content, stream, selectedFile.Name, ProfileOwnerId);
            }

            content = "";
            selectedFile = null;
            fileInputKey = Guid.NewGuid();
            await OnPosted.InvokeAsync(); // Sidan att ladda om
        }
        catch (IOException) { error = "Filen är för stor."; }
        catch (ArgumentException ex) { error = ex.Message; }
        finally { isPublishing = false; }
    }
}
