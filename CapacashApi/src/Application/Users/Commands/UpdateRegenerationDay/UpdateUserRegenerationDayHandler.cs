using Capacash.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Capacash.Application.Users.Commands.UpdateRegenerationDay
{
    public class UpdateUserRegenerationDayHandler : IRequestHandler<UpdateUserRegenerationDayCommand, Unit>
    {
        private readonly IAppDbContext _context;

        public UpdateUserRegenerationDayHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateUserRegenerationDayCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user == null)
                throw new Exception("User not found.");

            if (request.RegenerationDay < 1 || request.RegenerationDay > 31)
                throw new Exception("Invalid regeneration day.");

            user.RegenerationDayOfMonth = request.RegenerationDay;

            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
