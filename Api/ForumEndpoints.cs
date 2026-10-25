using Application.Services;

namespace Snackis.Api;

public static class ForumEndpoints
{
    public static void MapForumApi(this WebApplication app)
    {
        var api = app.MapGroup("/api");

        // Alla rubriker med sina ämnen
        api.MapGet("/categories", async (CategoryService service) =>
        {
            var categories = await service.GetAllAsync();

            // Vi väljer ut fälten själva, annars blir det en evig loop (Topic → Category → Topic...)
            return Results.Ok(categories.Select(c => new
            {
                c.Id,
                c.Name,
                Topics = c.Topics.Select(t => new { t.Id, t.Name })
            }));
        });

        // Diskussionerna i ett ämne
        api.MapGet("/topics/{id:int}/discussions", async (int id, DiscussionService service) =>
        {
            if (await service.GetTopicAsync(id) is null)
                return Results.NotFound();

            return Results.Ok(await service.GetByTopicAsync(id));
        });

        // En diskussion med inläggen i trädstruktur
        api.MapGet("/discussions/{id:int}", async (int id, PostService service) =>
        {
            var discussion = await service.GetDiscussionAsync(id);
            return discussion is null ? Results.NotFound() : Results.Ok(discussion);
        });

        // Alla inlägg i en diskussion
        api.MapGet("/discussions/{id:int}/posts", async (int id, PostService service) =>
        {
            var posts = await service.GetPostsAsync(id);
            return posts is null ? Results.NotFound() : Results.Ok(posts);
        });
    }
}