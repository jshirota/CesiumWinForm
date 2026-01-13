const params = new URLSearchParams(window.location.search);

Cesium.Ion.defaultAccessToken = params.get("token");

const viewer = new Cesium.Viewer("cesium", {
    terrain: Cesium.Terrain.fromWorldTerrain(),
    animation: params.get("animation") === "true",
    baseLayerPicker: params.get("baseLayerPicker") === "true",
    fullscreenButton: false,
    geocoder: params.get("geocoder") === "true",
    homeButton: false,
    navigationHelpButton: false,
    sceneModePicker: false,
    timeline: params.get("timeline") === "true",
});

function notify(id, data = null) {
    window.chrome.webview.postMessage({ Id: id, Data: data });
}

function getCamera() {
    const carto = Cesium.Cartographic.fromCartesian(viewer.camera.position);
    const { heading, pitch, roll } = viewer.camera;
    return {
        longitude: Cesium.Math.toDegrees(carto.longitude),
        latitude: Cesium.Math.toDegrees(carto.latitude),
        height: carto.height,
        heading: Cesium.Math.toDegrees(heading),
        pitch: Cesium.Math.toDegrees(pitch),
        roll: Cesium.Math.toDegrees(roll),
    };
}

const THROTTLE_MS = 100;
let lastCall = 0;

viewer.camera.changed.addEventListener(() => {
    const now = performance.now();
    if (now - lastCall >= THROTTLE_MS) {
        lastCall = now;
        notify("moved", getCamera());
    }
});

viewer.camera.moveEnd.addEventListener(() => notify("moved", getCamera()));

notify("moved", getCamera());
notify("ready");

function flyTo(longitude, latitude, height, heading, pitch, roll, duration) {
    return new Promise(resolve => {
        viewer.camera.flyTo({
            destination: Cesium.Cartesian3.fromDegrees(longitude, latitude, height),
            orientation: {
                heading: Cesium.Math.toRadians(heading),
                pitch: Cesium.Math.toRadians(pitch),
                roll: roll,
            },
            duration: duration,
            complete: () => resolve(),
        });
    });
}

const _czmls = new Map();

function addCzml(czml, flyToDuration) {
    return Cesium.CzmlDataSource.load(czml).then((dataSource) => {
        viewer.dataSources.add(dataSource);
        if (flyToDuration != null)
            viewer.flyTo(dataSource, { duration: flyToDuration });
        const key = ([..._czmls.keys()].pop() || 0) + 1;
        _czmls.set(key, dataSource);
        return key;
    });
}

function removeCzml(key = null) {
    key ??= [..._czmls.keys()].pop();
    if (_czmls.has(key)) {
        viewer.dataSources.remove(_czmls.get(key), true);
        _czmls.delete(key);
    }
    return Promise.resolve();
}

function add3DTileset(assetId, flyToDuration) {
    return Cesium.Cesium3DTileset.fromIonAssetId(assetId)
        .then(tileset => {
            viewer.scene.primitives.add(tileset);
            if (flyToDuration != null)
                viewer.flyTo(tileset, { duration: flyToDuration });
        });
}
