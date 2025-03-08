using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data;

namespace SitecoreRedemption.Feature.Chatbot
{
  public struct Templates
  {

        public struct ChatbotConfiguration
        {
            public static readonly ID Id = new ID("{139EC5B2-7A94-44E0-9091-EFFA98D29A9A}");

            public struct Fields
            {
                public static readonly ID BrandPrompt = new ID("{B2D8969A-21FE-44F8-BB89-611230B104F1}");
                public static readonly ID Greeting = new ID("{C89B2A5C-FDB5-4978-8EB8-0D9CC28E6CEF}");
                public static readonly ID ContentRoots = new ID("{03E2DC78-0508-4701-9DD9-F47F6EB7FDD7}");
                public static readonly ID IncludedTemplates = new ID("{BB3F194E-20C1-4AEE-ADDD-D835D8037A94}");
                public static readonly ID ExcludedTemplates = new ID("{729CE10F-730E-485E-A843-2F9A2250BF65}");
                public static readonly ID NoAnswer = new ID("{B1572F5B-5E94-4284-805F-44840FE584C9}");
            }
        }
    }
}