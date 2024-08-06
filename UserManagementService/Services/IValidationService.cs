using UserManagementService.Domain.Models;

namespace UserManagementService.Services
{
    public interface IValidationService
    {
        void ValidateUser(User user, string device);
    }
}