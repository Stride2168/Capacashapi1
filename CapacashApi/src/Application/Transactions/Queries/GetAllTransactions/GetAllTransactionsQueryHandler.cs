using Capacash.Application.Common.Interfaces;
using Capacash.Application.Commons.DTOs;
using Capacash.Application.Transactions.Queries.GetAllTransactions;
using Capacash.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Capacash.Application.Transactions.Queries.GetAllCompanyTransactions
{
    public class GetAllTransactionsQueryHandler : IRequestHandler<GetAllTransactionsQuery, List<TransactionDto>>
    {
        private readonly IAppDbContext _context;

        public GetAllTransactionsQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TransactionDto>> Handle(GetAllTransactionsQuery request, CancellationToken cancellationToken)
        {
            var transactions = await _context.Transactions
                .Where(t => t.CompanyId == request.CompanyId)
                .OrderByDescending(t => t.TransactionDate)
                .Select(t => new TransactionDto
                {
                    TransactionId = t.TransactionId,
                    Amount = t.Amount,
                    TransactionDate = t.TransactionDate,
                    UserId = t.UserId,
                    CompanyId = t.CompanyId
                })
                .ToListAsync(cancellationToken);

            return transactions;
        }
    }
}
