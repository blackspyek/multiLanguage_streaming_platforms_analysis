import React, { useEffect, useRef } from "react";
import classes from "./uploadstreaming.module.css";
import Header from "../../components/Header/Header";
import { useState } from "react";
import axios from "axios";
import { toast } from "react-toastify";
import { HubConnectionBuilder } from "@microsoft/signalr";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faTrash,
  faUpload,
  faMinusSquare,
} from "@fortawesome/free-solid-svg-icons";
import CategoryModal from "../../components/CategoryModal/CategoryModal";
import PlatformModal from "../../components/PlatformModal/PlatformModal";
import { axiosPrivate } from "../../api/axios";
export default function UploadStreaming() {
  const [file, setFile] = useState(null);
  const [progress, setProgress] = useState({ started: false, pc: 0 });
  const [msg, setMsg] = useState(null);
  const [remainingRows, setRemainingRows] = useState(0);
  const [rowsNumber, setRowsNumber] = useState(null);

  const [platforms, setPlatforms] = useState([]);
  const [displayPlatformInput, setdisplayPlatformInput] = useState(true);
  const [displayPlatformInputFile, setdisplayPlatformInputFile] =
    useState(true);
  const [displayCategoryInput, setdisplayCategoryInput] = useState(true);

  const [platformName, setPlatformName] = useState("");
  const [platformNameFile, setPlatformNameFile] = useState("");
  const [categoryName, setCategoryName] = useState("");

  const [type, setType] = useState("Movie");
  const [titleName, setTitleName] = useState("");
  const [country, setCountry] = useState("");
  const [release_Year, setRelease_Year] = useState(0);

  const [showCategoryModal, setShowCategoryModal] = useState(false);
  const [showPlatformModal, setShowPlatformModal] = useState(false);
  const [selectedCategories, setSelectedCategories] = useState("");
  const [selectedPlatforms, setSelectedPlatforms] = useState("");

  const fetchPlatforms = async () => {
    const controller = new AbortController();
    const ALL_PLATFORMS_URL = `streaming/allPlatforms`;

    try {
      const response = await axiosPrivate.get(ALL_PLATFORMS_URL, {
        signal: controller.signal,
      });
      setPlatforms(response.data);
    } catch (error) {
      console.error(error);
    }
  };
  useEffect(() => {
    try {
      fetchPlatforms();
    } catch (err) {
      console.log(err);
    }
  }, []);
  useEffect(() => {
    if (rowsNumber === null) {
      if (remainingRows > 0) {
        setRowsNumber(remainingRows);
      }
    }
    if (rowsNumber && remainingRows) {
      setProgress({
        started: true,
        pc: ((rowsNumber - remainingRows) / rowsNumber) * 100,
      });
    }
  }, [remainingRows, rowsNumber]);
  const sendOne = async (e) => {
    e.preventDefault();
    const data = {
      platformNames: selectedPlatforms,
      categoryNames: selectedCategories,
      type: type,
      titleName: titleName,
      countryNames: country,
      release_Year: release_Year,
    };
    console.log(data);
    try {
      await axios.post("http://localhost:5192/api/titles/titleapi", data);
      toast.success("Record added successfully");
    } catch (err) {
      if (err.response.status === 422) {
        toast.error("Record already exists");
      } else if (err.response.status === 400) {
        toast.error("Please fill all the fields");
      } else {
        toast.error("Record addition failed");
      }
    }
  };
  const handleUpload = async (e) => {
    e.preventDefault();

    if (!file) return;

    const fd = new FormData();
    fd.append("file", file);
    const connection = new HubConnectionBuilder()
      .withUrl("http://localhost:5192/progressHub")
      .withAutomaticReconnect()
      .build();
    connection.on("ProgressUpdate", (remainingRows) => {
      setRemainingRows(remainingRows);
    });
    setMsg("Uploading...");
    setProgress((prevState) => ({ ...prevState, started: true }));
    try {
      connection.start().then(() => {
        axios
          .post(
            `http://localhost:5192/api/titles/upload?platformName=${platformNameFile}`,
            fd
          )
          .then((res) => {
            setProgress({ started: false, pc: 100 });
            setMsg(null);
            toast.success("File uploaded successfully");
          });
      });
    } catch (err) {
      setMsg(err);
      toast.error("File upload failed");
    }
  };
  return (
    <div className={classes.container}>
      <Header />
      <h1 className={classes.title}>Upload Streaming Titles</h1>

      <div className={classes.innerContainer}>
        <div className={classes.form_wrapper}>
          <form className={classes.titleForm}>
            <div>
              <button
                className={classes.modalBtn}
                onClick={(e) => {
                  e.preventDefault();
                  setShowPlatformModal(true);
                }}
              >
                Select Platform
              </button>
              <PlatformModal
                isOpen={showPlatformModal}
                onClose={() => setShowPlatformModal(false)}
                onCategorySelect={(platforms) => {
                  setSelectedPlatforms(platforms);
                  setShowPlatformModal(false);
                }}
              />
              {selectedPlatforms && (
                <p className={classes.selection}>
                  Selected: {selectedPlatforms}
                </p>
              )}
              <button
                className={classes.modalBtn}
                onClick={(e) => {
                  e.preventDefault();
                  setShowCategoryModal(true);
                }}
              >
                Select Category
              </button>
              <CategoryModal
                isOpen={showCategoryModal}
                onClose={() => setShowCategoryModal(false)}
                onCategorySelect={(categoryies) => {
                  setSelectedCategories(categoryies);
                  setShowCategoryModal(false);
                }}
              />
              {selectedCategories && (
                <p className={classes.selection}>
                  Selected: {selectedCategories}
                </p>
              )}
              <select
                name="type"
                id="type"
                className={classes.selectPlatform}
                onChange={(e) => {
                  setType(e.target.value);
                }}
              >
                <option value="Movie">Movie</option>

                <option value="TV Show">TV Show</option>
              </select>
            </div>
            <div>
              <input
                onChange={(e) => {
                  setTitleName(e.target.value);
                }}
                className={classes.platformInput}
                type="text"
                placeholder="Title"
              />
              <input
                onChange={(e) => {
                  setCountry(e.target.value);
                }}
                className={classes.platformInput}
                type="text"
                placeholder="Country"
              />
              <input
                onChange={(e) => {
                  setRelease_Year(e.target.value);
                }}
                className={classes.platformInput}
                type="number"
                placeholder="Release Year"
              />
              <button
                className={classes.submit}
                onClick={sendOne}
                disabled={
                  selectedCategories &&
                  selectedPlatforms &&
                  type &&
                  titleName &&
                  country &&
                  release_Year
                    ? ""
                    : "true"
                }
              >
                Add Record
              </button>
            </div>
          </form>
        </div>
        <div className={classes.form_wrapper}>
          <form>
            <select
              name="platform"
              id="platform"
              className={classes.selectPlatform}
              onChange={(e) => {
                setPlatformNameFile(e.target.value);
                e.target.value === ""
                  ? setdisplayPlatformInputFile(true)
                  : setdisplayPlatformInputFile(false);
              }}
            >
              <option value="">Select Platform Or Enter Below</option>
              {platforms.map((platform) => (
                <option key={platform.id} value={platform.name}>
                  {platform.name}
                </option>
              ))}
            </select>
            <input
              className={`${classes.platformInput} ${
                displayPlatformInputFile ? "" : classes.platformInputHidden
              }`}
              type="text"
              placeholder="Enter Platform"
              onChange={(e) => {
                setPlatformNameFile(e.target.value);
              }}
            />
            <label className={classes.custom_file} htmlFor="file">
              <span className={classes.file}>File</span>
              {file ? file.name : <FontAwesomeIcon icon={faUpload} />}
            </label>
            <input
              onChange={(e) => {
                setFile(e.target.files[0]);
              }}
              type="file"
              id="file"
            />
            <button
              className={classes.submit}
              disabled={!file ? "true" : ""}
              onClick={handleUpload}
            >
              Upload
            </button>
          </form>
          {progress.started && (
            <span className={classes.totalText}>Total Rows:{rowsNumber}</span>
          )}
          {progress.started && (
            <progress value={progress.pc} max={100}></progress>
          )}
          {progress.started && (
            <span className={classes.remainingText}>
              remainingRows: {remainingRows}
            </span>
          )}
          {msg && <span>{msg}</span>}
        </div>
      </div>
    </div>
  );
}
