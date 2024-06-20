using apbd10.Dtos;
using apbd10.Services;
using AuthExample.Dtos.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace apbd10.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{

    private readonly IUserService _service;

    public UserController(IUserService service)
    {
        _service = service;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterUserAsync(UserRegisterDto dto)
    {
       return  Ok(await _service.RegisterUser(dto));
    }
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]

    public async Task<IActionResult> LoginUserAsync(UserLoginDto dto)
    {
        return Ok(await _service.Login(dto));
    }
    [AllowAnonymous]
    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]

    public async Task<IActionResult> RefreshUserTokenAsync(RefreshTokenRequestDto dto)
    {
        return Ok(await _service.RefreshTokenAsync(dto));
    }
    
}