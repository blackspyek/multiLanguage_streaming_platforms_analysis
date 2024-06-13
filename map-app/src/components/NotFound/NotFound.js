import React from "react";
import classes from "./notfound.module.css";
import { Link } from "react-router-dom";

export default function NotFound() {
  return (
    <div className={classes.container}>
      <h1 className={classes.notfound}>Error 404 - the site cannot be find!</h1>
      <Link to="/" className={classes.linkhome}>
        Go back to the main page
      </Link>
    </div>
  );
}
