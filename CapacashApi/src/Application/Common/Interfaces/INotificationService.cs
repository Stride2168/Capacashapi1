namespace Capacash.Application.Common.Interfaces{
  public interface INotificationService
{
    Task SendNotificationAsync(string userId, string title, string message);
}

}