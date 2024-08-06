using System.ComponentModel.DataAnnotations;

namespace UserManagementService.DTOs
{
    public class UserSearchDto
    {
        [StringLength(50)]
        public string? LastName { get; set; }

        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? MiddleName { get; set; }

        [RegularExpression(@"7\d{10}", ErrorMessage = "Invalid phone number format")]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(LastName) &&
                   string.IsNullOrEmpty(FirstName) &&
                   string.IsNullOrEmpty(MiddleName) &&
                   string.IsNullOrEmpty(Phone) &&
                   string.IsNullOrEmpty(Email);
        }
    }

}
