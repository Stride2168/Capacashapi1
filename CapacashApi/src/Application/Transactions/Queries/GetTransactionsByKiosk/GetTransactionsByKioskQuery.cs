using Capacash.Application.Commons.DTOs;
using MediatR;
using System;

namespace Capacash.Application.Transaction.Queries
{
    public class GetTransactionsByKioskQuery : IRequest<List<TransactionDto>>
    {
        public string KioskCode { get; set; } = default!;
    }
}
