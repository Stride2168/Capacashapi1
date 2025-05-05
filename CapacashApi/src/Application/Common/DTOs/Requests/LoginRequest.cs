namespace Capacash.Application.Commons.DTOs{
   public class LoginRequest
    {
        public string? Email { get; set; }
        public string? KioskId { get; set; }
        public required string Password { get; set; }
    }}