import React from "react";
import "./App.css";
import Login from "./pages/Login/Login";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import "../node_modules/bootstrap/dist/css/bootstrap.min.css";
import Navbar from "./layout/Navbar";
import FullChat from "./pages/Chat/FullChat";

function App() {
  return (
    <div className="App">
      <Router>
        <Navbar />
        <Routes>
          <Route exact path="/" element={<Login />} />
          <Route exact path="/chat" element={<FullChat />} />
        </Routes>
      </Router>
    </div>
  );
}

export default App;
