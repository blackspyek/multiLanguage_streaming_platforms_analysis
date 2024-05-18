import { useRef, useState, useEffect } from "react";
import axios from "../../api/axios";
import useAuth from "../../hooks/useAuth";
import { Link, useNavigate, useLocation } from "react-router-dom";
import classes from "./loginPage.module.css";
import Header from "../../components/Header/Header";
import LogoNav from "../../components/LogoNav/LogoNav";
import { toast } from "react-toastify";
const LOGIN_URL = "/auth/login";

function Login() {
  const { setAuth } = useAuth();
  const [isFocused, setIsFocused] = useState(null);

  const navigate = useNavigate();
  const location = useLocation();
  const from = location?.state?.from?.pathname || "/";

  const userRef = useRef();
  const errRef = useRef();

  const [user, setUser] = useState("");
  const [pwd, setPwd] = useState("");
  const [errMsg, setErrMsg] = useState("");

  useEffect(() => {
    userRef.current.focus();
  }, []);

  useEffect(() => {
    setErrMsg("");
  }, [user, pwd]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const response = await axios.post(
        LOGIN_URL,
        JSON.stringify({
          username: user,
          password: pwd,
        }),
        {
          headers: {
            "Content-Type": "application/json",
          },
          withCredentials: true,
        }
      );
      const accessToken = response?.data?.tokens?.access;
      const role = response?.data?.role;

      setAuth({ user, pwd, role, accessToken });
      setUser("");
      setPwd("");
      toast.success("Login successful");
      navigate(from, { replace: true });
    } catch (err) {
      if (!err?.response) {
        setErrMsg("Server is not responding. Please try again later.");
      } else if (err?.response?.status === 404) {
        setErrMsg("Invalid username or password");
      } else if (err?.response?.status === 401) {
        setErrMsg("An unathorized request was made. Please try again later.");
      } else {
        setErrMsg("An error occurred. Please try again later.");
      }
      errRef.current.focus();
    }
  };
  return (
    <div className={classes.container}>
      <div className={classes.logoMargin}>
        <LogoNav />
      </div>
      <section className={classes.loginForm}>
        <p
          ref={errRef}
          className={errMsg ? "errmsg" : "offscreen"}
          // eslint-disable-next-line jsx-a11y/aria-proptypes
          aria-live="assertlive"
        >
          {errMsg}
        </p>
        <h1>Sign In</h1>
        <form onSubmit={handleSubmit}>
          <div
            className={`${classes.formLine} ${
              isFocused === "username" ? classes.focusedLine : ""
            } `}
          >
            <label htmlFor="username">Username:</label>
            <input
              type="text"
              id="username"
              ref={userRef}
              autoComplete="off"
              onChange={(e) => setUser(e.target.value)}
              value={user}
              required
              onFocus={(e) => setIsFocused(e.target.id)}
              onBlur={() => setIsFocused(null)}
            />
          </div>
          <div
            className={`${classes.formLine} ${
              isFocused === "password" ? classes.focusedLine : ""
            } `}
          >
            <label htmlFor="password">Password:</label>
            <input
              type="password"
              id="password"
              onChange={(e) => setPwd(e.target.value)}
              value={pwd}
              required
              onFocus={(e) => setIsFocused(e.target.id)}
              onBlur={() => setIsFocused(null)}
            />
          </div>
          <button className={classes.btn}>Sign In</button>
        </form>
        <p className={classes.info}>
          Need an Account? <br />
          <span className="line">
            <Link to="/register">Sign Up</Link>
          </span>
        </p>
      </section>
    </div>
  );
}

export default Login;
