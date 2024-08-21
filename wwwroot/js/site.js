
function initializeSignalR() {
    // Создание подключения к SignalR Hub
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/chathub")
        .build();

    const currentUser = getCurrentUser();

    // Обработка входящих сообщений
    connection.on("ReceiveMessage", (user, message) => {
        addMessageToChat(user, message);
    });

    // Запуск подключения
    connection.start().catch(err => console.error(err.toString()));

    // Обработка нажатия кнопки "Send"
    document.getElementById("sendButton").addEventListener("click", () => {
        const user = document.getElementById("userInput").value;
        const message = document.getElementById("messageInput").value;
        sendMessage(connection, user, message);
    });
}

function getCurrentUser() {
    var thisUser = {
        id : 4
    };

    return thisUser;
}

function addMessageToChat(user, message) {
    const msg = `${user}: ${message}`;
    const li = document.createElement("li");
    li.textContent = msg;
    document.getElementById("messagesList").appendChild(li);
}

function sendMessage(connection, user, message) {
    debugger;
    const data = {
        "id": 0,
        "chat": {
            "id": 1,
            "chatName": "string",
            "ownerId": 0
        },
        "content": message,
        "messageDate": "2024-08-21T16:10:13.863Z",
        "user": {
            "id": 4,
            "username": "string"
        }
    };
    connection.invoke("SendMessage", user, message)
        .catch(err => console.error(err.toString()))
        .then(() => {
            // После отправки сообщения в SignalR, сохраняем его в БД через API
            fetch('https://localhost:7071/Channels/SaveMessage', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${getAuthToken()}`
                },
                body: JSON.stringify(data) 
            }).catch(err => console.error(err.toString()));
        })
        .catch(err => console.error(err.toString()));
}

document.addEventListener("DOMContentLoaded", () => {
    // Инициализация SignalR после загрузки DOM
    initializeSignalR();

    fetch('/Channels/GetChatHistory/1', {
        method: 'GET',
        headers: {
            'Authorization': `Bearer ${getAuthToken()}`
        }
    })
        .then(response => response.json())
        .then(messages => {
            messages.forEach(msg => addMessageToChat(msg.user, msg.content));
        })
        .catch(err => console.error(err.toString()));
});

function getAuthToken() {
    return localStorage.getItem('authToken');
}
