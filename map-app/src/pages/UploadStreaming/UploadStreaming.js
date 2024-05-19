import React, { useEffect } from "react";
import classes from "./uploadstreaming.module.css";
import Header from "../../components/Header/Header";
import { useState } from "react";
import axios from "axios";
import { toast } from "react-toastify";
import { HubConnectionBuilder } from "@microsoft/signalr";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faUpload } from "@fortawesome/free-solid-svg-icons";
export default function UploadStreaming() {
  const [file, setFile] = useState(null);
  const [progress, setProgress] = useState({ started: false, pc: 0 });
  const [msg, setMsg] = useState(null);
  const [remainingRows, setRemainingRows] = useState(0);
  const [rowsNumber, setRowsNumber] = useState(null);

  useEffect(() => {
    console.log("progress", progress.pc);
    console.log(
      "Percentage:",
      ((rowsNumber - remainingRows) / rowsNumber) * 100
    );
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
            "http://localhost:5192/api/titles/upload?platformName=Netflix",
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
      <div className={classes.form_wrapper}>
        <h1>Upload Streaming Titles</h1>
        <form>
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
          <button disabled={!file ? "true" : ""} onClick={handleUpload}>
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
  );
}
