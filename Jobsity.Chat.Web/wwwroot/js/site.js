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

  ChatSession.lastReadAt = newChat.dateSent;
});

function joinRoom() {
  connection
    .invoke("JoinRoom", ChatSession.roomId)
    .catch((err) => console.error(err));
}

function manageConnectionState(message, chatDisabled) {
  document.getElementById("send-chat").disabled = Boolean(chatDisabled);

  const p = document.createElement("p");
  p.innerHTML = message;
  document.getElementById("chat-messages").appendChild(p);
}

connection.onreconnecting((error) => {
  console.assert(connection.state === signalR.HubConnectionState.Reconnecting);
  console.log(`Connection lost due to error "${error}". Reconnecting...`);

  manageConnectionState("<em>Connection lost. Reconnecting...</em>", true);
});

connection.onreconnected((connectionId) => {
  console.assert(connection.state === signalR.HubConnectionState.Connected);
  console.log(`Connection reestablished with connectionId "${connectionId}".`);

  manageConnectionState("<strong>Connection reestablished.</strong>", false);
  joinRoom();

  if (ChatSession.lastReadAt) {
    connection
      .invoke("GetChatsSince", ChatSession.lastReadAt, ChatSession.roomId)
      .catch((err) => console.error(err));
  }
});

async function startConnection() {
  try {
    await connection.start();
    console.assert(connection.state === signalR.HubConnectionState.Connected);
    console.log("Connected to chat server.");
  } catch (err) {
    console.assert(
      connection.state === signalR.HubConnectionState.Disconnected
    );
    console.log(err);
    setTimeout(startConnection, 5000);
  }

  joinRoom();
}

connection.onclose(async () => {
  await startConnection();
});

startConnection();

const sendMessage = (roomId, userId) => {
  const message = document.getElementById("new-message").value;
  if (!message) return;
  connection
    .invoke("SendNewMessage", roomId, userId, message)
    .catch((err) => console.error(err));

  document.getElementById("new-message").value = "";
};
