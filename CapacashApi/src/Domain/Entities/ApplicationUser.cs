using Microsoft.AspNetCore.Identity;
namespace Capacash.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public DateTime? LastPasswordResetAt { get; set; }
}
