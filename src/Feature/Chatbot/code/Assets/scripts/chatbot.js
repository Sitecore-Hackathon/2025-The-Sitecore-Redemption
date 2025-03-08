$(document).ready(function () {
    var configId = $(".chatbot-container").data("config-id");

    $("#send-btn").click(function () {
        sendMessage();
    });

    $('#chat-input').keypress(function (e) {
        if (e.which === 13) {
            sendMessage();
        }
    });

    function sendMessage() {
        var question = $("#chat-input").val().trim();
        if (!question) return;

        appendMessage(question, 'user');
        $("#chat-input").val("");

        showTypingIndicator();

        $.ajax({
            url: '/api/sitecore/ChatBot/Ask',
            type: 'POST',
            data: { question: question, configId: configId },
            success: function (response) {
                removeTypingIndicator();
                var formattedResponse = marked.parse(response.Answer);
                var messageId = appendMessage(formattedResponse, 'bot'); // Get message ID
                addFeedbackButtons(messageId); // Pass the ID to ensure only this response gets the feedback
            },
            error: function () {
                removeTypingIndicator();
                appendMessage("Oops, something went wrong.", 'bot');
            }
        });
    }

    function appendMessage(text, type) {
        var className = (type === 'bot') ? 'bot-message' : 'user-message';
        var messageId = "msg-" + new Date().getTime(); // Unique ID for each message
        $(".chatbot-messages").append(`<div class="message ${className}" id="${messageId}">${text}</div>`);
        $(".chatbot-messages").scrollTop($(".chatbot-messages")[0].scrollHeight);
        return messageId; // Return the ID for later use
    }

    function showTypingIndicator() {
        $(".chatbot-messages").append(`
            <div class="message bot-message typing-indicator">
                <span class="dot"></span><span class="dot"></span><span class="dot"></span>
            </div>
        `);
        $(".chatbot-messages").scrollTop($(".chatbot-messages")[0].scrollHeight);
    }

    function removeTypingIndicator() {
        $(".typing-indicator").remove();
    }

    function addFeedbackButtons(messageId) {
        var feedbackHtml = `
            <div class="feedback" id="feedback-${messageId}">
                Was this helpful?
                <button class="feedback-btn" data-feedback="up" data-message-id="${messageId}">👍</button>
                <button class="feedback-btn" data-feedback="down" data-message-id="${messageId}">👎</button>
            </div>`;
        $("#" + messageId).after(feedbackHtml); // Append feedback **only** to the relevant message
        $(".chatbot-messages").scrollTop($(".chatbot-messages")[0].scrollHeight);
    }

    $(document).on('click', '.feedback-btn', function () {
        var feedback = $(this).data('feedback');
        var messageId = $(this).data('message-id'); // Get the ID of the message being rated

        $.ajax({
            url: '/api/sitecore/ChatBot/SubmitFeedback',
            type: 'POST',
            data: { feedback: feedback, configId: configId }
        });

        // Update **only the relevant feedback section**
        $("#feedback-" + messageId).html("Thank you for your feedback!");
    });
});
