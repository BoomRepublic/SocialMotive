let map = null;
let markersLayer = null;
let hubConnection = null;
let fitBoundsDone = false;

export function initMap(elementId, initialLocations) {
    if (map) return;

    map = L.map(elementId).setView([52.37, 4.90], 7);

    L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
        maxZoom: 19
    }).addTo(map);

    markersLayer = L.layerGroup().addTo(map);

    // Render initial markers passed from server
    if (initialLocations && initialLocations.length > 0) {
        updateMarkers(initialLocations);
    }

    // Connect to SignalR hub from the browser for live updates
    startHubConnection();
}

function startHubConnection() {
    const signalR = window.signalR;
    if (!signalR) {
        console.warn('[LiveMap] signalR not loaded, live updates disabled');
        return;
    }

    hubConnection = new signalR.HubConnectionBuilder()
        .withUrl('/locationhub')
        .withAutomaticReconnect()
        .build();

    hubConnection.on('ReceiveLocations', (locations) => {
        updateMarkers(locations);
    });

    hubConnection.start().catch(err => console.error('[LiveMap] Hub connection error:', err));
}

function updateMarkers(locations) {
    if (!map || !markersLayer) return;

    markersLayer.clearLayers();

    const bounds = [];

    for (const loc of locations) {
        const latlng = [loc.latitude, loc.longitude];
        bounds.push(latlng);

        const popup =
            `<strong>${loc.displayName}</strong><br/>` +
            `${loc.latitude.toFixed(5)}, ${loc.longitude.toFixed(5)}<br/>` +
            `<small>${new Date(loc.timestamp).toLocaleTimeString()}</small><br/>` +
            (loc.speedKmh != null ? `Speed: ${loc.speedKmh.toFixed(1)} km/h<br/>` : '') +
            (loc.headingDegrees != null ? `Heading: ${loc.headingDegrees.toFixed(0)}°` : '');

        L.marker(latlng)
            .bindPopup(popup)
            .bindTooltip(loc.displayName, { permanent: false })
            .addTo(markersLayer);
    }

    // Only fit bounds on initial load, not on live updates
    if (!fitBoundsDone && bounds.length > 0) {
        map.fitBounds(bounds, { padding: [40, 40], maxZoom: 15 });
        fitBoundsDone = true;
    }
}

export function destroyMap() {
    if (hubConnection) {
        hubConnection.stop().catch(() => {});
        hubConnection = null;
    }
    if (map) {
        map.remove();
        map = null;
        markersLayer = null;
        fitBoundsDone = false;
    }
}
