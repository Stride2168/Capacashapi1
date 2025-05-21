using MediatR;

using Capacash.Application.Commons.DTOs;

namespace Capacash.Application.Transactions.Queries.GetAllTransactions
{
    public class GetAllTransactionsQuery : IRequest<List<TransactionDto>>
    {
        public string CompanyId { get; }

        public GetAllTransactionsQuery(string companyId)
        {
            CompanyId = companyId;
        }
    }
}
