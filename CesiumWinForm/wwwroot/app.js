const params = new URLSearchParams(window.location.search);

Cesium.Ion.defaultAccessToken = params.get("token");

const viewer = new Cesium.Viewer("cesium", {
    terrain: Cesium.Terrain.fromWorldTerrain(),
    timeline: false,
    animation: false,
    navigationHelpButton: false,
    fullscreenButton: false,
    sceneModePicker: false,
    infoBox: false,
    selectionIndicator: false,
});

function notify(name, data = null) {
    window.chrome.webview.postMessage(JSON.stringify({ name: name, data: data }));
}

notify("ready");
