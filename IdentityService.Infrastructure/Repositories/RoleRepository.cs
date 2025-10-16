using IdentityService.Application.Interfaces.IRepositories;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Data;
using System.Linq.Expressions;

namespace IdentityService.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly IdentityDbContext _context;
        public RoleRepository(IdentityDbContext context) => _context = context;

        public Task<Role> CreateAsync(Role entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Role>> GetAllAsync(Expression<Func<Role, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<Role?> GetByAsync(Expression<Func<Role, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<Role?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Role> UpdateAsync(Guid id, Role entity)
        {
            throw new NotImplementedException();
        }
    }
}
