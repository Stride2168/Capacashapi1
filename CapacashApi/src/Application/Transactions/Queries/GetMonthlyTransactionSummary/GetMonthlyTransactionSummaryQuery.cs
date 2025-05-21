using MediatR;
using System.Collections.Generic;

namespace Capacash.Application.Transactions.Queries.GetMonthlyTransactionSummary
{
    public record MonthlyTransactionSummary(string Month, int Total);

    public record GetMonthlyTransactionSummaryQuery(string CompanyId) : IRequest<List<MonthlyTransactionSummary>>;
}
