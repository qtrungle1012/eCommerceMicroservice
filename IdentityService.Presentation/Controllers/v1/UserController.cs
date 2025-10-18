using IdentityService.Application.DTOs;
using IdentityService.Application.Features.User.Commands.CreateUser;
using IdentityService.Application.Features.User.Commands.DeleteUser;
using IdentityService.Application.Features.User.Commands.UpdateUser;
using IdentityService.Application.Features.User.Queries.GetUserById;
using IdentityService.Application.Features.User.Queries.GetUsers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrarySolution.Responses;

namespace IdentityService.Presentation.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: /api/v1/user
        // Lấy danh sách user có phân trang + lọc keyword
        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] GetUsersQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(new ApiResponse<PageResponse<UserDTO>>(StatusCodes.Status200OK, result));
        }

        // GET: /api/v1/user/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var result = await _mediator.Send(new GetUserByIdQuery { Id = id });
            return Ok(new ApiResponse<UserDTO>(StatusCodes.Status200OK, result));
        }

        // POST: /api/v1/user
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new ApiResponse<UserDTO>(StatusCodes.Status201Created, result));
        }

        // PUT: /api/v1/user/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserCommand command)
        {
            // đảm bảo id trong URL khớp với command
            command.Id = id;
            var result = await _mediator.Send(command);
            return Ok(new ApiResponse<UserDTO>(StatusCodes.Status200OK, result));
        }

        // DELETE: /api/v1/user/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            await _mediator.Send(new DeleteUserCommand { Id = id });
            return Ok(new ApiResponse<string>(StatusCodes.Status200OK,"User deleted successfully"));
        }
    }
}
