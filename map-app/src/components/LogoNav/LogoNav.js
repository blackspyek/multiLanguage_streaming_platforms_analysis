import React from "react";
import classes from "./logonav.module.css";
import { Link } from "react-router-dom";
export default function LogoNav() {
  return (
    <Link to="/">
      <img className={classes.logo} src="logo.png" alt="Logo"></img>
    </Link>
  );
}
