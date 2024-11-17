namespace Cofidis.Credit.Api.Dto
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string NIF { get; set; }
        public decimal MonthlyIncome { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
