using MediatR;
using System;

namespace Capacash.Application.Transactions.Commands.ProcessTransaction
{
    public class ProcessTransactionCommand : IRequest<string> // string = success message
    {
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public string KioskId { get; set; }

        public ProcessTransactionCommand(Guid userId, decimal amount, string kioskId)
        {
            UserId = userId;
            Amount = amount;
            KioskId = kioskId;
        }
    }
}
