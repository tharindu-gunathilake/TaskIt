using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskIt.Interfaces;
using TaskIt.Models.Requests;

namespace TaskIt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto newUser)
        {
            var response = await _authService.RegisterUserAsync(newUser);

            return StatusCode(
                response.StatusCode ?? StatusCodes.Status200OK,
                response
            );
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthenticateUserDto userCredentials)
        {
            var response = await _authService.AuthenticateUserAsync(userCredentials);

            return StatusCode(
                response.StatusCode ?? StatusCodes.Status200OK,
                response
            );
        }
    }
}
