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
                formattedResponse = ensureLinksOpenNewTab(formattedResponse); // Process links
                appendMessage(formattedResponse, 'bot');
                addFeedbackButtons();
            },
            error: function () {
                removeTypingIndicator();
                appendMessage("Oops, something went wrong.", 'bot');
            }
        });
    }

    function appendMessage(text, type) {
        var className = (type === 'bot') ? 'bot-message' : 'user-message';
        $(".chatbot-messages").append(`<div class="message ${className}">${text}</div>`);
        $(".chatbot-messages").scrollTop($(".chatbot-messages")[0].scrollHeight);
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

    function addFeedbackButtons() {
        var feedbackHtml = `
            <div class="feedback">
                Was this helpful?
                <button class="feedback-btn" data-feedback="up">👍</button>
                <button class="feedback-btn" data-feedback="down">👎</button>
            </div>`;
        $(".chatbot-messages").append(feedbackHtml);
        $(".chatbot-messages").scrollTop($(".chatbot-messages")[0].scrollHeight);
    }

    $(document).on('click', '.feedback-btn', function () {
        var feedback = $(this).data('feedback');
        $.ajax({
            url: '/api/sitecore/ChatBot/SubmitFeedback',
            type: 'POST',
            data: { feedback: feedback, configId: configId }
        });
        $(this).closest(".feedback").html("Thank you for your feedback!");
    });

    function ensureLinksOpenNewTab(htmlContent) {
        return htmlContent.replace(/<a /g, '<a target="_blank" rel="noopener noreferrer" ');
    }
});
