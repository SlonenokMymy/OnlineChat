window.onload = function () {

    // Example: Add a custom button to Swagger UI
    var button = document.createElement("button");
    button.innerHTML = "Custom Button";
    button.style.margin = "10px";
    button.onclick = function () {
        alert("Custom Button Clicked!");
    };

    // Find a suitable place to append the button
    var targetElement = document.querySelector(".chat-ui");
    if (targetElement) {
        targetElement.appendChild(button);
    } else {
        console.log("Target element not found.");
    }
};