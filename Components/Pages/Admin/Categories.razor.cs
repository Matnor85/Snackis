using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Components;

namespace Snackis.Components.Pages.Admin;

public partial class Categories
{
    [Inject] private CategoryService CategoryService { get; set; } = default!;

    private List<Category> categories = new();
    private string newCategoryName = "";
    private Dictionary<int, string> newTopicNames = new();
    private string? error;

    protected override async Task OnInitializedAsync() => await LoadAsync();

    private async Task LoadAsync()
    {
        categories = await CategoryService.GetAllAsync();
        foreach (var c in categories)
            newTopicNames.TryAdd(c.Id, "");
    }

    private async Task AddCategory()
    {
        error = null;
        try
        {
            await CategoryService.CreateCategoryAsync(newCategoryName);
            newCategoryName = "";
            await LoadAsync();
        }
        catch (ArgumentException ex) { error = ex.Message; }
    }

    private async Task AddTopic(int categoryId)
    {
        error = null;
        try
        {
            await CategoryService.CreateTopicAsync(categoryId, newTopicNames[categoryId]);
            newTopicNames[categoryId] = "";
            await LoadAsync();
        }
        catch (ArgumentException ex) { error = ex.Message; }
    }

    private async Task DeleteCategory(int id)
    {
        await CategoryService.DeleteCategoryAsync(id);
        await LoadAsync();
    }

    private async Task DeleteTopic(int id)
    {
        await CategoryService.DeleteTopicAsync(id);
        await LoadAsync();
    }
}