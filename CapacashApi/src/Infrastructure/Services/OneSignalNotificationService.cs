using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Capacash.Application.Common.Interfaces;

public class OneSignalNotificationService : INotificationService
{
    private readonly string _appId;
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;

    public OneSignalNotificationService(IConfiguration config)
    {
        _appId = config["OneSignal:AppId"] ?? throw new ArgumentNullException("OneSignal:AppId is missing in config.");
        _apiKey = config["OneSignal:ApiKey"] ?? throw new ArgumentNullException("OneSignal:ApiKey is missing in config.");
        _httpClient = new HttpClient();
    }

    public async Task SendNotificationAsync(string userId, string title, string message)
    {
        var payload = new
        {
            app_id = _appId,
            headings = new { en = title },
            contents = new { en = message },
            include_external_user_ids = new[] { userId }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "https://onesignal.com/api/v1/notifications")
        {
            Headers = {
                { "Authorization", $"Basic {_apiKey}" }
            },
            Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
        };

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
}
