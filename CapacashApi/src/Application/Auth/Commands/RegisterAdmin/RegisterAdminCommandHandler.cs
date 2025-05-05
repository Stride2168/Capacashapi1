using Capacash.Application.Common.Interfaces;

namespace Capacash.Application.Auth.Commands
{
    public class RegisterAdminCommandHandler : IRequestHandler<RegisterAdminCommand, string>
    {
        private readonly IAuthService _authService;
        
        public RegisterAdminCommandHandler( IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<string> Handle(RegisterAdminCommand request, CancellationToken cancellationToken)
        {
            return await _authService.RegisterAdminAsync(
                request.FullName,
                request.Email,
                request.Password,
                request.CompanyId);
        }
    }
}