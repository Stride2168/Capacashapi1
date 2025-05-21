namespace Capacash.Application.Commons.DTOs.Requests
{
    public record PendingAdminRequestDto(
        Guid UserId,
        string FullName,
        string Email,
        string CompanyId,
        DateTime CreatedAt);
}