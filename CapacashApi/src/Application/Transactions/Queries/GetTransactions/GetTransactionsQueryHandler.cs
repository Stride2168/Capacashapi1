// File: Application/Transactions/Queries/GetTransactions/GetTransactionsQueryHandler.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Capacash.Application.Common.Interfaces;
using Capacash.Application.Commons.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Capacash.Application.Transactions.Queries.GetTransactions
{
    public class GetTransactionsQueryHandler : IRequestHandler<GetTransactionsQuery, List<TransactionDto>>
    {
        private readonly IAppDbContext _context;

        public GetTransactionsQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TransactionDto>> Handle(GetTransactionsQuery request, CancellationToken cancellationToken)
        {
            // Check if the UserId is in valid GUID format
            if (!Guid.TryParse(request.UserId.ToString(), out var parsedUserId))
            {
                throw new FormatException("The provided UserId is not a valid GUID.");
            }

            var query = _context.Transactions
                .Include(t => t.User)  // Necessary to access User.CompanyId
                .Include(t => t.Kiosk) // Include Kiosk to get the KioskName
                .AsNoTracking()
                .Where(t => t.User != null && t.User.CompanyId == request.CompanyId); // <-- Filter using string CompanyId

            if (parsedUserId != Guid.Empty) // Only filter if parsedUserId is not empty
                query = query.Where(t => t.UserId == parsedUserId);

            if (request.StartDate.HasValue)
                query = query.Where(t => t.TransactionDate >= request.StartDate.Value);

            if (request.EndDate.HasValue)
                query = query.Where(t => t.TransactionDate <= request.EndDate.Value);

            // Now we handle the null checking of KioskName outside the LINQ expression
            var transactions = await query
                .OrderByDescending(t => t.TransactionDate)
                .Select(t => new 
                {
                    t.UserId,
                    t.Amount,
                    t.Id,
                    t.TransactionId,
                    t.TransactionDate,
                    t.TransactionType,
                    KioskName = t.Kiosk != null ? t.Kiosk.Name : null  // Handle null checking here
                })
                .ToListAsync(cancellationToken);

            // Now map to TransactionDto
            var transactionDtos = transactions.Select(t => new TransactionDto(
                t.UserId,
                t.Amount,
                t.Id,
                t.TransactionId,
                t.TransactionDate,
                t.TransactionType,
                t.KioskName
            )).ToList();

            if (transactionDtos.Count == 0)
            {
                // Log the case where no transactions are found
                Console.WriteLine($"No transactions found for UserId: {request.UserId}");
            }

            return transactionDtos;
        }
    }
}
