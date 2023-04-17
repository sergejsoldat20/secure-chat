/* eslint-disable no-unused-vars */
import React from "react";
import { useNavigate } from "react-router-dom";
import authenticate from "../services/auth.service";
import { Button } from "antd";

export default function Navbar() {
  const logout = () => {
    localStorage.removeItem("jwt");
    localStorage.removeItem("role");
    localStorage.removeItem("id");
    navigate("/");
  };
  const login = () => {
    navigate("/");
  };
  const navigate = useNavigate();
  return (
    <nav className="navbar navbar-expand-lg navbar-light bg-light">
      <button
        className="navbar-toggler"
        type="button"
        data-toggle="collapse"
        data-target="#navbarTogglerDemo01"
        aria-controls="navbarTogglerDemo01"
        aria-expanded="false"
        aria-label="Toggle navigation"
      >
        <span className="navbar-toggler-icon"></span>
      </button>
      <div className="collapse navbar-collapse" id="navbarTogglerDemo01">
        <a className="navbar-brand" href="#">
          Secure chat
        </a>
        <ul className="navbar-nav mr-auto mt-2 mt-lg-0">
          <li className="nav-item active">
            <a className="nav-link" href="/chat">
              Messages <span className="sr-only">(current)</span>
            </a>
          </li>
        </ul>
      </div>
    </nav>
  );
}
