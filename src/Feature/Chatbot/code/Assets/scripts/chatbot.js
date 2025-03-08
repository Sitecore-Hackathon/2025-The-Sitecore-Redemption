$(document).ready(function () {
    var configId = $("#chatbot-widget-container").data("config-id");

    $("#open-chat-btn").click(function () {
        $("#chat-window").toggle();
    });

    $("#send-btn").click(function () {
        var question = $("#chat-input").val();
        if (question.trim().length === 0) return;

        $("#chat-messages").append("<div>User: " + question + "</div>");
        $("#chat-input").val("");

        $.ajax({
            url: '/api/sitecore/ChatBot/Ask',
            type: 'POST',
            data: { question: question, configId: configId },
            success: function (response) {
                $("#chat-messages").append("<div>Bot: " + response.Answer + "</div>");
                addFeedbackButtons();
            },
            error: function () {
                $("#chat-messages").append("<div>Bot: Oops, something went wrong.</div>");
            }
        });
    });

    function addFeedbackButtons() {
        var feedbackHtml = `
            <div>
                Was this helpful? 
                <button class="feedback-btn" data-feedback="up">👍</button>
                <button class="feedback-btn" data-feedback="down">👎</button>
            </div>`;
        $("#chat-messages").append(feedbackHtml);
    }

    $(document).on('click', '.feedback-btn', function () {
        var feedback = $(this).data('feedback');
        var lastQuestion = $("#chat-messages div:contains('User:')").last().text().replace("User: ", "");
        var lastAnswer = $("#chat-messages div:contains('Bot:')").last().text().replace("Bot: ", "");

        $.ajax({
            url: '/api/sitecore/Chatbot/SubmitFeedback',
            type: 'POST',
            data: {
                question: lastQuestion,
                answer: lastAnswer,
                feedback: feedback,
                configId: configId
            },
            success: function () {
                console.log("Feedback submitted");
            }
        });

        $(".feedback-btn").parent().html("Thank you for your feedback!");
    });

});
