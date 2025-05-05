using Capacash.Application.Common.Interfaces;

namespace Capacash.Application.Auth.Commands
{
    public class RegisterEmployeeCommandHandler : IRequestHandler<RegisterEmployeeCommand, string>
    {  private readonly IAuthService _authService;
        private readonly IUserRepository _userRepository;
        
        public RegisterEmployeeCommandHandler(
            IAuthService authService,
            IUserRepository userRepository)
        {
            _authService = authService;
            _userRepository = userRepository;
        }

        public async Task<string> Handle(RegisterEmployeeCommand request, CancellationToken cancellationToken)
        {
            // Check if admin exists for company
            var adminExists = await _userRepository.ExistsAdminForCompanyAsync(request.CompanyId);
            if (!adminExists)
            {
                throw new InvalidOperationException("No company found with the provided Company ID.");
            }

            return await _authService.RegisterUserAsync(
                request.FullName,
                request.Email,
                request.Password,
                request.CompanyId,
    request.PhoneNumber);
        }
    }
}
