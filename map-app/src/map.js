import React, { useState } from "react";
import {
  ComposableMap,
  Geographies,
  Geography,
  Marker,
} from "react-simple-maps";

import geoUrl from "./map.json";
// from map.json file i want to extract country name and its coordinates
import markers from "./points.json";

const MapChart = () => {
  const [hoveredMarker, setHoveredMarker] = useState(null);

  return (
    <ComposableMap>
      <Geographies geography={geoUrl}>
        {({ geographies }) =>
          geographies.map((geo, index) => (
            <Geography
              key={index}
              geography={geo}
              onClick={() => {}}
              style={{
                default: { outline: "none" },
                hover: { outline: "none" },
                pressed: { outline: "none" },
              }}
            />
          ))
        }
      </Geographies>
      {markers.map(({ country, latitude, longitude }, index) => (
        <Marker
          key={index}
          coordinates={[longitude, latitude]}
          onMouseEnter={() => setHoveredMarker(index)}
          onMouseLeave={() => setHoveredMarker(null)}
        >
          <circle r={2} fill="#F00" stroke="#fff" strokeWidth={2} />
          {hoveredMarker === index && (
            <text
              textAnchor="middle"
              y={10}
              style={{
                fontFamily: "system-ui",
                fill: "#FF0000",
                fontSize: 12,
                fontWeight: "bold",
              }}
            >
              {country}: {Math.round(Math.random() * 10 * 10) / 10}
            </text>
          )}
        </Marker>
      ))}
    </ComposableMap>
  );
};

export default MapChart;
