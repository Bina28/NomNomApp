using System.Collections.Concurrent;
using System.Text.Json;

namespace Server.Features.Sse;

public class SseConnectionManager
{
    private readonly ConcurrentDictionary<string, HttpResponse> _userConnections = new();

    public void AddConnection(string userId, HttpResponse response)
    {   if (_userConnections.TryRemove(userId, out var oldResponse))
        {
            oldResponse.HttpContext.Abort();
        }
        
        _userConnections[userId] = response;
    }

    public void RemoveConnection(string userId, HttpResponse response)
    {
        _userConnections.TryRemove(new KeyValuePair<string, HttpResponse>(userId, response));
    }

    public async Task SendToUser(string targetUserId, string eventType, object data)
    {
        var json = JsonSerializer.Serialize(data);
        var message = $"event: {eventType}\ndata: {json}\n\n";



        if (_userConnections.TryGetValue(targetUserId, out var response))
        {

            try
            {
                await response.WriteAsync(message);
                await response.Body.FlushAsync();
            }
            catch { RemoveConnection(targetUserId, response); }
        }

    }

}
