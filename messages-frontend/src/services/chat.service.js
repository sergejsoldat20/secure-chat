import base from "./base.service";

const instance = base.service(true);

export const sendMessage = (messageRequest) => {
  return instance.post("/Messages/send-message", messageRequest);
};

export const getChatMessages = (id) => {
  return instance.get(`/Messages/chat-messages/${id}`);
};

export const getNumberOfMessages = (id) => {
  return instance.get(`/Messages/number-of-messages/${id}`);
};

export default {
  sendMessage,
  getChatMessages,
  getNumberOfMessages,
};
