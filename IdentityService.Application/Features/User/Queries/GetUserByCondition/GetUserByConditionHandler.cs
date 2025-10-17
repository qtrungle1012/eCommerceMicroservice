using AutoMapper;
using IdentityService.Application.Common.Exceptions;
using IdentityService.Application.DTOs;
using IdentityService.Application.Interfaces.IRepositories;
using MediatR;

namespace IdentityService.Application.Features.User.Queries.GetUserByCondition
{
    public class GetUserByConditionHandler : IRequestHandler<GetUserByCondition, UserDTO>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUserByConditionHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
        public async Task<UserDTO> Handle(GetUserByCondition request, CancellationToken cancellationToken)
        {

            if (string.IsNullOrWhiteSpace(request.Keyword))
                throw new AppException("Keyword is required");

            var user = await _userRepository.GetByAsync(u =>
                u.UserName == request.Keyword || u.Email == request.Keyword);

            return _mapper.Map<UserDTO>(user);
        }
    }
}
