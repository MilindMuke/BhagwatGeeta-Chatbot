# Bhagavad Gita Chatbot

## Concept & Purpose
This project is a modern web-based chatbot designed to answer questions about the Bhagavad Gita in English. It leverages Natural Language Processing (NLP) and curated Bhagavad Gita datasets to provide concise, spiritual, and context-aware responses. The chatbot aims to make the wisdom of the Bhagavad Gita accessible and interactive for users seeking guidance, knowledge, or philosophical insights.

## Features
- Chatbot UI: Responsive, modern, and user-friendly interface
- English support
- Context-aware, concise, and spiritual answers
- Multi-source answer synthesis (Q&A, chapters, commentary, glossary, keywords)
- Deduplication and paraphrasing for non-repetitive responses
- Keyword preservation from user queries
- Edge case handling for ambiguous, short, or compound questions
- Custom keyword matching and synonym normalization
- AWS Comprehend integration for NLP (language/key phrase detection)
- Null-safe and robust backend logic

## Tech Stack
- ASP.NET Core MVC (C#)
- HTML, CSS, JavaScript (Frontend)
- AWS Comprehend (NLP)
- JSON-based Bhagavad Gita datasets

## How to Run
1. Ensure .NET Core SDK is installed.
2. Clone the repository and navigate to the project folder.
3. Run the following command in PowerShell:
   ```powershell
   dotnet run
   ```
4. Open your browser and go to `http://localhost:5098` (or the configured port).

## Folder Structure
- `LocalData/` - Bhagavad Gita JSON data and training datasets
- `Services/` - NLP, answer generation, and data handling logic
- `Controllers/` - MVC controllers for chatbot and home page
- `Models/` - Data models for requests and error handling
- `Views/` - Razor views for UI
- `wwwroot/` - Static files (HTML, CSS, JS, images)

## How Does It Work?
1. User submits a question via the chatbot UI.
2. The backend detects language and key phrases using AWS Comprehend.
3. Intent and question type are determined using custom logic and keyword matching.
4. Relevant content is searched across all Bhagavad Gita datasets (Q&A, chapters, commentary, glossary, keywords).
5. The answer is synthesized, paraphrased, deduplicated, and returned in a concise, spiritual tone, preserving important keywords.
6. Edge cases and ambiguous queries are handled gracefully.

## Scope for Improvement & Scalability
- Add support for more languages (e.g., Hindi, Sanskrit)
- Integrate advanced NLP models (e.g., AWS Lex, GPT)
- Expand training datasets for broader coverage
- Add user authentication and personalized history
- Deploy as a scalable cloud service (e.g., AWS Elastic Beanstalk, Azure App Service)
- Mobile app integration
- Real-time analytics and feedback loop for continuous improvement

## Services Used
- AWS Comprehend (language/key phrase detection)

## License
MIT
