/* eslint-disable no-unused-vars */
import React from "react";
import { PropTypes } from "prop-types";

export default function Message(props) {
  const messageBoxStyle = {
    display: "inline-block",
    padding: "10px",
    borderRadius: "10px",
    backgroundColor: props.type === "sent" ? "#DCF8C6" : "#E6E6E6",
    maxWidth: "60%",
    textAlign: props.type === "sent" ? "right" : "left",
    float: props.type === "sent" ? "right" : "left",
    marginLeft: props.type === "received" ? "10px" : "auto",
    marginRight: props.type === "received" ? "auto" : "10px",
  };

  const messageContentStyle = {
    fontSize: "16px",
    color: "#000000",
  };

  return (
    <div className={`message-box ${props.type}`} style={messageBoxStyle}>
      <div className="message-content" style={messageContentStyle}>
        {props.content}
      </div>
    </div>
  );
}

Message.propTypes = {
  content: PropTypes.number,
  type: PropTypes.string,
};
