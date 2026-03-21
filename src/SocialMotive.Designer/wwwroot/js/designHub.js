let hubConnection = null;

/// <summary>
/// Initialise the DesignHub SignalR connection and join a session group.
/// Called from Blazor via JS interop after the component mounts.
/// </summary>
/// <param name="sessionId">Session group ID to join for scoped broadcasts.</param>
/// <param name="dotNetRef">DotNetObjectReference used to invoke Blazor callbacks on incoming events.</param>
export function initDesignHub(sessionId, dotNetRef) {
    if (hubConnection) return;

    const signalR = window.signalR;
    if (!signalR) {
        console.warn('[DesignHub] signalR not loaded');
        return;
    }

    hubConnection = new signalR.HubConnectionBuilder()
        .withUrl('/designhub')
        .withAutomaticReconnect()
        .build();

    // Receive a design update broadcast — invoke back into Blazor
    hubConnection.on('ReceiveUpdate', (payload) => {
        if (dotNetRef) {
            dotNetRef.invokeMethodAsync('OnReceiveUpdate', payload)
                .catch(err => console.error('[DesignHub] OnReceiveUpdate callback error:', err));
        }
    });

    hubConnection.onreconnecting(() => console.info('[DesignHub] Reconnecting...'));
    hubConnection.onreconnected(() => {
        console.info('[DesignHub] Reconnected');
        if (sessionId) joinSession(sessionId);
    });

    hubConnection.start()
        .then(() => {
            console.info('[DesignHub] Connected');
            if (sessionId) joinSession(sessionId);
        })
        .catch(err => console.error('[DesignHub] Connection error:', err));
}

function joinSession(sessionId) {
    hubConnection.invoke('JoinSession', sessionId)
        .catch(err => console.error('[DesignHub] JoinSession error:', err));
}

/// <summary>Broadcast an update to all clients in the session.</summary>
export function broadcastUpdate(sessionId, payload) {
    if (!hubConnection) return;
    hubConnection.invoke('BroadcastUpdate', sessionId, payload)
        .catch(err => console.error('[DesignHub] BroadcastUpdate error:', err));
}

/// <summary>Stop the hub connection and clean up. Call from Blazor IAsyncDisposable.</summary>
export function destroyDesignHub() {
    if (hubConnection) {
        hubConnection.stop().catch(() => {});
        hubConnection = null;
    }
}
