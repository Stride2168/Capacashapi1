// Application/Common/Exceptions/NotFoundException.cs
// Application/Common/Exceptions/NotFoundException.cs
namespace Capacash.Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException() { }
    public NotFoundException(string message) : base(message) { }
    public NotFoundException(string message, Exception inner) : base(message, inner) { }
}