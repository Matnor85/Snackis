using Application.DTOs;
using Microsoft.AspNetCore.Components;

namespace Snackis.Components.Shared;

public partial class FeedPostCard
{
    [Parameter, EditorRequired] public WallPostDto Post { get; set; } = null!;
    [Parameter] public string? CurrentUserId { get; set; }
    [Parameter] public bool IsAdmin { get; set; }
    [Parameter] public Func<int, string, Task>? OnComment { get; set; }
    [Parameter] public Func<int, Task>? OnDelete { get; set; }

    private string commentText = "";
    private bool confirmDelete;

    private bool CanDelete => CurrentUserId is not null &&
        (IsAdmin || Post.AuthorId == CurrentUserId || Post.ProfileOwnerId == CurrentUserId);

    private async Task SendComment()
    {
        if (string.IsNullOrWhiteSpace(commentText) || OnComment is null) return;
        await OnComment(Post.Id, commentText);
        commentText = "";
    }

    private async Task Delete()
    {
        if (OnDelete is not null) await OnDelete(Post.Id);
        confirmDelete = false;
    }
}
