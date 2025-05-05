
namespace Capacash.Application.Commons.DTOs
{
public class RegisterEmployeeDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string CompanyId { get; set; } = string.Empty; 
    
    public string PhoneNumber { get; set; } = string.Empty; 
}
}