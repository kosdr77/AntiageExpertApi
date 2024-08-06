using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using UserManagementService.DataContexts;
using UserManagementService.Domain.Models;
using UserManagementService.DTOs;

namespace UserManagementService.Services
{
    public class UserService : IUserService
    {
        private readonly UserContext _context;
        private readonly IValidationService _validationService;
        private readonly IMemoryCache _cache;

        public UserService(UserContext context, IValidationService validationService, IMemoryCache cache)
        {
            _context = context;
            _validationService = validationService;
            _cache = cache;
        }

        public async Task<User> CreateUserAsync(User user, string device)
        {
            _validationService.ValidateUser(user, device);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            if (!_cache.TryGetValue(id, out User user))
            {
                user = await _context.Users.FindAsync(id);
                if (user != null)
                {
                    _cache.Set(id, user, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                    });
                }
            }
            return user;
        }

        public async Task<List<User>> SearchUsersAsync(UserSearchDto userSearchDto)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(userSearchDto.LastName))
                query = query.Where(u => u.LastName.Contains(userSearchDto.LastName));
            if (!string.IsNullOrEmpty(userSearchDto.FirstName))
                query = query.Where(u => u.FirstName.Contains(userSearchDto.FirstName));
            if (!string.IsNullOrEmpty(userSearchDto.MiddleName))
                query = query.Where(u => u.MiddleName.Contains(userSearchDto.MiddleName));
            if (!string.IsNullOrEmpty(userSearchDto.Phone))
                query = query.Where(u => u.Phone.Contains(userSearchDto.Phone));
            if (!string.IsNullOrEmpty(userSearchDto.Email))
                query = query.Where(u => u.Email.Contains(userSearchDto.Email));

            return await query.ToListAsync();
        }
    }
}
