using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;
using Newtonsoft.Json;
using SitecoreRedemption.Feature.Chatbot.Models;

namespace SitecoreRedemption.Feature.Chatbot.Repositories
{
    public class JsonEmbeddingRepository : IChatbotEmbeddingRepository
    {
        private static readonly string FilePath = HostingEnvironment.MapPath("~/App_Data/ChatbotEmbeddings/embeddings.json");

        public void SaveEmbedding(ChatbotEmbedding embedding)
        {
            var embeddings = GetEmbeddings();
            embeddings.Add(embedding);
            SaveToFile(embeddings);
        }

        public IList<ChatbotEmbedding> GetEmbeddings()
        {
            if (!File.Exists(FilePath))
                return new List<ChatbotEmbedding>();

            var json = File.ReadAllText(FilePath);
            return JsonConvert.DeserializeObject<List<ChatbotEmbedding>>(json) ?? new List<ChatbotEmbedding>();
        }

        public void ClearEmbeddings()
        {
            SaveToFile(new List<ChatbotEmbedding>());
        }

        private void SaveToFile(IList<ChatbotEmbedding> embeddings)
        {
            var json = JsonConvert.SerializeObject(embeddings, Formatting.Indented);
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            File.WriteAllText(FilePath, json);
        }
    }
}
