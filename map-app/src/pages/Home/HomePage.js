import React, { useRef } from "react";
import { Link } from "react-router-dom";
import Map from "../../map";
import classes from "./homePage.module.css";
import Logout from "../../components/Logout/Logout";
import useAuth from "../../hooks/useAuth";
import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import Header from "../../components/Header/Header";
export default function Home() {
  const { auth } = useAuth();

  return (
    <div className={classes.container}>
      <Header />
      <main className="mapContainer">
        <div className={classes.svgWrapper}>
          <Map />
        </div>
      </main>
    </div>
  );
}
