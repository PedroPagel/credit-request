using System.ComponentModel.DataAnnotations;

namespace Cofidis.Credit.Domain.Models.Users
{
    public class UserRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public string Nif { get; set; }

        [Required]
        public decimal MonthlyIncome { get; set; }
    }
}
