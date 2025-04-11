using System;
using System.Threading;
using System.Threading.Tasks;
using Capacash.Application.Common.Interfaces;
using MediatR;

namespace Capacash.Application.Transactions.Commands.ProcessTransaction
{
    public class ProcessTransactionCommandHandler : IRequestHandler<ProcessTransactionCommand, string>
    {
        private readonly ITransactionService _transactionService;
        private readonly IKioskRepository _kioskRepository;

        public ProcessTransactionCommandHandler(ITransactionService transactionService, IKioskRepository kioskRepository)
        {
            _transactionService = transactionService;
            _kioskRepository = kioskRepository;
        }

        public async Task<string> Handle(ProcessTransactionCommand request, CancellationToken cancellationToken)
        {
            // ✅ Validate kiosk
            var kiosk = await _kioskRepository.GetKioskByKioskIdAsync(request.KioskId);
            if (kiosk == null)
                throw new UnauthorizedAccessException("Unauthorized: Invalid kiosk.");

            // ✅ Validate user ID
            if (request.UserId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.");

            // ✅ Call the transaction service
            await _transactionService.ProcessTransactionAsync(request.UserId, request.Amount);

            return "Transaction successful.";
        }
    }
}
