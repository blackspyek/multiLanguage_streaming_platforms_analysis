import React from "react";
import classes from "./updateddatainfo.module.css";

export default function UpdatedDataInfo({ dataUpdated }) {
  const handleRefresh = () => {
    window.location.reload();
  };

  return (
    dataUpdated && (
      <div className={classes.container}>
        <button onClick={handleRefresh} className={classes.refreshButton}>
          Data has been updated. Click to refresh.
        </button>
      </div>
    )
  );
}
