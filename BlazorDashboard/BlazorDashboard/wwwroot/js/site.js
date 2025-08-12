window.initLeafletMap = (mapId, tileUrlTemplate, minZoom, maxZoom, centerLat, centerLon) => {
    var map = L.map(mapId).setView([centerLat, centerLon], minZoom);

    L.tileLayer(tileUrlTemplate, {
        minZoom: minZoom,
        maxZoom: maxZoom,
        tileSize: 256,
        crossOrigin: true
    }).addTo(map);
};
