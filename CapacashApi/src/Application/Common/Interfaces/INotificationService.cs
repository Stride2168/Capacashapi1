namespace Capacash.Application.Common.Interfaces{
  public interface INotificationService
{
    Task SendNotificationAsync(Guid userId, string title, string message);
}

}