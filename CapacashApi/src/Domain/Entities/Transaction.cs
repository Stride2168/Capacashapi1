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
        public string? TransactionId { get; set; } // ✅ Unique Transaction ID

        [Required]
        public Guid UserId { get; set; } 

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        public User? User { get; set; }

        public Transaction() { }

        public Transaction(Guid userId, decimal amount)
        {
            UserId = userId;
            Amount = amount;
            TransactionDate = DateTime.UtcNow;
            TransactionId = GenerateTransactionId(); // ✅ Generate Transaction ID
        }

        private string GenerateTransactionId()
        {
            return $"TXN-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }
    }
}
