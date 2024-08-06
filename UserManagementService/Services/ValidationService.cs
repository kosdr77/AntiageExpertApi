using UserManagementService.Domain.Models;

namespace UserManagementService.Services
{
    public class ValidationService : IValidationService
    {
        public void ValidateUser(User user, string device)
        {
            switch (device.ToLower())
            {
                case "mail":
                    if (string.IsNullOrEmpty(user.FirstName) || string.IsNullOrEmpty(user.Email))
                        throw new ArgumentException("First Name and Email are required for mail device");
                    break;
                case "mobile":
                    if (string.IsNullOrEmpty(user.Phone))
                        throw new ArgumentException("Phone number is required for mobile device");
                    break;
                case "web":
                    if (string.IsNullOrEmpty(user.LastName) || string.IsNullOrEmpty(user.FirstName) ||
                        string.IsNullOrEmpty(user.MiddleName) || user.DateOfBirth == default ||
                        string.IsNullOrEmpty(user.PassportNumber) || string.IsNullOrEmpty(user.BirthPlace) ||
                        string.IsNullOrEmpty(user.Phone) || string.IsNullOrEmpty(user.RegistrationAddress))
                        throw new ArgumentException("All fields except Email and Residential Address are required for web device");
                    break;
                default:
                    throw new ArgumentException("Unknown device type");
            }
        }
    }
}
