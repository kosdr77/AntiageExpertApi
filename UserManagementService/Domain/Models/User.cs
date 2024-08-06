using System.ComponentModel.DataAnnotations;

namespace UserManagementService.Domain.Models
{
    public class User
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string? LastName { get; set; }

        [StringLength(50)]
        public string? FirstName { get; set; }

        [StringLength(50)]
        public string? MiddleName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [RegularExpression(@"\d{4} \d{6}", ErrorMessage = "Invalid passport number format")]
        public string? PassportNumber { get; set; }

        [StringLength(100)]
        public string? BirthPlace { get; set; }

        [RegularExpression(@"7\d{10}", ErrorMessage = "Invalid phone number format")]
        public string? Phone { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(200)]
        public string? RegistrationAddress { get; set; }

        [StringLength(200)]
        public string? ResidentialAddress { get; set; }
    }
}

