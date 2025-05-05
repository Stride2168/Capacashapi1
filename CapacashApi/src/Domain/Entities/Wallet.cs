using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capacash.Domain.Entities
{
    public class Wallet
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        // CompanyId should be consistent with the associated User's CompanyId
        [Required]
        public string? CompanyId { get; set; } 

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; } = 0.00m;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation property to User (for retrieving the CompanyId when necessary)
        public User User { get; set; } = null!;

        public Wallet() { }

        // Constructor ensuring the CompanyId is pulled from the User entity
        public Wallet(Guid userId, string companyId)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            CompanyId = companyId;  
            Balance = 0.00m;  
            CreatedAt = DateTime.UtcNow; 
        }

        public void DeductBalance(decimal amount)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be greater than zero.");
            if (Balance < amount) throw new InvalidOperationException("Insufficient balance.");

            Balance -= amount;
            UpdatedAt = DateTime.UtcNow;
        }

        // Ensure the CompanyId is consistent with the User entity
        public void UpdateCompanyId(string userCompanyId)
        {
            if (CompanyId != userCompanyId)
            {
                CompanyId = userCompanyId;
                UpdatedAt = DateTime.UtcNow;
            }
        }
        public void TransferTo(Wallet recipientWallet, decimal amount)
{
    if (recipientWallet == null) throw new ArgumentNullException(nameof(recipientWallet));
    if (amount <= 0) throw new ArgumentException("Amount must be greater than zero.");
    if (Balance < amount) throw new InvalidOperationException("Insufficient balance.");

    this.Balance -= amount;
    recipientWallet.Balance += amount;

    this.UpdatedAt = DateTime.UtcNow;
    recipientWallet.UpdatedAt = DateTime.UtcNow;
}

    }
}
