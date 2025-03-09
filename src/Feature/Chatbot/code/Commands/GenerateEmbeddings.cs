using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;
using SitecoreRedemption.Feature.Chatbot.Repositories;
using SitecoreRedemption.Feature.Chatbot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SitecoreRedemption.Feature.Chatbot.Commands
{
    public class GenerateEmbeddingsCommand : Command
    {
        public override void Execute(CommandContext context)
        {
            if (context.Items == null || context.Items.Length == 0)
            {
                SheerResponse.Alert("Please select a Chatbot Configuration item first.");
                return;
            }

            var settingsItem = context.Items[0];
            if (settingsItem.TemplateID != Templates.ChatbotConfiguration.Id)
            {
                SheerResponse.Alert("Invalid item selected. Please select a Chatbot Configuration item.");
                return;
            }

            var embeddingRepo = new JsonEmbeddingRepository();
            var chatbotService = new ChatbotService(embeddingRepo);

            try
            {
                chatbotService.CrawlAndGenerateEmbeddings(settingsItem);
                SheerResponse.Alert("Embeddings successfully generated!");
            }
            catch (System.Exception ex)
            {
                Log.Error("Error generating embeddings", ex, this);
                SheerResponse.Alert("An error occurred: " + ex.Message);
            }
        }

        public override CommandState QueryState(CommandContext context)
        {
            if (context.Items == null || context.Items.Length != 1)
                return CommandState.Hidden;

            Item item = context.Items[0];
            return item.TemplateID == Templates.ChatbotConfiguration.Id ? CommandState.Enabled : CommandState.Hidden;
        }
    }
}
