document.getElementById("send-btn").addEventListener("click", async () => {
    const message = document.querySelector("#user-input").value;
    if (!message.trim()) return;

    // Hiển thị tin nhắn người dùng
    appendMessage(message, "outgoing");

    // Gửi về server
    const res = await fetch("/Chat/GetResponse", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ message: message })
    });

    const data = await res.json();
    appendMessage(data.reply, "incoming");
});

function appendMessage(msg, type) {
    const chatBox = document.querySelector(".chatbox");
    const div = document.createElement("div");
    div.className = `message ${type}`;
    div.innerText = msg;
    chatBox.appendChild(div);
}
