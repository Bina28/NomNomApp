using System.Collections.Concurrent;
using System.Text.Json;

namespace Server.Features.Sse;

public class SetConnectionManager
{
    private readonly ConcurrentDictionary<string, HttpResponse> _userConnections = new();
    private readonly ConcurrentDictionary<string, int> _userViewingRecipe = new();

    public void AddConnection(string userId, HttpResponse response)
    {
        _userConnections[userId] = response;
    }

    public void RemoveConnection(string userId)
    {
        _userConnections.TryRemove(userId, out _);
        _userViewingRecipe.TryRemove(userId, out _);
    }

    public void SetViewingRecipe(string userId, int recipeId)
    {
        _userViewingRecipe[userId] = recipeId;
    }

    public async Task SendEventAsync(string userId, string eventType, object data)
    {
        if (_userConnections.TryGetValue(userId, out var response))
        {
            var json = JsonSerializer.Serialize(data);
            var message = $"event: {eventType}\ndata: {json}\n\n";
            await response.WriteAsync(message);
            await response.Body.FlushAsync();
        }
    }

    public async Task BroadcastToRecipeViewers(int recipeId, string eventType, object data)
    {
        var viewers = _userViewingRecipe
            .Where(kvp => kvp.Value == recipeId)
            .Select(kvp => kvp.Key);

        foreach (var userId in viewers)
        {
            await SendEventAsync(userId, eventType, data);
        }
    }
}
