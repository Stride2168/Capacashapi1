using Capacash.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Capacash.Application.Auth.Commands
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IAuthService _authService;
        private readonly ILogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(IAuthService authService, ILogger<LoginCommandHandler> logger)
        {
            _authService = authService;
            _logger = logger;
        }

   public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
{
    var passwordInfo = string.IsNullOrEmpty(request.Password) ? "null" : "[PROVIDED]";

    _logger.LogInformation("Received LoginCommand: Email={Email}, Password={PasswordStatus}", 
                           request.Email, 
                           passwordInfo);

    if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
    {
        _logger.LogWarning("Login failed due to missing email or password");
        throw new UnauthorizedAccessException("Invalid login credentials");
    }

    var token = await _authService.LoginAsync(request.Email, request.Password);
    var role = await _authService.GetUserRoleAsync(request.Email);

    return new LoginResponse(token, role);
}

    }
}
