using Capacash.Application.Common.Interfaces;
using Capacash.Domain.Entities;
namespace Capacash.Application.Auth.Commands
{
    public class RegisterAdminCommandHandler : IRequestHandler<RegisterAdminCommand, string>
    {
        private readonly IAuthService _authService;
        private readonly IUserRepository _userRepo;
        public RegisterAdminCommandHandler( IAuthService authService,IUserRepository userRepo)
        {
            _authService = authService;
            _userRepo = userRepo;
        }

public async Task<string> Handle(RegisterAdminCommand request, CancellationToken cancellationToken)
{
    var newAdmin = new User
    {
        FullName = request.FullName,
        Email = request.Email,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
        Role = "Admin",
        IsApproved = false,
        CompanyId = request.CompanyId,
        CreatedAt = DateTime.UtcNow,
        PhoneNumber = request.PhoneNumber 
    };

    await _userRepo.AddAsync(newAdmin);
    return _authService.GenerateJwtToken(newAdmin);
}

    }
}