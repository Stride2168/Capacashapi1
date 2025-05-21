namespace Capacash.Domain.Entities
{
    public class Kiosk
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string KioskId { get; private set; }
    public string PasswordHash { get; private set; }
    public string? Name { get; private set; }
    public string? Location { get; private set; }
    public string CompanyId { get; private set; }

    public bool IsActive { get; private set; } = true;
    public bool IsDeleted { get; private set; } = false;

    public Kiosk(string kioskId, string passwordHash, string? name, string? location, string companyId)
    {
        KioskId = kioskId ?? throw new ArgumentNullException(nameof(kioskId));
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        Name = name;
        Location = location;
        CompanyId = companyId ?? throw new ArgumentNullException(nameof(companyId));
    }

    public void Enable() => IsActive = true;
    public void Disable() => IsActive = false;
    public void Delete() => IsDeleted = true;
}

}
