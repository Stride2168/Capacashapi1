
using System;
using System.ComponentModel.DataAnnotations;

namespace Capacash.Application.Commons.DTOs
{
    public class TransactionDto
    {
        // The UserId for the transaction (Required and validated as a GUID)
        [Required]
        public Guid UserId { get; set; }  

        // The amount of the transaction (Required and validated to be greater than zero)
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        // The type of transaction (Required and validated to have a max length of 20 characters)
        [Required]
        [MaxLength(20)]
        public string TransactionType { get; set; }

        // The Transaction Id (Required)
        [Required]
        public int Id { get; set; }

        // The TransactionId (Optional, as it's nullable)
        public string? TransactionId { get; set; }

        // The date of the transaction (Required and should be a DateTime)
        [Required]
        public DateTime TransactionDate { get; set; }

        // Constructor to initialize required properties
        public TransactionDto(Guid userId, decimal amount, int id, string? transactionId, DateTime transactionDate, string transactionType)
        {
            UserId = userId;
            Amount = amount;
            Id = id;
            TransactionId = transactionId;
            TransactionDate = transactionDate;
            TransactionType = transactionType;
        }
    }
}
