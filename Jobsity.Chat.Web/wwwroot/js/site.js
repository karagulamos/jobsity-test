// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const createNewSignalRConnection = () => {
  const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();

  connection.on("ReceiveNewMessage", (newChat) => {
    const chatMessage = `[${newChat.userId}]: ${newChat.message}`;
    const p = document.createElement("p");
    p.innerText = chatMessage;
    document.getElementById("chat-messages").appendChild(p);
  });

  connection.start().catch((err) => console.error(err.toString()));

  return connection;
};

const connection = createNewSignalRConnection();

const sendMessage = (roomId, userId) => {
  const message = document.getElementById("new-message").value;
  if (!message) return;
  connection
    .invoke("SendNewMessage", roomId, userId, message)
    .catch((err) => console.error(err));

  document.getElementById("new-message").value = "";
};
