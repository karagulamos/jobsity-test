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

function manageConnectionState(message, chatDisabled) {
  document.getElementById("send-chat").disabled = Boolean(chatDisabled);

  const p = document.createElement("p");
  p.innerHTML = message;
  document.getElementById("chat-messages").appendChild(p);
}

async function joinRoom() {
  try {
    await connection.invoke("JoinRoom", ChatSession.roomId);
    console.log("Joined room.");

    if (!ChatSession.lastReadAt) return;

    manageConnectionState("<strong>Connection reestablished.</strong>", false);

    await connection.invoke(
      "GetChatsSince",
      ChatSession.lastReadAt,
      ChatSession.roomId
    );
  } catch (err) {
    console.error(err.toString());
  }
}

connection.onreconnecting((error) => {
  console.assert(connection.state === signalR.HubConnectionState.Reconnecting);
  console.log(`Connection lost due to error "${error}". Reconnecting...`);

  manageConnectionState("<em>Connection lost. Reconnecting...</em>", true);
});

connection.onreconnected(async (connectionId) => {
  console.assert(connection.state === signalR.HubConnectionState.Connected);
  console.log(`Connection reestablished with connectionId "${connectionId}".`);

  await joinRoom();
});

async function startConnection() {
  try {
    await connection.start();
    console.assert(connection.state === signalR.HubConnectionState.Connected);
    console.log("Connected to chat server.");

    await joinRoom();
  } catch (err) {
    console.assert(
      connection.state === signalR.HubConnectionState.Disconnected
    );
    console.log(err);
    setTimeout(startConnection, 5000);
  }
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
