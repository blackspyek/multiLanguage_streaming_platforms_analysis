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

  const [countryRatings, setCountryRatings] = useState({});

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
      {countryRatings.length > 0 && // Conditional rendering check
        markers
          .filter(
            (
              { country } // Filter markers with ratings
            ) =>
              countryRatings.some(
                (item) => item.country.toLowerCase() === country.toLowerCase()
              )
          )
          .map(({ country, latitude, longitude }, index) => (
            <Marker
              key={index}
              coordinates={[longitude, latitude]}
              onMouseEnter={() => setHoveredMarker(index)}
              onMouseLeave={() => setHoveredMarker(null)}
            >
              <circle r={2} fill={"red"} />

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
                  {country}:{" "}
                  {countryRatings
                    .find(
                      (item) =>
                        item.country.toLowerCase() === country.toLowerCase()
                    )
                    ?.rating.toFixed(2)}
                </text>
              )}
            </Marker>
          ))}
    </ComposableMap>
  );
};

export default MapChart;
