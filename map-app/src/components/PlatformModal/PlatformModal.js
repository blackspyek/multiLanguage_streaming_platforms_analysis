import React, { useState, useEffect } from "react";
import Modal from "react-modal";
import axios from "axios";
import classes from "./platformmodal.module.css";
function PlatformModal({ isOpen, onClose, onCategorySelect }) {
  const [platforms, setPlatforms] = useState([]);
  const [selectedPlatforms, setSelectedPlatforms] = useState([]);
  const [newPlatform, setNewPlatform] = useState("");
  const handlePlatformClick = (platform) => {
    if (selectedPlatforms.includes(platform)) {
      setSelectedPlatforms(selectedPlatforms.filter((p) => p !== platform));
    } else {
      setSelectedPlatforms([...selectedPlatforms, platform]);
    }
  };
  useEffect(() => {
    axios.get("http://localhost:5192/api/platform").then((res) => {
      setPlatforms(res.data);
    });
  }, []);

  return (
    <Modal
      style={{
        content: {
          top: "50%",
          left: "50%",
          right: "auto",
          bottom: "auto",
          marginRight: "-50%",
          transform: "translate(-50%, -50%)",
          maxWidth: "70%",
        },
      }}
      isOpen={isOpen}
      onRequestClose={onClose}
      contentLabel="Platform Modal"
    >
      <h1 className={classes.modalTitle}>Platforms</h1>
      <div className={classes.container}>
        <div className={classes.platforms}>
          {platforms.map((platform) => (
            <button
              key={platform.id}
              onClick={() => handlePlatformClick(platform.name)}
              className={`${classes.modalPlatformBtn} ${
                selectedPlatforms.includes(platform.name)
                  ? classes.selected
                  : ""
              }`}
            >
              {platform.name}
            </button>
          ))}
        </div>
        <div>
          <input
            className={classes.modalInputPlatforms}
            type="text"
            onChange={(e) => {
              setNewPlatform(e.target.value);
            }}
            placeholder="Netflix, Hulu"
          />
        </div>
      </div>
      <button
        className={classes.selectPlatformsBtn}
        onClick={() => {
          var selectedPlatformsVals =
            Object.values(selectedPlatforms).join(",");
          if (newPlatform) {
            selectedPlatformsVals += "," + newPlatform;
          }
          onCategorySelect(selectedPlatformsVals);
        }}
      >
        Select
      </button>
    </Modal>
  );
}
export default PlatformModal;
