using Application.DTOS;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Snackis.Api;

[ApiController]
[Route("api/discussions")]
public class DiscussionsController : ControllerBase
{
    private readonly PostService _postService;

    public DiscussionsController(PostService postService)
    {
        _postService = postService;
    }

    // GET api/discussions/1  → diskussionen med inläggen som träd
    [HttpGet("{id:int}")]
    public async Task<ActionResult<DiscussionDetailsDto>> GetDiscussion(int id)
    {
        var discussion = await _postService.GetDiscussionAsync(id);

        if (discussion is null)
            return NotFound();

        return discussion;
    }

    // GET api/discussions/1/posts  → alla inlägg med författare och datum (G-kravet)
    [HttpGet("{id:int}/posts")]
    public async Task<ActionResult<List<PostDto>>> GetPosts(int id)
    {
        var posts = await _postService.GetPostsAsync(id);

        if (posts is null)
            return NotFound();

        return posts;
    }
}