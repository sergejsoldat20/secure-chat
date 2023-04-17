/* eslint-disable no-unused-vars */
import React, { useState } from "react";
import { Input, Button, Row, Col } from "antd";
import { PropTypes } from "prop-types";
import { SendOutlined } from "@ant-design/icons";
import chatService from "../services/chat.service";

export default function SendMessage(props) {
  const [message, setMessage] = useState("");
  const { TextArea } = Input;

  const handleSendMessage = () => {
    if (message !== "") {
      const messageRequest = {
        receiverId: props.receiverId,
        text: message,
      };
      console.log(messageRequest.text);
      console.log();
      chatService.sendMessage(messageRequest);
    }
    setMessage("");
    console.log("Message sent!");
  };

  return (
    <div style={{ display: "flex" }}>
      <Input
        value={message}
        onChange={(e) => setMessage(e.target.value)}
        onPressEnter={handleSendMessage}
      />
      <Button
        icon={<SendOutlined />}
        onClick={() => handleSendMessage()}
      ></Button>
    </div>
  );
}

SendMessage.propTypes = {
  receiverId: PropTypes.number,
};
