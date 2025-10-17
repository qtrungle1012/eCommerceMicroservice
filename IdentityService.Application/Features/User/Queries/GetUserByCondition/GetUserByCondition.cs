using IdentityService.Application.DTOs;
using MediatR;

namespace IdentityService.Application.Features.User.Queries.GetUserByCondition
{
    public class GetUserByCondition : IRequest<UserDTO>
    {
        public string? Keyword { get; set; }

    }
}
