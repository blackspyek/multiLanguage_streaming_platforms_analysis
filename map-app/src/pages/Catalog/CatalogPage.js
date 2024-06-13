import React, { useEffect, useState } from "react";
import classes from "./catalogPage.module.css";
import useAuth from "../../hooks/useAuth";
import Header from "../../components/Header/Header";
import useAxiosPrivate from "../../hooks/useAxiosPrivate";
import Pagination from "../../components/TitlesPagination/TitlesPagination";
import Slider from "rc-slider";
import "rc-slider/assets/index.css";
import { saveAs } from "file-saver";
import { faDownload } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { toast } from "react-toastify";
import axios from "../../api/axios";
import UpdatedDataInfo from "../../components/UpdatedDataInfo/UpdatedDataInfo";

export default function Catalog() {
  const { auth } = useAuth();
  const axiosPrivate = useAxiosPrivate();

  const [titles, setTitles] = useState([]);
  const [currentPage, setCurrentPage] = useState(1);
  const [totalItems, setTotalItems] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  const [totalPages, setTotalPages] = useState(0);

  const [yearRange, setYearRange] = useState([2000, 2024]);
  const [type, setType] = useState("");
  const [titleName, setTitleName] = useState("");
  const [genres, setGenres] = useState([]);
  const [selectedGenres, setSelectedGenres] = useState([]);
  const [sort, setSort] = useState("asc");
  const [sortBy, setSortBy] = useState("");
  const [lastDataUpdate, setLastDataUpdate] = useState("");
  const [showUpdate, setShowUpdate] = useState(false);

  const handleYearRangeChange = (newRange) => {
    setYearRange(newRange);
  };

  useEffect(() => {
    const UPDATE_DATE_URL = `http://localhost:5192/api/titles/lastmod`;

    const fetchDataUpdateDate = async () => {
      try {
        if (lastDataUpdate === "") {
          return;
        }
        const lastmod = await axios.get(UPDATE_DATE_URL);
        if (lastDataUpdate !== lastmod.data) {
          setShowUpdate(true);
          console.log(`Data updated at: ${lastmod.data} ${lastDataUpdate}`);
        }
      } catch (error) {
        console.error("Error fetching data update date:", error);
      }
    };

    fetchDataUpdateDate();
    const intervalId = setInterval(fetchDataUpdateDate, 20000);

    return () => clearInterval(intervalId);
  }, []);

  useEffect(() => {
    fetchGenres();
    fetchTitles();
  }, [currentPage, sortBy, sort]);

  const downloadTitles = async () => {
    if (titles.length === 0) {
      toast.error("No titles to download");
      return;
    }
    const ALL_TITLES_URL = `paginate/download`;
    try {
      const response = await axiosPrivate.get(ALL_TITLES_URL, {
        params: {
          startYear: yearRange[0],
          endYear: yearRange[1],
          sortBy: sortBy,
          sort: sort,
          type: type,
          genres: selectedGenres.join(","),
        },
        responseType: "blob",
      });
      const blob = new Blob([response.data], {
        type: "application/json",
      });
      saveAs(blob, "titles.json");
      toast.success("Downloaded titles.json");
    } catch (error) {
      console.error(error);
    }
  };

  const downloadTitlesXML = async () => {
    if (titles.length === 0) {
      toast.error("No titles to download");
      return;
    }
    const ALL_TITLES_URL = `paginate/download/xml`;
    try {
      const response = await axiosPrivate.get(ALL_TITLES_URL, {
        params: {
          startYear: yearRange[0],
          endYear: yearRange[1],
          sortBy: sortBy,
          sort: sort,
          type: type,
          genres: selectedGenres.join(","),
        },
        responseType: "blob",
      });
      const blob = new Blob([response.data], {
        type: "application/xml",
      });
      saveAs(blob, "titles.xml");
      toast.success("Downloaded titles.xml");
    } catch (error) {
      console.error(error);
    }
  };
  const fetchGenres = async () => {
    const controller = new AbortController();
    const GENRES_URL = `paginate/categories`;
    try {
      const response = await axiosPrivate.get(GENRES_URL, {
        signal: controller.signal,
      });
      setGenres(response.data);
    } catch (error) {
      console.error(error);
    }
  };
  const fetchTitles = async () => {
    const controller = new AbortController();
    const ALL_TITLES_URL = `paginate/all`;
    const UPDATE_DATE_URL = `http://localhost:5192/api/titles/lastmod`;
    try {
      const response = await axiosPrivate.get(ALL_TITLES_URL, {
        params: {
          pageNumber: currentPage,
          pageSize: pageSize,
          startYear: yearRange[0],
          endYear: yearRange[1],
          sortBy: sortBy,
          sort: sort,
          type: type,
          genres: selectedGenres.join(","),
        },
        signal: controller.signal,
      });

      const lastmod = await axios.get(UPDATE_DATE_URL);
      setLastDataUpdate(lastmod.data);
      setTitles(response.data.items);
      setTotalItems(response.data.totalElements);
      setTotalPages(response.data.totalPages);
    } catch (error) {
      console.error(error);
    }
  };
  const handlePageChange = (page) => {
    setCurrentPage(page);
  };
  return (
    <div className={classes.container}>
      <Header />

      {auth && titles && (
        <div className={classes.content}>
          <h1 className={classes.title}>Catalog</h1>
          <div className={classes.filter}>
            <div className={classes.content}>
              <div className={classes.yearSelection}>
                <Slider
                  range
                  min={1800}
                  max={2024}
                  value={yearRange}
                  onChange={handleYearRangeChange}
                  step={1}
                />
                <p className={classes.yearRange}>
                  Selected range: {yearRange[0]} - {yearRange[1]}
                </p>
              </div>
            </div>
            <div className={classes.typeSelection}>
              <select
                className={classes.typeSelect}
                onChange={(e) => {
                  setType(e.target.value);
                }}
              >
                <option value="">Select Type</option>
                <option value="Movie">Movie</option>
                <option value="TV Show">TV Show</option>
              </select>
            </div>
            <div className={classes.genreSelection}>
              {genres.map((genre) => (
                <label
                  className={classes.genre}
                  key={genre.id}
                  htmlFor={genre.id}
                >
                  <input
                    id={genre.id}
                    type="checkbox"
                    value={genre.id}
                    checked={selectedGenres.includes(genre.id)}
                    className={classes.genreCheckbox}
                    onChange={(e) => {
                      setSelectedGenres((prevSelectedGenres) =>
                        e.target.checked
                          ? [...prevSelectedGenres, genre.id]
                          : prevSelectedGenres.filter((id) => id !== genre.id)
                      );
                    }}
                  />
                  <span
                    className={
                      selectedGenres.includes(genre.id) ? classes.selected : ""
                    }
                  >
                    {genre.name}
                  </span>
                </label>
              ))}
            </div>

            {sortBy && (
              <p
                onClick={() => {
                  setSortBy("");
                  setSort("asc");
                }}
                className={classes.sortedBy}
              >
                Sorted By: {sortBy} {sort}
              </p>
            )}
            <button className={classes.filterButton} onClick={fetchTitles}>
              Filter
            </button>
          </div>
          <div className={classes.tableHandle}>
            <div className={classes.downloadButton}>
              <button
                onClick={downloadTitles}
                className={classes.downloadButtons}
              >
                <FontAwesomeIcon icon={faDownload} />
                <span> Download JSON</span>
              </button>
              <button
                onClick={downloadTitlesXML}
                className={classes.downloadButtons}
              >
                <FontAwesomeIcon icon={faDownload} />
                <span> Download XML</span>
              </button>
            </div>
            <UpdatedDataInfo dataUpdated={showUpdate} />
            <table>
              <thead>
                <tr>
                  <th>Type</th>
                  <th>Platform(s)</th>
                  <th>Genre(s)</th>
                  <th>Title</th>
                  <th>Year</th>
                  <th
                    className={classes.sortable}
                    onClick={() => {
                      setSortBy("imdb_rating");
                      setSort(sort === "asc" ? "desc" : "asc");
                    }}
                  >
                    IMDB Rating
                  </th>
                  <th
                    className={classes.sortable}
                    onClick={() => {
                      setSortBy("tomatometer");
                      setSort(sort === "asc" ? "desc" : "asc");
                    }}
                  >
                    TomatoMeter Rating
                  </th>
                  <th
                    className={classes.sortable}
                    onClick={() => {
                      setSortBy("audience_rating");
                      setSort(sort === "asc" ? "desc" : "asc");
                    }}
                  >
                    Audience Rating
                  </th>
                  <th
                    className={classes.sortable}
                    onClick={() => {
                      setSortBy("over_all");
                      setSort(sort === "asc" ? "desc" : "asc");
                    }}
                  >
                    Over All
                  </th>
                </tr>
              </thead>
              <tbody>
                {titles.map((title) => (
                  <tr key={title.id}>
                    <td>{title.type}</td>
                    <td>
                      {title.titlePlatform.length > 0
                        ? title.titlePlatform
                            .map((platform) => platform.platform.name)
                            .join(", ")
                        : ""}
                    </td>
                    <td>
                      {title.titleCategory.length > 0
                        ? title.titleCategory
                            .map((category) => category.category.name)
                            .join(", ")
                        : ""}
                    </td>
                    <td>{title.titleName}</td>
                    <td>{title.release_Year}</td>
                    <td>{title.imdb_rating}</td>
                    <td>{title.tomatometer}</td>
                    <td>{title.audience_rating}</td>
                    <td>{title.over_all}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
          <Pagination
            currentPage={currentPage}
            totalPages={totalPages}
            onPageChange={handlePageChange}
          />
        </div>
      )}
    </div>
  );
}
