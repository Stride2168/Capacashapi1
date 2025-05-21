using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capacash.Domain.Entities;

public class WalletRegenerationSetting
{
    [Key]
    public Guid Id { get; private set; } = Guid.NewGuid();

    [Required]
    public string CompanyId { get; private set; } = null!;

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal MonthlyAmount { get; private set; }

    [Required]
    public int RegenerationDayOfMonth { get; private set; } // 1 to 28 ideally

    public WalletRegenerationSetting(string companyId, decimal monthlyAmount, int dayOfMonth)
    {
        if (monthlyAmount <= 0) throw new ArgumentException("Monthly amount must be greater than 0.");
        if (dayOfMonth < 1 || dayOfMonth > 28) throw new ArgumentException("Regeneration day must be between 1 and 28.");

        CompanyId = companyId;
        MonthlyAmount = monthlyAmount;
        RegenerationDayOfMonth = dayOfMonth;
    }

    // EF Constructor
    private WalletRegenerationSetting() { }
}
