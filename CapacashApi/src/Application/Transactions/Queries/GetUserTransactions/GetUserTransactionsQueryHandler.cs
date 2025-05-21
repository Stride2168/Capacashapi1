using Capacash.Application.Common.Interfaces;
using Capacash.Application.Commons.DTOs;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Capacash.Application.Transactions.Queries.GetUserTransactions
{
    public class GetUserTransactionsQueryHandler 
        : IRequestHandler<GetUserTransactionsQuery, List<TransactionDto>>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<GetUserTransactionsQueryHandler> _logger;

        public GetUserTransactionsQueryHandler(
            ITransactionRepository transactionRepository,
            ILogger<GetUserTransactionsQueryHandler> logger)
        {
            _transactionRepository = transactionRepository;
            _logger = logger;
        }

        public async Task<List<TransactionDto>> Handle(
            GetUserTransactionsQuery request, 
            CancellationToken cancellationToken)
        {
            try
            {
                // Get the base query
                var query = _transactionRepository
                    .GetTransactionsByUserIdQueryable(request.UserId);

                // Apply filters
                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    query = query.Where(t =>
                        t.TransactionId != null && 
                        t.TransactionId.Contains(request.SearchTerm));
                }

                if (!string.IsNullOrWhiteSpace(request.TransactionType))
                {
                    query = query.Where(t =>
                        t.TransactionType.ToLower().Contains(request.TransactionType.ToLower()));
                }

                if (!string.IsNullOrWhiteSpace(request.DateRange))
                {
                    var today = DateTime.UtcNow.Date;
                    query = request.DateRange.ToLower() switch
                    {
                        "yesterday" => query.Where(t => 
                            t.TransactionDate.Date == today.AddDays(-1)),
                        "lastweek" => query.Where(t => 
                            t.TransactionDate >= today.AddDays(-7)),
                        "last30days" => query.Where(t => 
                            t.TransactionDate >= today.AddDays(-30)),
                        _ => query
                    };
                }

                // Execute the query with projection
                var result = await query
                    .OrderByDescending(t => t.TransactionDate)
                    .Select(t => new TransactionDto(
                        t.UserId,
                        t.Amount,
                        t.Id,
                        t.TransactionId,
                        t.TransactionDate,
                        t.TransactionType,
                        t.KioskName,
                        null // companyId
                    ))
                    .ToListAsync(cancellationToken);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving transactions for user {UserId}", request.UserId);
                throw; // Re-throw to let the global exception handler handle it
            }
        }
    }
}