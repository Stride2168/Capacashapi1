namespace Capacash.Domain.Entities
{
    public class Kiosk
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string KioskId { get; private set; }
        public string PasswordHash { get; private set; }
        public string? Name { get; private set; }
        public string? Location { get; private set; }
        public string CompanyId { get; private set; }  // Add CompanyId property

        // ✅ Constructor
        public Kiosk(string kioskId, string passwordHash, string? name, string? location, string companyId)
        {
            KioskId = kioskId ?? throw new ArgumentNullException(nameof(kioskId));
            PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
            Name = name;
            Location = location;
            CompanyId = companyId ?? throw new ArgumentNullException(nameof(companyId));  // Ensure CompanyId is set
        }
    }
}
