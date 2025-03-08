using Newtonsoft.Json;
using Sitecore.Configuration;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using SitecoreRedemption.Feature.Chatbot.Models;
using SitecoreRedemption.Feature.Chatbot.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SitecoreRedemption.Feature.Chatbot.Services
{
    public class ChatbotService
    {
        private readonly IChatbotEmbeddingRepository _embeddingRepository;
        private static readonly HttpClient _httpClient = new HttpClient();

        private static readonly string OllamaEmbedUrl = Settings.GetSetting("Feature.ChatBot.OllamaEmbedUrl", "http://localhost:11434/api/embeddings");
        private static readonly string ModelName = Settings.GetSetting("Feature.ChatBot.ModelName", "mistral");

        public ChatbotService(IChatbotEmbeddingRepository embeddingRepository)
        {
            _embeddingRepository = embeddingRepository;
        }
        public string GenerateAnswer(string question, string brandPrompt)
        {
            var contentSearchResults = GetRelevantContent(question);

            var contentBuilder = new StringBuilder();
            foreach (var result in contentSearchResults)
            {
                contentBuilder.AppendLine(result.ContentExcerpt);
            }

            var fullPrompt = $"{brandPrompt}\n\n" +
                             $"Content:\n{contentBuilder}\n\n" +
                             $"Question: {question}\nAnswer:";

            return OllamaApiService.GetCompletion(fullPrompt);
        }

        private IList<ChatbotEmbedding> GetRelevantContent(string query)
        {
            // Simplified retrieval based on keyword match
            var allEmbeddings = _embeddingRepository.GetEmbeddings();
            return allEmbeddings
                .Where(e => e.ContentExcerpt.ToLower().Contains(query.ToLower()))
                .Take(5)
                .ToList();
        }
        public void CrawlAndGenerateEmbeddings(Item settingsItem)
        {
            _embeddingRepository.ClearEmbeddings();

            var rootsField = (MultilistField)settingsItem.Fields[Templates.ChatbotConfiguration.Fields.ContentRoots];
            var includedTemplatesField = (MultilistField)settingsItem.Fields[Templates.ChatbotConfiguration.Fields.IncludedTemplates];
            var excludedTemplatesField = (MultilistField)settingsItem.Fields[Templates.ChatbotConfiguration.Fields.ExcludedTemplates];

            var roots = rootsField.GetItems();
            var includedTemplates = includedTemplatesField.GetItems();
            var excludedTemplates = excludedTemplatesField.GetItems();

            foreach (var root in roots)
            {
                CrawlItemRecursive(root, includedTemplates, excludedTemplates);
            }
        }

        private void CrawlItemRecursive(Item item, Item[] includedTemplates, Item[] excludedTemplates)
        {
            if (ShouldIndex(item, includedTemplates, excludedTemplates))
            {
                var content = ExtractItemContent(item);
                var embedding = GenerateEmbedding(content);

                if (embedding != null)
                {
                    _embeddingRepository.SaveEmbedding(new ChatbotEmbedding
                    {
                        ItemId = item.ID.ToString(),
                        ItemPath = item.Paths.FullPath,
                        ContentExcerpt = content,
                        Vector = embedding
                    });
                }
            }

            foreach (Item child in item.Children)
            {
                CrawlItemRecursive(child, includedTemplates, excludedTemplates);
            }
        }

        private bool ShouldIndex(Item item, Item[] includedTemplates, Item[] excludedTemplates)
        {
            if (excludedTemplates.Any() && excludedTemplates.Any(t => t.ID == item.TemplateID))
                return false;

            if (includedTemplates.Any())
                return includedTemplates.Any(t => t.ID == item.TemplateID);

            return true; // if no includedTemplates specified, include all by default
        }

        private string ExtractItemContent(Item item)
        {
            return string.Join(" ", item.Fields
                .Where(f => !f.Name.StartsWith("__") && !string.IsNullOrEmpty(f.Value))
                .Select(f => Sitecore.Web.UI.WebControls.FieldRenderer.Render(item, f.Name)));
        }

        private float[] GenerateEmbedding(string text)
        {
            var payload = new { model = ModelName, prompt = text };
            var jsonPayload = JsonConvert.SerializeObject(payload);

            var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var response = _httpClient.PostAsync(OllamaEmbedUrl, httpContent).Result;

            if (!response.IsSuccessStatusCode)
                return null;

            var responseContent = response.Content.ReadAsStringAsync().Result;
            dynamic result = JsonConvert.DeserializeObject(responseContent);

            IList<float> embedding = ((IEnumerable<dynamic>)result.embedding).Select(v => (float)v).ToList();

            return embedding.ToArray();
        }

    }
}
