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

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; } = 0.00m;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public Wallet() { }

        public Wallet(Guid userId)
        {
            Id = Guid.NewGuid();
            UserId = userId;
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
    }
}
