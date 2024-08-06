using UserManagementService.Domain.Models;
using UserManagementService.DTOs;

namespace UserManagementService.Services
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(User user, string device);
        Task<User?> GetUserByIdAsync(int id);
        Task<List<User>> SearchUsersAsync(UserSearchDto userSearchDto);
    }
}