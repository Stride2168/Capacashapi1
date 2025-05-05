using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capacash.Domain.Entities
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? TransactionId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(20)]
        public string TransactionType { get; set; } = "Purchase";

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        public string? CompanyId { get; set; }

        // Add these two properties for Kiosk relationship
        public Guid? KioskId { get; set; }  // Nullable for non-purchase transactions
        
        [ForeignKey("KioskId")]
        public virtual Kiosk? Kiosk { get; set; }  // Navigation property

        public User? User { get; set; }

        public Transaction() { }

        // Updated constructor with optional kioskId parameter
        public Transaction(Guid userId, decimal amount, string transactionType = "Purchase", 
                          string? companyId = null, Guid? kioskId = null)
        {
            UserId = userId;
            Amount = amount;
            TransactionDate = DateTime.UtcNow;
            TransactionId = GenerateTransactionId();
            TransactionType = transactionType;
            CompanyId = companyId;
            KioskId = transactionType == "Purchase" ? kioskId : null;  // Only set for purchases
        }

        private string GenerateTransactionId()
        {
            return $"TXN-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }
    }
}