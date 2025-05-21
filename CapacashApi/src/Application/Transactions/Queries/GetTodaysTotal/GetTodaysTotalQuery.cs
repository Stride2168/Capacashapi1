using MediatR;

namespace Capacash.Application.Transaction.Queries
{
    public class GetTodaysTotalQuery : IRequest<decimal>
    {
        public string? CompanyId { get; set; }
        public Guid? KioskId { get; set; }
        public string? TransactionType { get; set; }
    }
}
