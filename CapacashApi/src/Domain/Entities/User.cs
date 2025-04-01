namespace Capacash.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Employee"; // Default role
    public bool IsApproved { get; set; } = false;  // New approval field
    public string? CompanyId { get; set; }  // Needed for Employee registration
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
