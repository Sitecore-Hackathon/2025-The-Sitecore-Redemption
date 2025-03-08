using Newtonsoft.Json;
using Sitecore.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SitecoreRedemption.Feature.Chatbot.Services
{
    public class OllamaApiService
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        private static readonly string OllamaUrl = Settings.GetSetting("Feature.ChatBot.OllamaUrl", "http://localhost:11434/api/generate");
        private static readonly string ModelName = Settings.GetSetting("Feature.ChatBot.ModelName", "mistral");


        public static string GetCompletion(string prompt)
        {
            var payload = new
            {
                model = ModelName,
                prompt = prompt,
                stream = false
            };

            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            var response = _httpClient.PostAsync(OllamaUrl, content).Result;

            if (!response.IsSuccessStatusCode)
                return "Error: Unable to get response from AI service.";

            dynamic result = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
            return result.response;
        }
    }
}
