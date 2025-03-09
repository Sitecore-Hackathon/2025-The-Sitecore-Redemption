![Hackathon Logo](docs/images/hackathon.png?raw=true "Hackathon Logo")
# Sitecore Hackathon 2025

## Team name
⟹ The Sitecore Redemption


## Category
⟹ Free for all - AI module

## Description
⟹ A Sitecore Content-drive AI Chatbot

  - The module exposes a AI Chatbot which bases its reponses on the content available in Sitecore.
  - For example, in this demo, there are product details about bicycles.
    - The chatbot can answer questions about the bikes based on the data found in Sitecore using conversational speech.


## Video link
⟹ Provide a video highlighing your Hackathon module submission and provide a link to the video. You can use any video hosting, file share or even upload the video to this repository. _Just remember to update the link below_

⟹ [Replace this Video link](#video-link)

## Pre-requisites and Dependencies

⟹ Does your module rely on other Sitecore modules or frameworks?

- Ollama (Local LLM Server) 



## Installation instructions


## ✅ 1. Install & Set Up Ollama (Local LLM Server)

Ollama is used for **processing AI responses** using a local LLM model.

### 🔹 Step 2.1: Install Ollama

1. Download Ollama from: [https://ollama.com/download](https://ollama.com/download).
2. Run the installer and follow the on-screen instructions.
3. Verify installation by running:
   ```sh
   ollama --version

4. Upon install of Ollama, the server should alredy be started (check the task bar).  If the service is not running, execute
   ```sh
   ollama serve

5.  Download the required AI model
   ```sh
   ollama pull nomic-embed-text
```
## ✅ 2. Install the Sitecore Chatbot Module

1. Install THIS SITECORE PACKAGE (/package/chatbot.zip)
2. Publish Site


### Configuration

## ✅ 1. Generate the LLM model
1. In Sitecore, navigate to: /sitecore/content/Global/Chatbots/Primary Chatbot
2. From the "Developer" ribbon in the content editor, click the "Generate Chatbot Embeddings". This might take a few minutes to complete.


_Remove this subsection if your entry does not require any configuration that is not fully covered in the installation instructions already_

## Usage instructions

## ✅ 1. Run the Chat bot
1. Navigate to https://<yourlocalhost>/chatbot
2. Chatbot should appear.
3. You can ask questions like:
    1. Give me a list of your ebikes
    2. What types of tires does the Superfoxy bike have?


## ✅ 2. Experiment with the prompt settings
1. You can explore with the prompt on this item /sitecore/content/Global/Chatbots/Primary Chatbot 
2. You can also change other settings.


![Hackathon Logo](docs/images/hackathon.png?raw=true "Hackathon Logo")

You can embed images of different formats too:

![Deal With It](docs/images/deal-with-it.gif?raw=true "Deal With It")

And you can embed external images too:

![Random](https://thiscatdoesnotexist.com/)



