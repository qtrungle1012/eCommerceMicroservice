﻿using IdentityService.Application.Common.Mappings;
using IdentityService.Domain.Entities;

namespace IdentityService.Application.DTOs
{
    public class UserDTO : IMapFrom<User>
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
