using Capacash.Application.Common.Interfaces;
using Capacash.Application.Commons.DTOs;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace Capacash.Application.Transactions.Queries.GetUserTransactions
{
    public class GetUserTransactionsQueryHandler 
        : IRequestHandler<GetUserTransactionsQuery, List<TransactionDto>>
    {
        private readonly ITransactionRepository _transactionRepository;

        public GetUserTransactionsQueryHandler(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<List<TransactionDto>> Handle(
            GetUserTransactionsQuery request, 
            CancellationToken cancellationToken)
        {
            var transactions = await _transactionRepository
                .GetTransactionsByUserIdAsync(request.UserId);

            // Apply filters
            var filtered = transactions.AsQueryable();

            // 1. Search Term (TransactionId only)
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                filtered = filtered.Where(t =>
                    t.TransactionId != null && 
                    t.TransactionId.Contains(request.SearchTerm));
            }

            // 2. Transaction Type (Case-insensitive)
            if (!string.IsNullOrWhiteSpace(request.TransactionType))
            {
                filtered = filtered.Where(t =>
                    t.TransactionType.Equals(
                        request.TransactionType, 
                        StringComparison.OrdinalIgnoreCase));
            }

            // 3. Date Range
            if (!string.IsNullOrWhiteSpace(request.DateRange))
            {
                var today = DateTime.UtcNow.Date;
                filtered = request.DateRange.ToLower() switch
                {
                    "yesterday" => filtered.Where(t => 
                        t.TransactionDate.Date == today.AddDays(-1)),
                    "lastweek" => filtered.Where(t => 
                        t.TransactionDate >= today.AddDays(-7)),
                    "last30days" => filtered.Where(t => 
                        t.TransactionDate >= today.AddDays(-30)),
                    _ => filtered
                };
            }

            return filtered
                .OrderByDescending(t => t.TransactionDate)
                .Select(t => new TransactionDto(
                    t.UserId,       
                    t.Amount,      
                    t.Id,          
                    t.TransactionId, 
                    t.TransactionDate,
                    t.TransactionType
                ))
                .ToList();
        }
    }
}