let map = null;
let markers = [];
let infoWindow = null;

export function initMap(elementId, apiKey) {
    return new Promise((resolve) => {
        // Load Google Maps script dynamically
        const script = document.getElementById('google-maps-script');
        if (apiKey && !script.src) {
            script.src = `https://maps.googleapis.com/maps/api/js?key=${apiKey}`;
            script.onload = () => createMap(elementId, resolve);
        } else if (window.google && window.google.maps) {
            createMap(elementId, resolve);
        } else {
            // No API key — load without key (shows watermark)
            script.src = 'https://maps.googleapis.com/maps/api/js';
            script.onload = () => createMap(elementId, resolve);
        }
    });
}

function createMap(elementId, resolve) {
    const element = document.getElementById(elementId);
    map = new google.maps.Map(element, {
        center: { lat: 52.37, lng: 4.90 },
        zoom: 7,
        mapTypeControl: true,
        streetViewControl: false
    });
    infoWindow = new google.maps.InfoWindow();
    resolve();
}

export function updateMarkers(locations) {
    if (!map) return;

    // Clear existing markers
    for (const m of markers) {
        m.setMap(null);
    }
    markers = [];

    const bounds = new google.maps.LatLngBounds();

    for (const loc of locations) {
        const position = { lat: loc.latitude, lng: loc.longitude };
        bounds.extend(position);

        const marker = new google.maps.Marker({
            position: position,
            map: map,
            title: loc.displayName
        });

        const content =
            `<strong>${loc.displayName}</strong><br/>` +
            `${loc.latitude.toFixed(5)}, ${loc.longitude.toFixed(5)}<br/>` +
            `<small>${new Date(loc.timestamp).toLocaleTimeString()}</small><br/>` +
            (loc.speedKmh != null ? `Speed: ${loc.speedKmh.toFixed(1)} km/h<br/>` : '') +
            (loc.headingDegrees != null ? `Heading: ${loc.headingDegrees.toFixed(0)}&deg;` : '');

        marker.addListener('click', () => {
            infoWindow.setContent(content);
            infoWindow.open(map, marker);
        });

        markers.push(marker);
    }

    if (locations.length > 0) {
        map.fitBounds(bounds);
    }
}

export function destroyMap() {
    for (const m of markers) {
        m.setMap(null);
    }
    markers = [];
    map = null;
    infoWindow = null;
}
