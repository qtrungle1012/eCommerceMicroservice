using IdentityService.Application.DTOs;
using IdentityService.Application.Features.Auth.Command.Login;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using SharedLibrarySolution.Responses;

namespace IdentityService.Presentation.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    //[Route("api/v{version:apiVersion}/auth")]
    [ApiController]
    [ApiVersion("1.0")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand request)
        {

            var tokenResponse = await _mediator.Send(request);
            var response = new ApiResponse<TokenResponse>(StatusCodes.Status200OK, tokenResponse);
            return Ok(response);

        }
    }
}
