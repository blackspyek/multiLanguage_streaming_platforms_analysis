import axios from "../api/axios";
import useAuth from "./useAuth";

const REFRESH_URL = "/auth/refreshtoken";
function useRefreshToken() {
  const { setAuth } = useAuth();

  const refresh = async () => {
    const response = await axios.get(REFRESH_URL, {
      withCredentials: true,
    });
    setAuth((prev) => {
      return {
        ...prev,
        role: response.data.role,
        accessToken: response.data.access_token,
      };
    });
    return response.data.access_token;
  };

  return refresh;
}

export default useRefreshToken;
