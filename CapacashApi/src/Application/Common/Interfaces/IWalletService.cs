using System;
using System.Threading.Tasks;

namespace Capacash.Application.Common.Interfaces
{
    public interface IWalletService
    {
        Task<bool> CreateWalletForUserAsync(Guid userId);
    }
}
