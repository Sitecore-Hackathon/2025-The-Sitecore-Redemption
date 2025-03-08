using SitecoreRedemption.Feature.Chatbot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SitecoreRedemption.Feature.Chatbot.Repositories
{
    public class GraphQLEmbeddingRepository : IChatbotEmbeddingRepository
    {
        public void SaveEmbedding(ChatbotEmbedding embedding)
        {
            // Future implementation for GraphQL-based embedding saving
            throw new NotImplementedException();
        }

        public IList<ChatbotEmbedding> GetEmbeddings()
        {
            // Future implementation for GraphQL-based retrieval
            throw new NotImplementedException();
        }

        public void ClearEmbeddings()
        {
            // Future implementation for clearing GraphQL-based embeddings
            throw new NotImplementedException();
        }

        IList<ChatbotEmbedding> IChatbotEmbeddingRepository.GetEmbeddings()
        {
            throw new NotImplementedException();
        }
    }
}
