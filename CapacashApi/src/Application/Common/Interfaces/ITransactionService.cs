namespace Capacash.Application.Common.Interfaces
{
    public interface ITransactionService
    {
        Task<bool> ProcessTransactionAsync(Guid userId, decimal amount);
    }
}
