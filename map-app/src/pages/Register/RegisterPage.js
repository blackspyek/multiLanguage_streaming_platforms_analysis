import { useRef, useState, useEffect } from "react";
import {
  faCheck,
  faTimes,
  faInfoCircle,
} from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import axios from "../../api/axios";
import { Link, useNavigate } from "react-router-dom";
import classes from "./registerPage.module.css";
import LogoNav from "../../components/LogoNav/LogoNav";
import { toast } from "react-toastify";

const USER_REGEX = /^[A-z][A-z0-9-_]{3,23}$/; // 4-24 characters, starts with a letter, only letters, numbers, hyphens, and underscores
const PWD_REGEX = /^(?=.*[A-Z])(?=.*[0-9]).{4,24}$/; // 4-24 characters, at least one uppercase letter, at least one number
const EMAIL_REGEX = /^[A-z0-9._%+-]+@[A-z0-9.-]+\.[A-z]{2,}$/; // email must be valid
const REGISTER_URL = "/auth/register";
function Register() {
  const userRef = useRef();
  const errRef = useRef();
  const [isFocused, setIsFocused] = useState(null);

  const [user, setUser] = useState("");
  const [validName, setValidName] = useState(false);
  const [userFocus, setUserFocus] = useState(false);

  const [email, setEmail] = useState("");
  const [validEmail, setValidEmail] = useState(false);
  const [emailFocus, setEmailFocus] = useState(false);

  const [pwd, setPwd] = useState("");
  const [validPwd, setValidPwd] = useState(false);
  const [pwdFocus, setPwdFocus] = useState(false);

  const [matchPwd, setMatchPwd] = useState("");
  const [validMatch, setValidMatch] = useState(false);
  const [matchFocus, setMatchFocus] = useState(false);

  const [errMsg, setErrMsg] = useState("");
  const [success, setSuccess] = useState(false);

  const navigate = useNavigate();
  useEffect(() => {
    userRef.current.focus();
  }, []);

  useEffect(() => {
    console.log(user);
    const result = USER_REGEX.test(user);
    setValidName(result);
  }, [user]);

  useEffect(() => {
    console.log(email);
    const result = EMAIL_REGEX.test(email);
    setValidEmail(result);
  }, [email]);

  useEffect(() => {
    console.log(pwd);
    const result = PWD_REGEX.test(pwd);
    setValidPwd(result);
    const match = pwd === matchPwd;
    setValidMatch(match);
  }, [pwd, matchPwd]);

  useEffect(() => {
    setErrMsg("");
  }, [user, pwd, matchPwd]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    const v1 = USER_REGEX.test(user);
    const v2 = PWD_REGEX.test(pwd);
    if (!v1 || !v2) {
      setErrMsg("Invalid username or password");
      return;
    }
    try {
      const response = await axios.post(
        REGISTER_URL,
        JSON.stringify({
          username: user,
          email: email,
          password: pwd,
        }),
        {
          headers: {
            "Content-Type": "application/json",
          },
          withCredentials: true,
        }
      );

      setSuccess(true);

      setPwd("");
      setUser("");
      setEmail("");
      setMatchPwd("");
      toast.success("Registration successful. Please login.");
      navigate("/login");
    } catch (e) {
      if (!e?.response) {
        setErrMsg("No server response. Please try again later.");
      } else if (e.response.status === 400) {
        setErrMsg("Username already exists. Please choose another.");
      } else {
        setErrMsg("An error occurred. Please try again later.");
      }
      errRef.current.focus();
      toast.error("Registration failed. Check error message above.");
    }
  };

  return (
    <>
      <div className={classes.container}>
        <div className={classes.logoMargin}>
          <LogoNav />
        </div>
        <section className={classes.loginForm}>
          <p
            ref={errRef}
            className={errMsg ? classes.errmsg : classes.offscreen}
            aria-live="assertive"
          >
            {errMsg}
          </p>
          <h1>Register</h1>
          <form onSubmit={handleSubmit}>
            <div
              className={`${classes.formLine} ${
                isFocused === "username" ? classes.focusedLine : ""
              } `}
            >
              <label htmlFor="username">
                Username:
                <span className={validName ? classes.valid : classes.hide}>
                  <FontAwesomeIcon icon={faCheck} />
                </span>
                <span
                  className={
                    validName || !user ? classes.hide : classes.invalid
                  }
                >
                  <FontAwesomeIcon icon={faTimes} />
                </span>
              </label>
              <input
                type="text"
                id="username"
                ref={userRef}
                autoComplete="off"
                onChange={(e) => setUser(e.target.value)}
                required
                value={user}
                aria-invalid={validName ? "false" : "true"}
                aria-describedby="uidnote"
                onFocus={(e) => {
                  setUserFocus(true);
                  setIsFocused(e.target.id);
                }}
                onBlur={() => {
                  setUserFocus(false);
                  setIsFocused(null);
                }}
              />
              <p
                id="uidnote"
                className={
                  userFocus && user && !validName
                    ? classes.instructions
                    : classes.offscreen
                }
              >
                <FontAwesomeIcon icon={faInfoCircle} />
                4-24 characters, starts with a letter, only letters, numbers,
                hyphens, and underscores
              </p>
            </div>
            <div
              className={`${classes.formLine} ${
                isFocused === "email" ? classes.focusedLine : ""
              } `}
            >
              <label htmlFor="email">
                Email:
                <span className={validEmail ? classes.valid : classes.hide}>
                  <FontAwesomeIcon icon={faCheck} />
                </span>
                <span
                  className={
                    validEmail || !email ? classes.hide : classes.invalid
                  }
                >
                  <FontAwesomeIcon icon={faTimes} />
                </span>
              </label>
              <input
                type="text"
                id="email"
                onChange={(e) => setEmail(e.target.value)}
                required
                value={email}
                aria-invalid={validEmail ? "false" : "true"}
                aria-describedby="emailnote"
                onFocus={(e) => {
                  setEmailFocus(true);
                  setIsFocused(e.target.id);
                }}
                onBlur={() => {
                  setEmailFocus(false);
                  setIsFocused(null);
                }}
              />
              <p
                id="uidnote"
                className={
                  emailFocus && email && !validEmail
                    ? classes.instructions
                    : classes.offscreen
                }
              >
                <FontAwesomeIcon icon={faInfoCircle} />
                Email must be valid
              </p>
            </div>
            <div
              className={`${classes.formLine} ${
                isFocused === "password" ? classes.focusedLine : ""
              } `}
            >
              <label htmlFor="password">
                Password:
                <FontAwesomeIcon
                  icon={faCheck}
                  className={validPwd ? classes.valid : classes.hide}
                />
                <FontAwesomeIcon
                  icon={faTimes}
                  className={validPwd || !pwd ? classes.hide : classes.invalid}
                />
              </label>
              <input
                type="password"
                id="password"
                onChange={(e) => setPwd(e.target.value)}
                required
                value={pwd}
                aria-invalid={validPwd ? "false" : "true"}
                aria-describedby="pwdnote"
                onFocus={(e) => {
                  setPwdFocus(true);
                  setIsFocused(e.target.id);
                }}
                onBlur={() => {
                  setPwdFocus(false);
                  setIsFocused(null);
                }}
              />
              <p
                id="pwdnote"
                className={
                  pwdFocus && pwd && !validPwd
                    ? classes.instructions
                    : classes.offscreen
                }
              >
                <FontAwesomeIcon icon={faInfoCircle} />
                4 to 24 characters.
                <br />
                at least one uppercase letter, at least one number
                <br />
              </p>
            </div>
            <div
              className={`${classes.formLine} ${
                isFocused === "confirm_pwd" ? classes.focusedLine : ""
              } `}
            >
              <label htmlFor="confirm_pwd">
                Confirm Password:
                <FontAwesomeIcon
                  icon={faCheck}
                  className={
                    validMatch && matchPwd ? classes.valid : classes.hide
                  }
                />
                <FontAwesomeIcon
                  icon={faTimes}
                  className={
                    validMatch || !matchPwd ? classes.hide : classes.invalid
                  }
                />
              </label>
              <input
                type="password"
                id="confirm_pwd"
                onChange={(e) => setMatchPwd(e.target.value)}
                required
                value={matchPwd}
                aria-invalid={validMatch ? "false" : "true"}
                aria-describedby="confirmnote"
                onFocus={(e) => {
                  setMatchFocus(true);
                  setIsFocused(e.target.id);
                }}
                onBlur={() => {
                  setMatchFocus(false);
                  setIsFocused(null);
                }}
              />
              <p
                id="confirmnote"
                className={
                  matchFocus && !validMatch
                    ? classes.instructions
                    : classes.offscreen
                }
              >
                <FontAwesomeIcon icon={faInfoCircle} />
                Must match the first password input field.
              </p>
            </div>
            <button
              className={classes.btn}
              disabled={!validName || !validPwd || !validMatch ? true : false}
            >
              Sign Up
            </button>
          </form>
          <p className={classes.info}>
            Already Register
            <br />
            <span className="line">
              <Link to="/login">Sign In</Link>
            </span>
          </p>
        </section>
      </div>
    </>
  );
}

export default Register;
