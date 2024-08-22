document.addEventListener("DOMContentLoaded", () => {
    // Инициализация SignalR после загрузки DOM 
    initializeSignalR();

    var user = getCurrentUser();
    var url = `/Channels/GetAllChats/${ownerId = user.Id}`

    fetch(url, {
        method: 'GET',
        headers: {
            'Authorization': `Bearer ${getAuthToken()}`
        }
    })
        .then(response => response.json())
        .then(chats => {
            chats.forEach(chat => createNewChat(chat));
        })
        .catch(err => console.error(err.toString()));
});

function initializeSignalR() {
    // Создание подключения к SignalR Hub
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/chathub")
        .build();

    const currentUser = getCurrentUser();

    // Обработка входящих сообщений
    connection.on("ReceiveMessage", (user, message, chatId) => {
        var mainContainer = document.getElementById('chatContainer');
        //немного магии говнокода
        var chatBox = mainContainer.getElementsByClassName('chat-box' + chatId)[0];

        addMessageToChat(user, message, chatBox);
    });

    // Запуск подключения
    connection.start().catch(err => console.error(err.toString()));
    document.addEventListener('sendMessageEvent', function (event) {
        sendMessage(connection, event.detail.user, event.detail.messageText, event.detail.chatId);
    });

    localStorage.setItem('connectionR', connection);
}

function addMessageToChat(user, message, chatBox) {
    const msg = `${user}: ${message}`;
    const ul = document.createElement("ul");
    ul.textContent = msg;
    chatBox.appendChild(ul);
}

function createNewChat(chatData) {
    // Найдите основной контейнер для чатов
    const chatContainer = document.getElementById('chatContainer');

    // Создание контейнера для нового чата
    const chatBox = document.createElement('div');
    chatBox.classList.add('chat-box' + chatData.id);

    // Добавление заголовка чата
    const chatHeader = document.createElement('h2');
    chatHeader.textContent = chatData.chatName;
    chatBox.appendChild(chatHeader);

    // Создание контейнера для ввода сообщений
    const chatInputContainer = document.createElement('div');
    chatInputContainer.classList.add('chat-input' + chatData.id);

    // Поле ввода сообщения
    const messageInput = document.createElement('input');
    messageInput.setAttribute('type', 'text');
    messageInput.setAttribute('placeholder', 'Your message');
    chatInputContainer.appendChild(messageInput);

    // Кнопка отправки сообщения
    const sendButton = document.createElement('button');
    sendButton.textContent = 'Send';
    chatInputContainer.appendChild(sendButton);

    chatBox.appendChild(chatInputContainer);

    // Список сообщений
    chatData.chatHistories.forEach(msg => addMessageToChat(msg.user.username, msg.content, chatBox));

    const user = getCurrentUser();

    // Обработка отправки сообщения
    sendButton.addEventListener('click', function () {
        const messageText = messageInput.value.trim();
        if (messageText) {
            //addMessageToChat(user.Name, messageText, chatBox); // Добавление вашего сообщения
            messageInput.value = ''; // Очистка поля ввода
            const sendMessageEvent = new CustomEvent('sendMessageEvent', {
                detail: {
                    user: user,
                    messageText: messageText,
                    chatId: chatData.id
                }
            });
            document.dispatchEvent(sendMessageEvent);

        }
    });

    // Добавление чата на страницу
    chatContainer.appendChild(chatBox);
}

function sendMessage(connection, user, message, chatId) {
    const data = {
        "id": 0,
        "chat": {
            "id": chatId,
            "chatName": "",
            "ownerId": 0
        },
        "content": message,
        "messageDate": new Date().toISOString(),
        "user": {
            "id": user.Id,
            "username": ""
        }
    };
    connection.invoke("SendMessage", user.Name, message, chatId)
        .catch(err => console.error(err.toString()))
        .then(() => {
            // После отправки сообщения в SignalR, сохраняем его в БД через API
            fetch('/Channels/SaveMessage', {
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

function getAuthToken() {
    return localStorage.getItem('authToken');
}

function getCurrentUser() {
    return user = {
        "Name": localStorage.getItem('userName'),
        "Id": localStorage.getItem('userId'),
    };
}