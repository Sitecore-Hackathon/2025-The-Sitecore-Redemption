using SitecoreRedemption.Feature.Chatbot.Repositories;
using Sitecore.DependencyInjection;
using System.Linq;
using System.Web.Mvc;
using SitecoreRedemption.Feature.Chatbot.Services;
using Sitecore.Mvc.Controllers;
using Sitecore.Collections;
using Sitecore.Mvc.Presentation;
using Sitecore;


namespace SitecoreRedemption.Feature.Chatbot.Controllers
{
    public class ChatbotController : SitecoreController
    {
        private readonly ChatbotService _chatbotService;

        public ChatbotController() : this(new JsonEmbeddingRepository()) { }

        public ChatbotController(IChatbotEmbeddingRepository embeddingRepo)
        {
            _chatbotService = new ChatbotService(embeddingRepo);
        }

        [HttpPost]
        public JsonResult Ask(string question, string configId)
        {
            var settingsItem = Context.Database.GetItem(configId);

            if (settingsItem == null || settingsItem.TemplateID != Templates.ChatbotConfiguration.Id)
                return Json(new { Answer = "Invalid configuration." });

            var brandPrompt = settingsItem.Fields[Templates.ChatbotConfiguration.Fields.BrandPrompt]?.Value ?? string.Empty;

            var answer = _chatbotService.GenerateAnswer(question, brandPrompt);

            return Json(new { Answer = answer });
        }

        [HttpPost]
        public JsonResult SubmitFeedback(string question, string answer, string feedback, string configId)
        {
            var settingsItem = Context.Database.GetItem(configId);
            if (settingsItem == null || settingsItem.TemplateID != Templates.ChatbotConfiguration.Id)
                return Json(new { Success = false, Message = "Invalid configuration." });

            // TODO: Implement feedback logic here.
            Sitecore.Diagnostics.Log.Info(
                $"Feedback (Config:{configId}): Question='{question}', Answer='{answer}', Feedback='{feedback}'", this);

            return Json(new { Success = true });
        }
    }
}
