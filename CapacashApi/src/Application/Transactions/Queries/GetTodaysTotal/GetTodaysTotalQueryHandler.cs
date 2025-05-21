using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Capacash.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Capacash.Application.Transaction.Queries
{
    public class GetTodaysTotalQueryHandler : IRequestHandler<GetTodaysTotalQuery, decimal>
    {
        private readonly IAppDbContext _context;

        public GetTodaysTotalQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> Handle(GetTodaysTotalQuery request, CancellationToken cancellationToken)
        {
            var today = DateTime.UtcNow.Date;

            var query = _context.Transactions
                .Where(t => t.TransactionDate.Date == today);

            if (!string.IsNullOrEmpty(request.CompanyId))
                query = query.Where(t => t.CompanyId == request.CompanyId);

            if (request.KioskId.HasValue)
                query = query.Where(t => t.KioskId == request.KioskId);

            if (!string.IsNullOrEmpty(request.TransactionType))
                query = query.Where(t => t.TransactionType == request.TransactionType);

            return await query.SumAsync(t => t.Amount, cancellationToken);
        }
    }
}
