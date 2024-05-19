import React from "react";
import { Link } from "react-router-dom";
import Logout from "../Logout/Logout";
import classes from "./header.module.css";
import useAuth from "../../hooks/useAuth";
import LogoNav from "../LogoNav/LogoNav";
export default function Header() {
  const { auth } = useAuth();
  return (
    <header>
      <LogoNav />
      <nav>
        {Object.keys(auth).length != 0 ? (
          <>
            {auth?.role === "admin" && <Link to="/admin">Admin</Link>}
            {auth?.role === "admin" && (
              <Link to="/uploadstream">Streaming</Link>
            )}
            <Logout />
          </>
        ) : (
          <Link to="/login">Login</Link>
        )}
      </nav>
    </header>
  );
}
