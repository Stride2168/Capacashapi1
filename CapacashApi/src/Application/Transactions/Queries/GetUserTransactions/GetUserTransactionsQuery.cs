using MediatR;
using Capacash.Application.Commons.DTOs;
using System;

namespace Capacash.Application.Transactions.Queries.GetUserTransactions
{
    public class GetUserTransactionsQuery : IRequest<List<TransactionDto>>
    {
        public Guid UserId { get; }
        public string? SearchTerm { get; }          // Searches TransactionId or Notes
        public string? TransactionType { get; }     // Must match your enum/types (e.g., "Purchase")
        public string? DateRange { get; }           // "Yesterday", "LastWeek", "Last30Days"

        public GetUserTransactionsQuery(
            Guid userId,
            string? searchTerm = null,
            string? transactionType = null,
            string? dateRange = null)
        {
            UserId = userId;
            SearchTerm = searchTerm;
            TransactionType = transactionType;
            DateRange = dateRange;
        }
    }
}