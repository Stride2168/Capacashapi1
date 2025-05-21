namespace Capacash.Application.Transaction.Commands{
public record GenerateQrCommand(decimal Amount) : IRequest<string>; 
}