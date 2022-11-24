// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

let connection = new signalR.HubConnectionBuilder()
  .withUrl("/chatHub")
  .withAutomaticReconnect()
  .build();

connection.on("ReceiveNewMessage", (newChat) => {
  const chatMessage = `[${newChat.userId}]: ${newChat.message}`;
  const p = document.createElement("p");
  p.innerText = chatMessage;
  document.getElementById("chat-messages").appendChild(p);
});

async function startConnection() {
  try {
    await connection.start();
    console.log("Connected to chat hub.");

    await connection.invoke("JoinRoom", ChatSession.roomId);
  } catch (err) {
    console.log(err);
    setTimeout(startConnection, 5000);
  }
}

startConnection();

const sendMessage = (roomId, userId) => {
  const message = document.getElementById("new-message").value;
  if (!message) return;
  connection
    .invoke("SendNewMessage", roomId, userId, message)
    .catch((err) => console.error(err));

  document.getElementById("new-message").value = "";
};
