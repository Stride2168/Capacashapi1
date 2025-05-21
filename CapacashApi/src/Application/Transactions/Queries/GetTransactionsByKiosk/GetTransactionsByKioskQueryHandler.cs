using Capacash.Application.Commons.DTOs;
using Capacash.Application.Transaction.Queries;
using Capacash.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Capacash.Application.Transaction.Handlers;

public class GetTransactionsByKioskQueryHandler : IRequestHandler<GetTransactionsByKioskQuery, List<TransactionDto>>
{
    private readonly IAppDbContext _context;

    public GetTransactionsByKioskQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

public async Task<List<TransactionDto>> Handle(GetTransactionsByKioskQuery request, CancellationToken cancellationToken)
{
    if (string.IsNullOrEmpty(request.KioskCode))
    {
        throw new ArgumentException("KioskCode is required.");
    }

    // Try to parse the KioskCode to Guid
    if (!Guid.TryParse(request.KioskCode, out var kioskGuid))
    {
        throw new ArgumentException("Invalid KioskCode format.");
    }

    // Query the transactions with the parsed Guid
    var transactions = await _context.Transactions
        .Where(t => t.KioskId == kioskGuid)  // Now comparing Guid? to Guid
        .OrderByDescending(t => t.TransactionDate)
        .Select(t => new TransactionDto(
            t.UserId,
            t.Amount,
            t.Id,
            t.TransactionId,
            t.TransactionDate,
            t.TransactionType,
            t.KioskName,
            t.CompanyId
        ))
        .ToListAsync(cancellationToken);

    return transactions;
}

}

