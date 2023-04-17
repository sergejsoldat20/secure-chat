/* eslint-disable no-unused-vars */
import React, { useState, useEffect } from "react";
import chatService from "../../services/chat.service";
import userService from "../../services/auth.service";
import ChatList from "../../components/ChatUsersComponent";
import Chat from "../../components/ChatComponent";
import { Grid } from "@mui/material";
import authService from "../../services/auth.service";
export default function FullChat() {
  const [messages, setMessages] = useState([]);
  const [chatUsers, setChatUsers] = useState([]);
  const [receiverId, setReceiverId] = useState(0);
  const [numberOfMessages, setNumberOfMessages] = useState(0);
  let messNum = 0;
  useEffect(() => {
    const intervalId = setInterval(() => {
      // Fetch new messages from your backend API or WebSocket server here
      chatService.getNumberOfMessages((result) => {
        setNumberOfMessages(result.data);
      });
      if (numberOfMessages > messNum) {
        loadChatUsers();
        messNum = numberOfMessages;
      }
    }, 500); // Fetch messages every second

    return () => {
      clearInterval(intervalId);
    };
  }, [receiverId]);

  useEffect(() => {
    loadChatUsers();
  }, []);

  const loadMessages = () => {
    if (receiverId !== 0) {
      chatService.getChatMessages(receiverId).then((result) => {
        console.log(result.data);
        setMessages(result.data);
      });
    }
  };

  const loadChatUsers = () => {
    authService.getChatUsers().then((result) => {
      setChatUsers(result.data);
    });
  };

  const onSelectUser = (userId) => {
    setReceiverId(userId);
    console.log(userId + "User ID");
  };
  return (
    <Grid alignItems="center" justifyContent="center" className="text-center">
      <div className="container">
        <div className="rom">
          <div className="col-md-8 offset-md-2 border rounder p-4 mt-2 shadow">
            <h2 className="text-center m-4">Razgovori</h2>

            <div className="card">
              <div className="card-header">
                <div className="row">
                  <ChatList
                    users={chatUsers}
                    onSelectUser={onSelectUser}
                    receiverId={receiverId}
                  />
                  <Chat messages={messages} receiverId={receiverId} />
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </Grid>
  );
}
