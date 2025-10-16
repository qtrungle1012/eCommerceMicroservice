using IdentityService.Application.Common.Exceptions;
using IdentityService.Application.Interfaces.IRepositories;
using IdentityService.Domain.Entities;
using IdentityService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SharedLibrarySolution.Interfaces;
using SharedLibrarySolution.Logs;
using System.Linq.Expressions;


namespace IdentityService.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IdentityDbContext _context;
        public UserRepository(IdentityDbContext context) => _context = context;

        public async Task<User> CreateAsync(User entity)
        {
            try
            {
                await _context.Users.AddAsync(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                LogException.LogError(ex);
                throw;
            }
        }

        // delete user 
        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var existingUser = await _context.Users.FindAsync(id);
                if (existingUser == null)
                    throw new AppException("User not found");

                _context.Users.Remove(existingUser);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                LogException.LogError(ex);
                throw;
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync(Expression<Func<User, bool>> predicate)
        {
            try
            {
                var listUsers = _context.Users.ToListAsync();
                return await listUsers.ContinueWith(t => (IEnumerable<User>)t.Result);
            }
            catch (Exception ex)
            {
                LogException.LogError(ex);
                throw;
            }
        }

        // Get user by condition (VD: username, email)
        public async Task<User?> GetByAsync(Expression<Func<User, bool>> predicate)
        {
            try
            {
                var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(predicate);
                if (user == null)
                    throw new AppException("User not found");

                return user;
            }
            catch (Exception ex)
            {
                LogException.LogError(ex);
                throw;
            }
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            try
            {
                var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                    throw new AppException("User not found");

                return user;
            }
            catch (Exception ex)
            {
                LogException.LogError(ex);
                throw;
            }
        }

        

        public async Task<User> UpdateAsync(Guid id, User entity)
        {
            try
            {
                var existingUser = await _context.Users.FindAsync(id);
                if (existingUser == null)
                    throw new AppException("User not found");

                _context.Entry(existingUser).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
                return existingUser;
            }
            catch (Exception ex)
            {
                LogException.LogError(ex);
                throw;
            }

        }

  
    }
}
