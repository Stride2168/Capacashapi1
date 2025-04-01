namespace Capacash.Domain.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? CompanyId { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
    }
}
