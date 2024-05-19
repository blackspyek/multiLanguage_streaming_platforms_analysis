import "./App.css";
import React from "react";
import Register from "./pages/Register/RegisterPage";
import Login from "./pages/Login/LoginPage";
import { Routes, Route } from "react-router-dom";
import Layout from "./components/Layout";
import Map from "./map";
import NotFound from "./components/NotFound";
import RequireAuth from "./components/RequireAuth";
import Unauthorized from "./components/Unauthorized";
import Admin from "./components/Admin";
import PersistLogin from "./components/PersistLogin";
import Home from "./pages/Home/HomePage";
import UploadStreaming from "./pages/UploadStreaming/UploadStreaming";
const ROLES = {
  ADMIN: "admin",
  USER: "user",
};

function App() {
  return (
    <Routes>
      <Route path="/" element={<Layout />}>
        {/* public routes */}
        <Route path="login" element={<Login />} />
        <Route path="register" element={<Register />} />
        <Route path="map" element={<Map />} />
        <Route path="unauthorized" element={<Unauthorized />} />
        <Route element={<PersistLogin />}>
          <Route path="/" element={<Home />} />

          {/* private routes */}
          <Route element={<RequireAuth allowedRoles={[ROLES.ADMIN]} />}>
            <Route path="admin" element={<Admin />} />
            <Route path="uploadstream" element={<UploadStreaming />} />
          </Route>
        </Route>
        {/* 404 */}
        <Route path="*" element={<NotFound />} />
      </Route>
    </Routes>
  );
}

export default App;
