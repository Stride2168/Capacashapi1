using Capacash.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Capacash.Domain.Entities;

namespace Capacash.Application.Users.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, string>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher; // Changed to User

        public ResetPasswordCommandHandler(
            IUserRepository userRepository, 
            IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<string> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
                throw new UnauthorizedAccessException("User not found.");

            if (user.LastPasswordResetAt.HasValue &&
                user.LastPasswordResetAt.Value.AddMonths(3) > DateTime.UtcNow)
            {
                var nextReset = user.LastPasswordResetAt.Value.AddMonths(3);
                throw new InvalidOperationException($"Password can only be reset after {nextReset:yyyy-MM-dd}.");
            }

            var hashedPassword = _passwordHasher.HashPassword(user, request.NewPassword);
            user.PasswordHash = hashedPassword;
            user.LastPasswordResetAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            return "Password reset successful.";
        }
    }
}