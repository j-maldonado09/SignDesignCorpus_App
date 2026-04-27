import FeatureLayer from "https://js.arcgis.com/5.0/@arcgis/core/layers/FeatureLayer.js";

console.log("ArcGIS modules loaded");

const mapElement = document.querySelector("#map");

// Wait for the map view to be ready
mapElement.addEventListener("arcgisViewReadyChange", () => {
    const view = mapElement.view;

    console.log("Map view ready:", view);

    // Get the FeatureLayer created by the Web Component
    const layer = view.map.findLayerById("trailheads");

    console.log("Layers in map:", view.map.layers.toArray());

    if (layer) {
        layer.when(() => {
            console.log("FeatureLayer loaded:", layer);
            view.goTo(layer.fullExtent).catch(() => { });
        });
    } else {
        console.error("FeatureLayer NOT found in map");
    }
});




