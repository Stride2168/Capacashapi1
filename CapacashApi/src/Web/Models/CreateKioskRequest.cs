namespace Capacash.Web.Models
{
    public class CreateKioskRequest
    {
        public string KioskId { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Name { get; set; }
        public string? Location { get; set; }
    }
}
