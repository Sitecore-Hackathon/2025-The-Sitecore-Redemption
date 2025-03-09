using SitecoreRedemption.Feature.Chatbot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SitecoreRedemption.Feature.Chatbot.Repositories
{
    public interface IChatbotEmbeddingRepository
    {
        void SaveEmbedding(ChatbotEmbedding embedding);
        IList<ChatbotEmbedding> GetEmbeddings();
        void ClearEmbeddings();
    }   
}
