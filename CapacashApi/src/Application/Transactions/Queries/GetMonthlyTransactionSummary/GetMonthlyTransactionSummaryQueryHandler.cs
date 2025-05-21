using Capacash.Application.Common.Interfaces;
using Capacash.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Capacash.Application.Transactions.Queries.GetMonthlyTransactionSummary
{
   public class GetMonthlyTransactionSummaryQueryHandler : IRequestHandler<GetMonthlyTransactionSummaryQuery, List<MonthlyTransactionSummary>>
{
    private readonly IAppDbContext _context;

    public GetMonthlyTransactionSummaryQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<MonthlyTransactionSummary>> Handle(GetMonthlyTransactionSummaryQuery request, CancellationToken cancellationToken)
    {
        var summaries = await _context.Transactions
            .Where(t => t.CompanyId == request.CompanyId)
            .ToListAsync(cancellationToken); // client-side grouping starts here

        var monthlySummary = summaries
            .GroupBy(t => new
            {
                Year = t.TransactionDate.Year,
                Month = t.TransactionDate.Month
            })
            .Select(g => new MonthlyTransactionSummary(
                $"{g.Key.Year}-{g.Key.Month:00}",
                g.Count()
            ))
            .OrderByDescending(e => e.Month)
            .ToList();

        return monthlySummary;
    }
}

}
