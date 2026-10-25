using Domain.Entities;
using Application.Services;
using Microsoft.AspNetCore.Components;

namespace Snackis.Components.Pages.Forum;

public partial class ForumPage
{
    [Inject] private CategoryService CategoryService { get; set; } = default!;

    private List<Category> categories = new();

    protected override async Task OnInitializedAsync() =>
        categories = await CategoryService.GetAllAsync();
}
