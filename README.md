# secure-chat

Chat application for college purposes. Simple chat app on frontend developed with React.js. 
Backend application is developed with ASP.NET has few steps:
- message is encrypted and splited into n random parts
- encrypted parts are sent to four RabbitMQ servers
- background service is collecting message parts, composing message and saving it to database
