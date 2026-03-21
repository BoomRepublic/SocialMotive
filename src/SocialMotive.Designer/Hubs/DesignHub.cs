using Microsoft.AspNetCore.SignalR;

namespace SocialMotive.Designer.Hubs;

/// <summary>
/// SignalR hub for real-time collaboration in the Designer.
/// Clients join named session groups and broadcast design updates to each other.
/// </summary>
public class DesignHub : Hub
{
    private readonly ILogger<DesignHub> _logger;

    public DesignHub(ILogger<DesignHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Join a design session group. All clients in the group receive each other's updates.
    /// </summary>
    public async Task JoinSession(string sessionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
        _logger.LogDebug("Client {ConnectionId} joined design session {SessionId}", Context.ConnectionId, sessionId);
    }

    /// <summary>
    /// Leave a design session group.
    /// </summary>
    public async Task LeaveSession(string sessionId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId);
        _logger.LogDebug("Client {ConnectionId} left design session {SessionId}", Context.ConnectionId, sessionId);
    }

    /// <summary>
    /// Broadcast a design update to all other clients in the session.
    /// </summary>
    public async Task BroadcastUpdate(string sessionId, object payload)
    {
        await Clients.OthersInGroup(sessionId).SendAsync("ReceiveUpdate", payload);
    }
}
