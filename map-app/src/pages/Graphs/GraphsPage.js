import React, { useEffect, useState } from "react";
import useAuth from "../../hooks/useAuth";
import classes from "./graphsPage.module.css";
import Header from "../../components/Header/Header";
import {
  Chart as ChartJS,
  ArcElement,
  Tooltip,
  Legend,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  RadialLinearScale,
} from "chart.js";
import { Chart, Line, PolarArea } from "react-chartjs-2";
import useAxiosPrivate from "../../hooks/useAxiosPrivate";
import Slider from "rc-slider";
import "rc-slider/assets/index.css";

const AVG_YEAR_URL_IMDB = "/imdb/all";
const AVG_YEAR_URL_ROTTEN = "/rotten/all";

function Graphs() {
  const { auth } = useAuth();
  const axiosPrivate = useAxiosPrivate();
  const [years, setYears] = useState([]);
  const [yearsAvg, setAvg] = useState([]);
  const [rottenYears, setRottenYears] = useState([]);
  const [rottenAudienceRating, setRottenAudienceRating] = useState([]);
  const [rottenTomatometerRating, setRottenTomatometerRating] = useState([]);
  const [yearRange, setYearRange] = useState([2000, 2024]);

  const [BestMovieDirectors, setBestMovieDirectors] = useState([]);
  const [BestRatedMovies, setBestRatedMovies] = useState([]);
  const [type, setType] = useState("movie");

  ChartJS.register(
    ArcElement,
    Tooltip,
    Legend,
    CategoryScale,
    LinearScale,
    PointElement,
    LineElement,
    RadialLinearScale
  );

  useEffect(() => {
    let isMounted = true;
    const controller = new AbortController();

    const getRottenYears = async () => {
      try {
        const response = await axiosPrivate.get(AVG_YEAR_URL_ROTTEN, {
          signal: controller.signal,
        });
        if (isMounted) {
          var data = response.data;
          const allYears = new Set([
            ...years,
            ...data.map((item) => item.year),
          ]);
          const yearsArray = Array.from(allYears).sort((a, b) => a - b);
          const rottenAudienceRating = yearsArray.map((year) => {
            const rating = data.find((item) => item.release_year === year);
            return rating ? rating.avg_audience_rating : null;
          });
          const rottenTomatometerRating = yearsArray.map((year) => {
            const rating = data.find((item) => item.release_year === year);
            return rating ? rating.avg_tomatometer_rating : null;
          });
          setRottenAudienceRating(rottenAudienceRating);
          setRottenTomatometerRating(rottenTomatometerRating);
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
  }, [axiosPrivate, years]);
  useEffect(() => {
    let isMounted = true;
    const controller = new AbortController();

    const getBestMovieDirectors = async () => {
      try {
        const response = await axiosPrivate.get("/directors/all", {
          signal: controller.signal,
          params: {
            type,
          },
        });
        if (isMounted) {
          setBestMovieDirectors(response.data);
        }
      } catch (err) {
        console.log(err);
      }
    };

    getBestMovieDirectors();

    return () => {
      isMounted = false;
      controller.abort();
    };
  }, [axiosPrivate, type]);

  useEffect(() => {
    let isMounted = true;
    const controller = new AbortController();

    const getBestRatedMovies = async () => {
      try {
        const response = await axiosPrivate.get("/bestRated/best", {
          signal: controller.signal,
          // params: {
          //   best,
          // },
        });
        if (isMounted) {
          setBestRatedMovies(response.data);
        }
      } catch (err) {
        console.log(err);
      }
    };

    getBestRatedMovies();


    return () => {
      isMounted = false;
      controller.abort();
    };
  }, [axiosPrivate]);

  useEffect(() => {
    let isMounted = true;
    const controller = new AbortController();

    const getYears = async () => {
      try {
        const response = await axiosPrivate.get(AVG_YEAR_URL_IMDB, {
          signal: controller.signal,
        });
        if (isMounted) {
          var data = response.data;
          data.sort((a, b) => a.year - b.year);
          var testYears = data.map((item) => item.year);
          var testRating = data.map((item) => item.avg);

          setYears(testYears);
          setAvg(testRating);
        }
      } catch (err) {
        console.log(err);
      }
    };

    getYears();

    return () => {
      isMounted = false;
      controller.abort();
    };
  }, [axiosPrivate]);

  const handleYearRangeChange = (newRange) => {
    setYearRange(newRange);
  };

  const filteredYears = years.filter(
    (year) => year >= yearRange[0] && year <= yearRange[1]
  );
  const filteredYearsAvg = yearsAvg.slice(
    years.indexOf(filteredYears[0]),
    years.indexOf(filteredYears[filteredYears.length - 1]) + 1
  );
  const filteredRottenAudienceRating = rottenAudienceRating.slice(
    years.indexOf(filteredYears[0]),
    years.indexOf(filteredYears[filteredYears.length - 1]) + 1
  );
  const filteredRottenTomatometerRating = rottenTomatometerRating.slice(
    years.indexOf(filteredYears[0]),
    years.indexOf(filteredYears[filteredYears.length - 1]) + 1
  );

  return (
    <div className={classes.container}>
      <Header />
      <div className={classes.graphsContainer}>
        <h1>Ratings by year</h1>
        <div className={classes.yearSelection}>
          <Slider
            range
            min={Math.min(...years)}
            max={Math.max(...years) - 1}
            value={yearRange}
            onChange={handleYearRangeChange}
            step={1}
          />
          <p className={classes.yearRange}>
            Selected range: {yearRange[0]} - {yearRange[1]}
          </p>
        </div>
        <div className={classes.graphs}>
          <div className={classes.graph}>
            <Line
              datasetIdKey="id"
              options={{
                maintainAspectRatio: false,
                radius: 1,
                hoverRadius: 10,
              }}
              data={{
                labels: filteredYears,
                datasets: [
                  {
                    id: 1,
                    label: "IMDB",
                    data: filteredYearsAvg,
                    borderColor: "rgb(53, 162, 235)",
                    backgroundColor: "rgba(53, 162, 235, 0.5)",
                  },
                  {
                    id: 2,
                    label: "Rotten Tomatoes Audience",
                    data: filteredRottenAudienceRating,
                    borderColor: "rgb(255, 99, 132)",
                    backgroundColor: "rgba(255, 99, 132, 0.5)",
                  },
                  {
                    id: 3,
                    label: "Rotten Tomatoes Tomatometer",
                    data: filteredRottenTomatometerRating,
                    borderColor: "rgb(75, 192, 192)",
                    backgroundColor: "rgba(75, 192, 192, 0.5)",
                  },
                ],
              }}
            />
          </div>
          <div>
            <h1>Best directors</h1>
            <select
              value={type}
              onChange={(e) => setType(e.target.value)}
              className={classes.select}
            >
              <option value="movie">Movie</option>
              <option value="tvSeries">TV Show</option>
            </select>
            <div>
              <PolarArea
                datasetIdKey="bestDirectors"
                options={{
                  maintainAspectRatio: false,
                  radius: 1,
                  hoverRadius: 10,
                }}
                data={{
                  labels: BestMovieDirectors.map(
                    (director) => director.director_name
                  ),
                  datasets: [
                    {
                      id: 1,
                      label: "Weighted Average",
                      data: BestMovieDirectors.map(
                        (director) => director.weighted_average
                      ),
                      backgroundColor: [
                        "rgba(255, 99, 132, 0.5)",
                        "rgba(54, 162, 235, 0.5)",
                        "rgba(255, 206, 86, 0.5)",
                        "rgba(75, 192, 192, 0.5)",
                        "rgba(153, 102, 255, 0.5)",
                      ],
                    },
                  ],
                }}
              />
            </div>
          </div>
          <div>
            <h1>Best Rated Movies</h1>
            {/* <select
              value={type}
              onChange={(e) => setType(e.target.value)}
              className={classes.select}
            >
              { <option value="movie">Movie</option>
              <option value="tvSeries">TV Show</option> }
            </select> */}
            <div>
              <PolarArea
                datasetIdKey="bestRatedMovies"
                options={{
                  maintainAspectRatio: false,
                  radius: 1,
                  hoverRadius: 20,
                }}
                data={{
                  labels: BestRatedMovies.map(
                    (movie) => movie.movie_name
                  ),
                  datasets: [
                    {
                      id: 1,
                      label: "Average Rating",
                      data: BestRatedMovies.map(
                        (movie) => movie.average_rating
                      ),
                      backgroundColor: [
                        "rgba(255, 0, 0, 1)",
                        "rgba(255, 0, 51, 1)",
                        "rgba(255, 51, 0, 1)",
                        "rgba(255, 51, 51, 1)",
                        "rgba(255, 102, 0, 1)",
                        "rgba(255, 102, 15, 1)",
                        "rgba(255, 153, 0, 1)",
                        "rgba(255, 153, 15, 1)",
                        "rgba(255, 204, 0, 1)",
                        "rgba(255, 204, 51, 1)",
                      ],
                    },
                  ],
                }}
              />
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default Graphs;
