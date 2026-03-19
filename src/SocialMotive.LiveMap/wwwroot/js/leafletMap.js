let map = null;
let markersLayer = null;

export function initMap(elementId) {
    if (map) return;

    map = L.map(elementId).setView([52.37, 4.90], 7);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
        maxZoom: 19
    }).addTo(map);

    markersLayer = L.layerGroup().addTo(map);
}

export function updateMarkers(locations) {
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

    if (bounds.length > 0) {
        map.fitBounds(bounds, { padding: [40, 40], maxZoom: 15 });
    }
}

export function destroyMap() {
    if (map) {
        map.remove();
        map = null;
        markersLayer = null;
    }
}
