// Application/Common/Exceptions/NotFoundException.cs
namespace Capacash.Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException() { }
    
    public NotFoundException(string message) : base(message) { }
    
    public NotFoundException(string message, Exception innerException) 
        : base(message, innerException) { }
    
    // Optional: Keep this if you need the entity name/key format
    public NotFoundException(string entityName, object key)
    : base($"Entity \"{entityName}\" ({key}) was not found.") { }

}