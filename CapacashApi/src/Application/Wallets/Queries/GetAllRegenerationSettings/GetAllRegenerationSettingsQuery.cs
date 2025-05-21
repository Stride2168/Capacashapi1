using Capacash.Domain.Entities;
using Capacash.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

public record GetAllRegenerationSettingsQuery : IRequest<List<WalletRegenerationSetting>>;

public class GetAllRegenerationSettingsHandler : IRequestHandler<GetAllRegenerationSettingsQuery, List<WalletRegenerationSetting>>
{
    private readonly IAppDbContext _context;

    public GetAllRegenerationSettingsHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<WalletRegenerationSetting>> Handle(GetAllRegenerationSettingsQuery request, CancellationToken cancellationToken)
    {
        return await _context.WalletRegenerationSettings.ToListAsync(cancellationToken);
    }
}
