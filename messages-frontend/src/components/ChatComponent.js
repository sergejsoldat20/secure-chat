/* eslint-disable no-unused-vars */
import { List } from "antd";

import React, { useEffect, useState, useRef } from "react";
import { PropTypes } from "prop-types";
import Message from "./MessageComponent";
import SendMessage from "./SendMessageComponent";

import authenticate from "../services/auth.service";
const Chat = (props) => {
  const messagesEndRef = useRef(null);
  const [currentUser, setCurrentUser] = useState();

  useEffect(() => {
    loadCurrentUser();
    if (messagesEndRef.current) {
      messagesEndRef.current.scrollTop = messagesEndRef.current.scrollHeight;
    }
  }, [props.messages]);

  const loadCurrentUser = () => {
    authenticate.getCurrentUser().then((result) => {
      console.log(result.data);
      setCurrentUser(result.data);
    });
  };

  const getMessageType = (message) => {
    if (message.senderId === currentUser.id) {
      return "sent";
    } else {
      return "received";
    }
  };

  return (
    <div className="col-8">
      <div
        className="card"
        style={{ height: "500px", overflowY: "scroll" }}
        ref={messagesEndRef}
      >
        <List
          dataSource={props.messages}
          bordered={false}
          renderItem={(message) => (
            <List.Item
              style={{
                border: "none",
                alignItems:
                  getMessageType(message) === "sent" ? "right" : "left",
              }}
            >
              <Message content={message.text} type={getMessageType(message)} />
            </List.Item>
          )}
        />
      </div>
      <SendMessage receiverId={props.receiverId} messageType={"chat"} />
    </div>
  );
};
export default Chat;
Chat.propTypes = {
  receiverId: PropTypes.number,
  messages: PropTypes.array,
};
