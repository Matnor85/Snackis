using Application.DTOs;
using Application.DTOS;
using Microsoft.AspNetCore.Components;

namespace Snackis.Components.Pages;

public partial class ApiTest
{
    [Inject] IHttpClientFactory HttpClientFactory { get; set; } = default!;
    [Inject] NavigationManager Navigation { get; set; } = default!;

    private int? discussionId;
    private List<PostDto>? posts;
    private bool postsNotFound;

    private int? userId;
    private UserInfoDto? user;
    private bool userNotFound;

    private HttpClient CreateClient()
    {
        var client = HttpClientFactory.CreateClient();
        client.BaseAddress = new Uri(Navigation.BaseUri);
        return client;
    }

    private async Task GetPosts()
    {
        if (discussionId is null) return;

        posts = null;
        postsNotFound = false;
        // API anrop
        var response = await CreateClient().GetAsync($"api/discussions/{discussionId}/posts");

        if (response.IsSuccessStatusCode)
            posts = await response.Content.ReadFromJsonAsync<List<PostDto>>();
        else
            postsNotFound = true;
    }

    private async Task GetUser()
    {
        if (userId is null) return;

        user = null;
        userNotFound = false;
        // API anrop
        var response = await CreateClient().GetAsync($"api/users/{userId}");

        if (response.IsSuccessStatusCode)
            user = await response.Content.ReadFromJsonAsync<UserInfoDto>();
        else
            userNotFound = true;
    }
}
