using System.Collections.Concurrent;

namespace Server.Features.Sse;

public class SetConnectionManager
{

    // Подключения пользователей
    private readonly ConcurrentDictionary<string, HttpResponse> _userConnections = new();
    // Кто какой рецепт смотрит (для real-time комментариев)
    private readonly ConcurrentDictionary<string, int> _userViewingRecipe = new();

    public void AddConnection(string userId, HttpResponse response);
    public void RemoveConnection(string userId);
    public void SetViewingRecipe(string userId, int recipeId);
    public async Task SendEventAsync(string userId, string eventType, object data);
    public async Task BroadcastToRecipeViewers(int recipeId, string eventType, object data);
}
