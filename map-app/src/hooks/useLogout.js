import axios from "../api/axios";
import useAuth from "./useAuth";
import { useNavigate } from "react-router-dom";
const LOGOUT_URL = "/auth/logout/cookies";

const useLogout = () => {
  const { setAuth } = useAuth();
  const navigate = useNavigate();
  const logout = async () => {
    try {
      const response = await axios(LOGOUT_URL, {
        withCredentials: true,
      });
      setAuth({});
    } catch (err) {
      console.error(err);
    }
  };

  return logout;
};

export default useLogout;
