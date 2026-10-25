using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Snackis.Api;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly ProfileService _profileService;

    public UsersController(ProfileService profileService)
    {
        _profileService = profileService;
    }

    // GET api/users/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserInfoDto>> GetUser(int id)
    {
        var user = await _profileService.GetUserInfoAsync(id);

        if (user is null)
            return NotFound();

        return user;
    }
}