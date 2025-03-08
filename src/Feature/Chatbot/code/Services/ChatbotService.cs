using HtmlAgilityPack;
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
        private static readonly float ResponseSensitivity = (float)Settings.GetDoubleSetting("Feature.ChatBot.ResponseSensitivity", 0.5);

        public ChatbotService(IChatbotEmbeddingRepository embeddingRepository)
        {
            _embeddingRepository = embeddingRepository;
        }
        public string GenerateAnswer(string question, string brandPrompt)
        {
            var relevantContent = GetRelevantContent(question);

            if (!relevantContent.Any())
                return "I'm sorry, I couldn't find any information on that topic.";

            var contentBuilder = new StringBuilder();
            foreach (var content in relevantContent)
            {
                contentBuilder.AppendLine(content.ContentExcerpt);
            }

            var fullPrompt = $@"
                {brandPrompt}

                Answer ONLY using the provided content below. DO NOT add any information that's not explicitly provided. 
                If no answer is explicitly found, say exactly: 'I'm sorry, I couldn't find any information on that topic.'

                Provided Content:
                {contentBuilder}

                Question: {question}
                Answer:";

            return OllamaApiService.GetCompletion(fullPrompt);
        }


        private IList<ChatbotEmbedding> GetRelevantContent(string query, int topResults = 5)
        {
            var queryEmbedding = GenerateEmbedding(query);
            if (queryEmbedding == null)
                return new List<ChatbotEmbedding>();

            var allEmbeddings = _embeddingRepository.GetEmbeddings();

            var rankedEmbeddings = allEmbeddings
                .Select(e => new
                {
                    Embedding = e,
                    Similarity = CosineSimilarity(queryEmbedding, e.Vector)
                })
                .OrderByDescending(e => e.Similarity)
                .Take(topResults)
                .ToList();

            return rankedEmbeddings
                .Where(e => e.Similarity > ResponseSensitivity)
                .Select(e => e.Embedding)
                .ToList();
        }


        private static float CosineSimilarity(float[] vecA, float[] vecB)
        {
            var dotProduct = 0.0f;
            var magA = 0.0f;
            var magB = 0.0f;

            for (int i = 0; i < vecA.Length; i++)
            {
                dotProduct += vecA[i] * vecB[i];
                magA += vecA[i] * vecA[i];
                magB += vecB[i] * vecB[i];
            }

            if (magA == 0 || magB == 0)
                return 0;

            return dotProduct / ((float)(Math.Sqrt(magA) * Math.Sqrt(magB)));
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
                var structuredText = BuildEmbeddingText(item);
                var embedding = GenerateEmbedding(structuredText);

                if (embedding != null)
                {
                    _embeddingRepository.SaveEmbedding(new ChatbotEmbedding
                    {
                        ItemId = item.ID.ToString(),
                        ItemPath = item.Paths.FullPath,
                        ContentExcerpt = structuredText,
                        Vector = embedding
                    });
                }
            }

            foreach (Item child in item.Children)
            {
                CrawlItemRecursive(child, includedTemplates, excludedTemplates);
            }
        }

        private string BuildEmbeddingText(Item bikeItem)
        {
            var modelName = bikeItem["ModelName"];
            var bikeType = bikeItem["Type"];
            var descriptionHtml = bikeItem["Description"];
            var features = ExtractStructuredFeatures(descriptionHtml);

            var sb = new StringBuilder();
            sb.AppendLine($"Model: {modelName}");
            sb.AppendLine($"Type: {bikeType}");

            foreach (var feature in features)
            {
                sb.AppendLine($"{feature.Key}: {feature.Value}");
            }

            return sb.ToString();
        }



        private Dictionary<string, string> ExtractStructuredFeatures(string htmlDescription)
        {
            var features = new Dictionary<string, string>();
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlDescription);

            var featureNodes = doc.DocumentNode.SelectNodes("//div[@class='features__item']//div[@class='text']");

            if (featureNodes != null)
            {
                foreach (var node in featureNodes)
                {
                    var titleNode = node.SelectSingleNode(".//p[strong]");

                    if (titleNode != null)
                    {
                        // Explicitly find the next sibling <p> node after the titleNode
                        var descriptionNode = titleNode.SelectSingleNode("following-sibling::p[1]");

                        if (descriptionNode != null)
                        {
                            var key = titleNode.InnerText.Trim();
                            var value = descriptionNode.InnerText.Trim();

                            if (!features.ContainsKey(key))
                            {
                                features.Add(key, value);
                            }
                        }
                    }
                }
            }

            return features;
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
            var payload = new { model = "nomic-embed-text", prompt = text };
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
