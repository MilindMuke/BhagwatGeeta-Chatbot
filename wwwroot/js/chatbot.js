document.addEventListener('DOMContentLoaded', function() {
    const chatForm = document.getElementById('chat-form');
    const userInput = document.getElementById('user-input');
    const chatHistory = document.getElementById('chat-history');

    chatForm.addEventListener('submit', async function(e) {
        e.preventDefault();
        const message = userInput.value.trim();
        if (!message) return;
        appendMessage('user', message);
        userInput.value = '';
        const response = await fetch('/api/chatbot', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ question: message })
        });
        const data = await response.json();
        appendMessage('bot', data.answer);
    });

    function appendMessage(sender, text) {
        const bubble = document.createElement('div');
        bubble.className = sender === 'user' ? 'user-bubble' : 'bot-bubble';
        bubble.textContent = text;
        chatHistory.appendChild(bubble);
        chatHistory.scrollTop = chatHistory.scrollHeight;
    }
});
