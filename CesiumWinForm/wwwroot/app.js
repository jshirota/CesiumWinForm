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

imageryAssetId = params.get("imageryAssetId");

async function loadImageryProvider() {
    viewer.imageryLayers.removeAll();
    viewer.imageryLayers.addImageryProvider(
        await Cesium.IonImageryProvider.fromAssetId(imageryAssetId)
    );
}

loadImageryProvider();

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

const dataSources = new Map();
let dataSourceKey = 0;

function addDataSource(dataSource) {
    viewer.dataSources.add(dataSource);
    dataSourceKey++;
    dataSources.set(dataSourceKey, dataSource);
    return dataSourceKey;
}

function getDataSource(id) {
    return dataSources.get(id);
}

function deleteDataSource(id) {
    const dataSource = dataSources.get(id);
    if (dataSource != null) {
        viewer.dataSources.remove(dataSource, true);
        dataSources.delete(id);
    }
}

function addCzml(czml, flyToDuration) {
    return Cesium.CzmlDataSource.load(czml).then((dataSource) => {
        if (flyToDuration != null) {
            viewer.flyTo(dataSource, { duration: flyToDuration });
        }
        return addDataSource(dataSource);
    });
}

function addGeoJson(geojson, flyToDuration, cameraHeight, markerUrl, markerWidth, markerHeight, anchorBottom, stroke, strokeWidth, strokeAlpha, fill, fillAlpha) {
    return Cesium.GeoJsonDataSource.load(geojson, {
        clampToGround: false,
        stroke: Cesium.Color.fromCssColorString(stroke).withAlpha(strokeAlpha),
        strokeWidth,
        fill: Cesium.Color.fromCssColorString(fill).withAlpha(fillAlpha),
    }).then((dataSource) => {
        if (flyToDuration != null) {
            viewer.flyTo(dataSource, {
                duration: flyToDuration,
                offset: cameraHeight == null ? null : new Cesium.HeadingPitchRange(0, -Cesium.Math.PI_OVER_FOUR, cameraHeight)
            });
        }
        dataSource.entities.values.forEach(e => {
            if (e.position) {
                if (markerUrl == null) {
                    const pinBuilder = new Cesium.PinBuilder();
                    markerUrl = pinBuilder.fromColor(Cesium.Color.WHITE, 32).toDataURL();
                }
                e.billboard = {
                    image: markerUrl,
                    width: markerWidth,
                    height: markerHeight,
                    verticalOrigin: anchorBottom ? Cesium.VerticalOrigin.BOTTOM : Cesium.VerticalOrigin.CENTER
                };
            }
        });
        return addDataSource(dataSource);
    });
}

function remove(key) {
    deleteDataSource(key);
    return Promise.resolve();
}

function removeAll() {
    viewer.dataSources.removeAll();
    dataSources.clear();
    return Promise.resolve();
}

function add3DTileset(assetId, flyToDuration) {
    return Cesium.Cesium3DTileset.fromIonAssetId(assetId)
        .then(tileset => {
            viewer.scene.primitives.add(tileset);
            if (flyToDuration != null) {
                viewer.flyTo(tileset, { duration: flyToDuration }); t
            }
        });
}
