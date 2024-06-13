import React, { useEffect, useState } from "react";
import {
  ComposableMap,
  Geographies,
  Geography,
  Marker,
} from "react-simple-maps";

import geoUrl from "./map.json";
// from map.json file i want to extract country name and its coordinates
import markers from "./points.json";
import { axiosPrivate } from "./api/axios";
const COUNTRIES_AVG_URL = "/map/all";

const MapChart = () => {
  const [hoveredMarker, setHoveredMarker] = useState(null);

  const [countryRatings, setCountryRatings] = useState([]);
  // const [countryRatings, setCountryRatings] = useState({});

  const [minRating, setMinRating] = useState(0);
  const [maxRating, setMaxRating] = useState(0);

  useEffect(() => {
    let isMounted = true;
    const controller = new AbortController();

    const getRottenYears = async () => {
      try {
        const response = await axiosPrivate.get(COUNTRIES_AVG_URL, {
          signal: controller.signal,
        });
        if (isMounted) {
          var data = response.data;
          const countryRatings = [];
          for (const [country, rating] of Object.entries(data)) {
            countryRatings.push({ country, rating });
          }
          const ratings = countryRatings.map((item) => item.rating);
          setMinRating(Math.min(...ratings));
          setMaxRating(Math.max(...ratings));
          setCountryRatings(countryRatings);
        }
      } catch (err) {
        console.log(err);
      }
    };

    getRottenYears();
    return () => {
      isMounted = false;
      controller.abort();
    };
  }, [axiosPrivate]);

  const getColorForRating = (rating) => {
    const green = [0, 255, 0];
    const yellow = [255, 255, 0];
    const orange = [255, 165, 0];
    const red = [255, 0, 0];

    if (rating === undefined) return "rgb(255, 255, 255)";

    const scale = (rating - minRating) / (maxRating - minRating);
    if (scale <= 0.25) {
      // Interpolate between red and orange
      return interpolateColor(red, orange, scale / 0.25);
    } else if (scale <= 0.5) {
      // Interpolate between orange and yellow
      return interpolateColor(orange, yellow, (scale - 0.25) / 0.25);
    } else if (scale <= 0.75) {
      // Interpolate between yellow and green
      return interpolateColor(yellow, green, (scale - 0.5) / 0.25);
    } else {
      // Use green
      return `rgb(${green[0]},${green[1]},${green[2]})`;
    }
  };

  const interpolateColor = (color1, color2, factor) => {
    const result = color1.map((c1, i) => c1 + factor * (color2[i] - c1));
    return `rgb(${result[0]},${result[1]},${result[2]})`;
  };


  return (
    <ComposableMap
      projectionConfig={{
        scale: 160,
      }}
    >
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
      {countryRatings.length > 0 &&
        markers
          .filter(({ country }) =>
            countryRatings.some(
              (item) => item.country.toLowerCase() === country.toLowerCase()
            )
          )
          .map(({ country, latitude, longitude }, index) => {
            const rating = countryRatings.find(
              (item) => item.country.toLowerCase() === country.toLowerCase()
            )?.rating;
            return (
              <Marker
                key={index}
                coordinates={[longitude, latitude]}
                onMouseEnter={() => setHoveredMarker(index)}
                onMouseLeave={() => setHoveredMarker(null)}
              >
                <circle r={2} fill={getColorForRating(rating)} />

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
                    {country}: {rating?.toFixed(2)}
                  </text>
                )}
              </Marker>
            );
          })}
    </ComposableMap>
  );
};

export default MapChart;