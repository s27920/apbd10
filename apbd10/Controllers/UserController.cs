using apbd10.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace apbd10.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(UserRegisterDto dto)
    {
        throw new NotImplementedException();
    }
}