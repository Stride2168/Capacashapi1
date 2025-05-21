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

        // Kiosk relationship properties
        public Guid? KioskId { get; set; }

        // New KioskCode property
        public string? KioskCode { get; set; }

        [ForeignKey("KioskId")]
        public virtual Kiosk? Kiosk { get; set; }

        // Add KioskName property
        public string? KioskName { get; set; }

        public User? User { get; set; }

        public Transaction() { }

        // Updated constructor with kioskCode parameter
        public Transaction(Guid userId, decimal amount, string transactionType = "Purchase", 
                           string? companyId = null, Guid? kioskId = null, string? kioskName = null, string? kioskCode = null)
        {
            UserId = userId;
            Amount = amount;
            TransactionDate = DateTime.UtcNow;
            TransactionId = GenerateTransactionId();
            TransactionType = transactionType;
            CompanyId = companyId;
            KioskId = transactionType == "Purchase" ? kioskId : null;
            KioskName = transactionType == "Purchase" ? kioskName : null;
            KioskCode = transactionType == "Purchase" ? kioskCode : null;
        }

        private string GenerateTransactionId()
        {
            return $"TXN-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }
    }
}
