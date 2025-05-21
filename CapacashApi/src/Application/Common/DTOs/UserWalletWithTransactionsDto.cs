// UserWalletWithTransactionsDto.cs
using System;
using System.Collections.Generic;

namespace Capacash.Application.Commons.DTOs
{
    public class UserWalletWithTransactionsDto
    {
        public Guid UserId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Status { get; set; }
        public string? CompanyId { get; set; }
        public decimal MonthlyLimit { get; set; }
        public decimal CurrentBalance { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<TransactionDto> Transactions { get; set; } = new List<TransactionDto>();
    }
}