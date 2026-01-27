using System.Collections.Concurrent;
using System.Text.Json;

namespace Server.Features.Sse;

public class SetConnectionManager
{
    private readonly ConcurrentDictionary<string, HttpResponse> _userConnections = new();

    public void AddConnection(string userId, HttpResponse response)
    {
        _userConnections[userId] = response;
    }

    public void RemoveConnection(string userId)
    {
        _userConnections.TryRemove(userId, out _);
    }

    public async Task BroadcastToAll(string eventType, object data)
    {
        var json = JsonSerializer.Serialize(data);
        var message = $"event: {eventType}\ndata: {json}\n\n";

        foreach (var response in _userConnections.Values)
        {
            await response.WriteAsync(message);
            await response.Body.FlushAsync();
        }
    }
}
