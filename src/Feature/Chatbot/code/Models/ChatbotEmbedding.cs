using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SitecoreRedemption.Feature.Chatbot.Models
{
    public class ChatbotEmbedding
    {
        public string ItemId { get; set; }
        public string ItemPath { get; set; }
        public float[] Vector { get; set; }
        public string ContentExcerpt { get; set; }
        public string Url { get; set; }
        public string ModelName { get; set; }
    }
}
