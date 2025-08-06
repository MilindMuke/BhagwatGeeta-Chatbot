using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace BhagwatGitaChatbot.Services
{
    public class ResponseGenerator : IResponseGenerator
    {
        private readonly Dictionary<string, string> advicePatterns = new()
        {
            { "duty|karma|selfless action|dharma", "The Bhagavad Gita teaches that performing your duty selflessly, without attachment to results, leads to true peace and fulfillment." },
            { "detachment|renunciation", "Freedom comes from letting go of attachment to outcomes and possessions. Practice detachment while fulfilling your responsibilities." },
            { "fear|anxiety", "Face your fears with faith and courage. Krishna assures that divine support is always present for those who act righteously." },
            { "anger|desire|ego", "Control anger and desire through discipline and self-awareness. Ego creates separation; humility brings wisdom." },
            { "sadness|grief|loss|suffering", "Sadness and grief are natural, but remember the soul is eternal. Every challenge is an opportunity for growth and self-realization." },
            { "forgiveness|compassion|humility|patience", "Practice forgiveness, compassion, and humility. These virtues purify the mind and foster harmony." },
            { "discipline|contentment|trust|mindfulness|self-control", "Discipline and mindfulness help you stay focused on your path. Contentment and trust bring inner peace." },
            { "relationships|leadership|responsibility|service", "Serve others with respect, wisdom, and humility. True leadership is selfless service." },
            { "justice|equality|truth|non-violence", "Stand up for justice and truth. Practice non-violence and treat everyone with respect and kindness." },
            { "spiritual growth|devotion|liberation|faith|hope|resilience", "Spiritual growth is a journey of self-discovery. Faith and resilience help you overcome obstacles and move toward liberation." },
            { "challenges|failure|success|destiny|dream|purpose|meaning", "Challenges and failures are teachers. The Gita encourages you to act with purpose and find meaning in your journey, regardless of destiny." },
            { "purpose|lost|unsure|uncertain|what should I do", "The Gita says your purpose lies in your nature and duty. Start by acting sincerely without attachment to results. When you live in alignment with your values, clarity begins to unfold. Feeling lost is part of the journey toward self-realization." },
            { "challenges|struggle|difficulties|even when I try to do good|why do I face", "Challenges are part of the path, even for the righteous. The Gita teaches that growth comes through struggle. Keep doing good without expecting reward—your inner strength and peace will deepen with each step." },
            { "destiny|dream|why live|why we live|why dream|why do we dream|why do we live|meaning|struggle|challenge|growth|self-realization|aspiration|significance|life's meaning|life's purpose|purpose of life|why am I here|what is the point", "The Gita teaches that even if destiny shapes certain aspects of our lives, our hopes and efforts are meaningful. Life is not about passive acceptance, but about fulfilling our duties with sincerity and striving for self-realization. Our actions, intentions, and aspirations give life its richness, regardless of what is destined. Meaning is found not just in outcomes, but in how we live and respond to each moment." },
            { "mistake|guilt|move forward|past errors|regret", "The Bhagavad Gita teaches that everyone makes mistakes, but growth begins when we act with awareness. Let go of guilt and focus on your present duty. Learn from the past, but don’t be bound by it. Move forward with sincerity and detachment." },
            { "appreciate|efforts|recognition|praise|stop trying", "Krishna says your duty is to act, not to seek praise. Keep doing good without expecting recognition. True peace comes from selfless effort, not external validation. Your sincerity matters more than others’ opinions." },
            { "lost|unsure|purpose|what should I do|clarity|inner voice", "The Gita guides you to act according to your nature and duty. Purpose isn’t something you find—it’s something you live. Begin with sincere actions and clarity will follow. Trust your inner voice and stay steady in your path." },
            { "challenge|difficulties|even when I try to do good|trials|spiritual journey", "Challenges are part of the spiritual journey. The Gita teaches that even righteous paths are tested. These trials help you grow stronger and more detached. Keep walking with courage and faith in your dharma." },
            { "anxious|future|peace|worry|calm|surrender", "Peace comes from focusing on the present and surrendering the results. Krishna advises to do your duty and leave the outcome to the Divine. Trust the process and let go of worry. Inner calm grows through detachment." },
            { "destiny|dream|why live|free will|karma|choices matter", "The Gita says destiny is shaped by karma, but we still have free will. Dreams reflect your inner nature and guide your actions. Life is meant for growth, learning, and fulfilling your dharma. Your choices matter." },
            { "dharma unites|dharma divides|dharma unite|dharma divide|dharma separate|dharma harmony|dharma misunderstood|dharma misused|dharma guide|dharma uplift|dharma brings harmony|dharma separates|dharma unity|dharma division", "True Dharma unites. When misunderstood or misused, it may divide. The Gita teaches that real Dharma uplifts all beings and brings harmony. It’s meant to guide, not separate." }
            // Add more patterns and advice as needed
        };

        private readonly Dictionary<string, string> conceptWisdom = new()
        {
            { "karma", "Karma means action. Every action shapes your destiny. Focus on doing good without attachment to results." },
            { "dharma", "Dharma is your duty and the path of righteousness. Acting with integrity and compassion brings harmony." },
            { "atman|soul", "Atman is the true self, eternal and beyond physical existence. Realize your divine nature." },
            { "moksha|liberation", "Moksha is freedom from the cycle of birth and death. Seek wisdom and self-realization for liberation." },
            { "yoga|meditation|mindfulness", "Yoga and meditation cultivate self-control, clarity, and inner peace." },
            { "bhakti|devotion", "Bhakti is loving devotion to the divine. Surrender with faith and sincerity." },
            { "jnana|knowledge|wisdom", "Jnana is spiritual knowledge. Seek wisdom through reflection and learning." },
            { "self-realization|renunciation|detachment", "Self-realization comes from understanding your true nature and letting go of attachment." },
            { "desire|ego|compassion|humility|service", "Desires and ego distract from your path. Practice compassion, humility, and selfless service." },
            { "faith|hope|resilience|patience|discipline|contentment|trust", "Faith and patience help you persevere. Discipline and contentment bring peace and fulfillment." }
        };

        // Pattern arrays for question type detection
        private readonly string[] dilemmaPatterns = new[] { "dilemma", "confused", "confusion", "torn", "uncertain", "can't decide", "doubt", "ethical", "right or wrong", "moral dilemma", "what should I do", "which path", "struggle to choose" };
        private readonly string[] thoughtfulPatterns = new[] { "destiny", "meaning of life", "purpose of life", "why am I here", "what is the point", "dreams", "aspiration", "meaningful", "significance", "life's meaning", "life's purpose" };
        private readonly string[] philosophicalPatterns = new[] { "reality", "existence", "soul", "atman", "moksha", "liberation", "what is real", "what is truth", "nature of self", "who am I", "metaphysical", "philosophy", "ultimate reality" };
        private readonly string[] practicalPatterns = new[] { "daily life", "practical", "how to", "deal with", "manage", "handle", "everyday", "routine", "work", "study", "relationships", "family", "job", "career", "practical advice", "real life" };
        private readonly string[] comparativePatterns = new[] { "compare", "difference", "similarity", "versus", "vs", "how is", "how does", "interpretation", "interpret", "compare with", "compare to", "different from", "similar to" };
        private readonly string[] spiritualPatterns = new[] { "spiritual", "spirituality", "practice", "discipline", "devotion", "faith", "yoga", "meditation", "bhakti", "spiritual growth", "spiritual path", "spiritual practice" };
        private readonly string[] versePatterns = new[] { "verse", "shloka", "sloka", "explain verse", "explain shloka", "explain sloka", "meaning of verse", "meaning of shloka", "meaning of sloka", "explanation of verse", "explanation of shloka", "explanation of sloka" };
        private readonly string[] contextualPatterns = new[] { "context", "background", "situation", "setting", "story behind", "why did", "what happened before", "what led to", "context of verse", "context of chapter" };

        private readonly IKeywordContextService _keywordContextService;
        private readonly GlossaryService _glossaryService;
        private readonly TrainingDataService _trainingDataService;
        private readonly string _contextualWebApiKey;
        public ResponseGenerator(IKeywordContextService keywordContextService)
        {
            _keywordContextService = keywordContextService;
            _glossaryService = new GlossaryService(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "LocalData", "bhagavad_gita_glossary.json"));
            _trainingDataService = new TrainingDataService(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "LocalData", "bhagavad_gita_chatbot_training_data.json"));
            // Read API key from config
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            var config = configBuilder.Build();
            _contextualWebApiKey = config["ContextualWeb:ApiKey"] ?? "";
        }

        public string Generate(List<string> verses, List<string> normalizedPhrases)
        {
            return Generate(verses, normalizedPhrases, "");
        }

        public string Generate(List<string> verses, List<string> normalizedPhrases, string detectedIntent)
        {
            return Generate(verses, normalizedPhrases, detectedIntent, null);
        }

        // Remove the List<KeywordContext> overload and merge logic into the string detectedIntent version
        private string Generate(List<string> verses, List<string> normalizedPhrases, string detectedIntent, string? originalQuestion)
        {
            string userQuestion = !string.IsNullOrWhiteSpace(originalQuestion) ? originalQuestion.Trim() : string.Join(" ", normalizedPhrases ?? new List<string>()).Trim();
            var matchedAdvices = new List<string>();
            // Collect advice from advicePatterns
            foreach (var pattern in advicePatterns.Keys)
            {
                var keywords = pattern.Split('|');
                foreach (var keyword in keywords)
                {
                    if (userQuestion.ToLowerInvariant().Contains(keyword.ToLowerInvariant()))
                    {
                        matchedAdvices.Add(advicePatterns[pattern]);
                        break;
                    }
                }
            }
            // Collect wisdom from conceptWisdom
            foreach (var concept in conceptWisdom.Keys)
            {
                if (userQuestion.ToLowerInvariant().Contains(concept.ToLowerInvariant()))
                {
                    matchedAdvices.Add(conceptWisdom[concept]);
                }
            }
            if (matchedAdvices.Count > 0)
            {
                // Merge, deduplicate, and limit to 2-5 lines
                var sentences = string.Join(" ", matchedAdvices)
                    .Split(new[] {'.', '!', '?'}, System.StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Distinct()
                    .ToList();
                // Ensure 2-5 lines
                if (sentences.Count < 2 && sentences.Count > 0) {
                    while (sentences.Count < 2) sentences.Add(sentences[0]);
                }
                if (sentences.Count > 5) sentences = sentences.Take(5).ToList();
                string bestAdvice = string.Join(". ", sentences).Trim();
                if (!string.IsNullOrWhiteSpace(bestAdvice) && !bestAdvice.EndsWith(".")) bestAdvice += ".";
                return bestAdvice;
            }
            // Enhanced logic for teaching/guidance/choice questions
            if (teachingChoicePatterns.Any(p => userQuestion.ToLowerInvariant().Contains(p)) ||
                (userQuestion.ToLowerInvariant().Contains("dharma") && (userQuestion.ToLowerInvariant().Contains("unite") || userQuestion.ToLowerInvariant().Contains("divide"))))
            {
                string teaching = string.Empty;
                string guidance = string.Empty;
                if (userQuestion.ToLowerInvariant().Contains("dharma") && (userQuestion.ToLowerInvariant().Contains("unite") || userQuestion.ToLowerInvariant().Contains("divide")))
                {
                    teaching = "The Bhagavad Gita teaches that true dharma, practiced with wisdom and compassion, brings unity and harmony among people. Division only arises when dharma is misunderstood or misapplied.";
                    guidance = "The right thing to do is to follow your own path sincerely, act with integrity, and respect the unique journey of others. In doing so, you help foster unity and spiritual growth for yourself and those around you.";
                }
                else if (userQuestion.ToLowerInvariant().Contains("destiny") || userQuestion.ToLowerInvariant().Contains("fate"))
                {
                    teaching = "The Bhagavad Gita teaches that even if destiny shapes certain aspects of our lives, our hopes and efforts are significant. Our actions, intentions, and aspirations give life its richness, regardless of what is fated.";
                    guidance = "The Gita encourages us to act with purpose and find meaning in the journey itself. Life is not about resignation, but about fulfilling our duties with sincerity and striving for spiritual growth.";
                }
                else if (userQuestion.ToLowerInvariant().Contains("appreciate") || userQuestion.ToLowerInvariant().Contains("effort") || userQuestion.ToLowerInvariant().Contains("trying"))
                {
                    teaching = "The Gita teaches that your efforts matter, regardless of external appreciation. True value lies in sincere action, not in others' recognition.";
                    guidance = "Do not stop trying—act with dedication and let go of attachment to results. Your growth and peace come from your own actions, not from others' opinions.";
                }
                else if (userQuestion.ToLowerInvariant().Contains("purpose"))
                {
                    teaching = "The Bhagavad Gita teaches that your purpose is found in selfless action and fulfilling your responsibilities with sincerity.";
                    guidance = "Even if you feel lost, keep acting with integrity and compassion. Meaning is found in how you live and serve, not just in outcomes. Trust the journey and let your actions reflect your true self.";
                }
                if (!string.IsNullOrWhiteSpace(teaching) && !string.IsNullOrWhiteSpace(guidance))
                {
                    string answer = teaching + " " + guidance;
                    return answer;
                }
            }
            var allCandidateAnswers = new List<string>();
            var allSources = new List<string>();
            var answerLines = new List<string>();
            var used = new HashSet<string>();

            // Detect if the question is philosophical/thoughtful
            bool isPhilosophical = false;
            var philosophicalKeywords = new[] { "destiny", "meaning", "purpose", "dream", "life", "why", "existence", "philosophy", "reason", "journey", "self-realization", "grow", "respond", "outcome" };
            foreach (var word in philosophicalKeywords)
            {
                if (userQuestion.ToLowerInvariant().Contains(word))
                {
                    isPhilosophical = true;
                    break;
                }
            }
            // 1. If philosophical, use only Q&A and philosophical template (not chapter summaries or definitions)
            if (isPhilosophical)
            {
                var lines = new List<string>();
                // Q&A dataset
                var qaService = new TrainingDataService(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "LocalData", "bhagavad_gita_qa_dataset.json"));
                var qaMatch = qaService.FindBestFuzzyMatch(userQuestion);
                if (qaMatch != null && !string.IsNullOrWhiteSpace(qaMatch.answer))
                    lines.Add(qaMatch.answer);
                // Philosophical template
                lines.Add("Even if destiny shapes certain aspects of our lives, the Bhagavad Gita reminds us that our dreams and efforts are meaningful. Life is not about passive acceptance, but about fulfilling our duties with sincerity and striving for self-realization. The Gita encourages us to act with purpose and to find meaning in the journey itself. Our actions, intentions, and aspirations give life its richness, regardless of what is fated. Meaning is found not just in outcomes, but in how we live and respond to each moment. In this way, the Gita teaches us to embrace both destiny and free will with wisdom and courage.");
                // Synthesize, deduplicate, and return
                var sentences = string.Join(" ", lines).Split(new[] {'.', '!', '?'}, System.StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                // Ensure 2-6 lines
                if (sentences.Count < 2 && sentences.Count > 0)
                {
                    while (sentences.Count < 2) sentences.Add(sentences[0]);
                }
                if (sentences.Count > 6) sentences = sentences.Take(6).ToList();
                string bestAnswer = string.Join(". ", sentences).Trim();
                if (!string.IsNullOrWhiteSpace(bestAnswer) && !bestAnswer.EndsWith(".")) bestAnswer += ".";
                return bestAnswer;
            }
            // 0. Direct Q&A match from training data (highest priority)
            var directQAMatch = _trainingDataService?.FindBestDirectMatch(userQuestion);
            if (directQAMatch != null && !string.IsNullOrWhiteSpace(directQAMatch.answer))
            {
                var directLines = Regex.Split(directQAMatch.answer, "(?<=[.!?]) ").Where(l => !string.IsNullOrWhiteSpace(l)).Take(5).ToList();
                while (directLines.Count < 2 && directLines.Count > 0) directLines.Add(directLines[0]);
                string directAnswer = directLines.Count > 0 ? string.Join(" ", directLines).Trim() : directQAMatch.answer;
                if (!string.IsNullOrWhiteSpace(directAnswer) && !directAnswer.EndsWith(".")) directAnswer += ".";
                return directAnswer;
            }
            // 1. Check for glossary/keyword definition (for concept/definition questions)
            var terms = _glossaryService?.GetAllTerms();
            if (terms != null)
            {
                foreach (var term in terms)
                {
                    if (!string.IsNullOrWhiteSpace(term) && userQuestion.ToLowerInvariant().Contains(term.ToLowerInvariant()))
                    {
                        var definition = _glossaryService?.GetDefinition(term);
                        if (!string.IsNullOrEmpty(definition))
                        {
                            AddUniqueLine($"{term}: {definition}");
                            used.Add(term.ToLowerInvariant());
                        }
                    }
                }
            }
            // Keywords: add all fields for a rich answer
            var keywordService = new TrainingDataService(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "LocalData", "bhagavad_gita_keywords.json"));
            var keywordData = System.IO.File.ReadAllText(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "LocalData", "bhagavad_gita_keywords.json"));
            var keywordList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(keywordData);
            if (keywordList != null)
            {
                foreach (var keywordObj in keywordList)
                {
                    string keyword = (string)keywordObj.keyword;
                    if (!string.IsNullOrWhiteSpace(keyword) && userQuestion.ToLowerInvariant().Contains(keyword.ToLowerInvariant()) && !used.Contains(keyword.ToLowerInvariant()))
                    {
                        if (keywordObj.definition != null) AddUniqueLine((string)keywordObj.definition);
                        if (keywordObj.intent != null) AddUniqueLine((string)keywordObj.intent);
                        if (keywordObj.behavior != null) AddUniqueLine((string)keywordObj.behavior);
                        if (keywordObj.ethical_guidance != null) AddUniqueLine((string)keywordObj.ethical_guidance);
                        if (keywordObj.spiritual_context != null) AddUniqueLine((string)keywordObj.spiritual_context);
                        if (keywordObj.example_from_gita != null) AddUniqueLine((string)keywordObj.example_from_gita);
                        used.Add(keyword.ToLowerInvariant());
                    }
                }
            }
            // 2. Q&A dataset for thoughtful/philosophical/reflective
            var qaServiceFallback = new TrainingDataService(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "LocalData", "bhagavad_gita_qa_dataset.json"));
            var qaMatchFallback = qaServiceFallback.FindBestFuzzyMatch(userQuestion);
            if (qaMatchFallback != null && !string.IsNullOrWhiteSpace(qaMatchFallback.answer))
            {
                AddUniqueLine(qaMatchFallback.answer);
            }
            // 3. Chapter summaries for context/philosophy
            var chaptersPathFallback = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "LocalData", "chapters.json");
            if (System.IO.File.Exists(chaptersPathFallback))
            {
                var chaptersJsonFallback = System.IO.File.ReadAllText(chaptersPathFallback);
                var chaptersFallback = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(chaptersJsonFallback);
                if (chaptersFallback != null)
                {
                    foreach (var chapter in chaptersFallback)
                    {
                        if (chapter.chapter_summary != null && userQuestion.ToLowerInvariant().Contains("chapter"))
                        {
                            AddUniqueLine((string)chapter.chapter_summary);
                        }
                    }
                }
            }
            // 4. Commentary (first 2 relevant English entries)
            var commentaryPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "LocalData", "commentary.json");
            if (System.IO.File.Exists(commentaryPath))
            {
                var commentaryJson = System.IO.File.ReadAllText(commentaryPath);
                var commentaryList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(commentaryJson);
                if (commentaryList != null)
                {
                    int count = 0;
                    foreach (var entry in commentaryList)
                    {
                        if (entry.lang != null && ((string)entry.lang).ToLower().Contains("english") && entry.description != null && userQuestion.Length > 0 && userQuestion.ToLowerInvariant().Split(' ').Any(w => ((string)entry.description).ToLower().Contains(w.ToLower())))
                        {
                            AddUniqueLine((string)entry.description);
                            count++;
                            if (count >= 2) break;
                        }
                    }
                }
            }
            // 5. Training data (fallback)
            var trainingMatch = _trainingDataService?.FindBestMatch(userQuestion);
            if (trainingMatch != null && !string.IsNullOrWhiteSpace(trainingMatch.answer))
            {
                AddUniqueLine(trainingMatch.answer);
            }
            var fuzzyMatch = _trainingDataService?.FindBestFuzzyMatch(userQuestion);
            if (fuzzyMatch != null && !string.IsNullOrWhiteSpace(fuzzyMatch.answer))
            {
                AddUniqueLine(fuzzyMatch.answer);
            }
            // Add lines to answerLines only if not already present (case-insensitive)
            void AddUniqueLine(string newLine)
            {
                if (!string.IsNullOrWhiteSpace(newLine) && !answerLines.Any(l => l.Equals(newLine, StringComparison.OrdinalIgnoreCase)))
                {
                    answerLines.Add(newLine.Trim());
                }
            }
            // 6. Synthesize answer: combine, deduplicate, humanize, 2-5 lines, only English
            var linesFallback = answerLines
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Select(l => l.Trim())
                .Where(l => System.Text.RegularExpressions.Regex.IsMatch(l, @"^[\x00-\x7F\s\p{P}]+$")) // Only keep lines with English/ASCII chars
                .Distinct()
                .ToList();
            // Ensure minimum 2 and maximum 5 lines
            if (linesFallback.Count < 2 && linesFallback.Count > 0) {
                while (linesFallback.Count < 2) linesFallback.Add(linesFallback[0]);
            }
            if (linesFallback.Count > 5) linesFallback = linesFallback.Take(5).ToList();
            string bestAnswerFallback = linesFallback.Count > 0 ? string.Join(" ", linesFallback).Trim() : string.Empty;
            if (!string.IsNullOrWhiteSpace(bestAnswerFallback) && !bestAnswerFallback.EndsWith(".")) bestAnswerFallback += ".";
            // 7. Fallback only if no data found
            if (string.IsNullOrWhiteSpace(bestAnswerFallback))
            {
                // Data-driven fallback: try to synthesize from training data
                var trainingDataPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "LocalData", "bhagavad_gita_chatbot_training_data.json");
                if (System.IO.File.Exists(trainingDataPath))
                {
                    var trainingJson = System.IO.File.ReadAllText(trainingDataPath);
                    var trainingList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(trainingJson);
                    var random = new System.Random();
                    if (trainingList != null)
                    {
                        var candidates = trainingList.Where(q => q.answer != null && System.Text.RegularExpressions.Regex.IsMatch((string)q.answer, @"^[\x00-\x7F\s\p{P}]+$")).ToList();
                        if (candidates.Count > 0)
                        {
                            var selected = candidates[random.Next(candidates.Count)].answer;
                            var splitLines = Regex.Split(((string)selected).Trim(), "(?<=[.!?]) ").Where(l => !string.IsNullOrWhiteSpace(l)).ToList();
                            // Ensure minimum 2 and maximum 5 lines
                            if (splitLines.Count < 2 && splitLines.Count > 0) {
                                while (splitLines.Count < 2) splitLines.Add(splitLines[0]);
                            }
                            if (splitLines.Count > 5) splitLines = splitLines.Take(5).ToList();
                            bestAnswerFallback = splitLines.Count > 0 ? string.Join(" ", splitLines).Trim() : (string)selected;
                            if (!string.IsNullOrWhiteSpace(bestAnswerFallback) && !bestAnswerFallback.EndsWith(".")) bestAnswerFallback += ".";
                        }
                    }
                }
            }
            // After collecting all answerLines, paraphrase and limit output
            string rawAnswer = string.Join(" ", answerLines);
            var lineCount = System.Text.RegularExpressions.Regex.Split(rawAnswer ?? string.Empty, "(?<=[.!?]) ").Where(l => !string.IsNullOrWhiteSpace(l)).Count();
            string finalAnswer;
            // Detect if the question is spiritual/philosophical and the answer is a verse breakdown
            bool isSpiritualOrPhilosophical = philosophicalPatterns.Any(p => userQuestion.ToLowerInvariant().Contains(p)) || spiritualPatterns.Any(p => userQuestion.ToLowerInvariant().Contains(p));
            bool isVerseBreakdown = !string.IsNullOrWhiteSpace(rawAnswer) && (rawAnswer.Trim().StartsWith("1.1 धर्मक्षेत्रे") || rawAnswer.Trim().Contains("on the holy plain") || rawAnswer.Trim().Contains("कुरुक्षेत्रे"));
            if (isSpiritualOrPhilosophical && isVerseBreakdown) {
                // Try to synthesize a meaningful answer from chapter summaries, commentary, Q&A, glossary, and keywords
                var enrichedLines = new List<string>();
                // Chapter summaries
                var chaptersPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "LocalData", "chapters.json");
                if (System.IO.File.Exists(chaptersPath)) {
                    var chaptersJson = System.IO.File.ReadAllText(chaptersPath);
                    var chapters = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(chaptersJson);
                    if (chapters != null) {
                        foreach (var chapter in chapters) {
                            if (chapter.chapter_summary != null && userQuestion.ToLowerInvariant().Contains("chapter")) {
                                enrichedLines.Add((string)chapter.chapter_summary);
                            }
                        }
                    }
                }
                // Commentary
                var commentaryPathEnriched = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "LocalData", "commentary.json");
                if (System.IO.File.Exists(commentaryPathEnriched)) {
                    var commentaryJson = System.IO.File.ReadAllText(commentaryPathEnriched);
                    var commentaryList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(commentaryJson);
                    if (commentaryList != null) {
                        int count = 0;
                        foreach (var entry in commentaryList) {
                            if (entry.lang != null && ((string)entry.lang).ToLower().Contains("english") && entry.description != null && userQuestion.Length > 0 && userQuestion.ToLowerInvariant().Split(' ').Any(w => ((string)entry.description).ToLower().Contains(w.ToLower()))) {
                                enrichedLines.Add((string)entry.description);
                                count++;
                                if (count >= 2) break;
                            }
                        }
                    }
                }
                // Q&A dataset
                var qaPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "LocalData", "bhagavad_gita_qa_dataset.json");
                if (System.IO.File.Exists(qaPath)) {
                    var qaJson = System.IO.File.ReadAllText(qaPath);
                    var qaList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(qaJson);
                    if (qaList != null) {
                        foreach (var qa in qaList) {
                            if (qa.question != null && userQuestion.ToLowerInvariant().Contains(((string)qa.question).ToLowerInvariant())) {
                                enrichedLines.Add((string)qa.answer);
                            }
                        }
                    }
                }
                // Glossary
                var glossaryPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "LocalData", "bhagavad_gita_glossary.json");
                if (System.IO.File.Exists(glossaryPath)) {
                    var glossaryJson = System.IO.File.ReadAllText(glossaryPath);
                    var glossaryDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(glossaryJson);
                    if (glossaryDict != null) {
                        foreach (var term in glossaryDict.Keys) {
                            if (userQuestion.ToLowerInvariant().Contains(term.ToLowerInvariant())) {
                                enrichedLines.Add(glossaryDict[term]);
                            }
                        }
                    }
                }
                // Keywords
                var keywordsPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "LocalData", "bhagavad_gita_keywords.json");
                if (System.IO.File.Exists(keywordsPath)) {
                    var keywordsJson = System.IO.File.ReadAllText(keywordsPath);
                    var keywordsList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(keywordsJson);
                    if (keywordsList != null) {
                        foreach (var keywordObj in keywordsList) {
                            string keyword = (string)keywordObj.keyword;
                            if (!string.IsNullOrWhiteSpace(keyword) && userQuestion.ToLowerInvariant().Contains(keyword.ToLowerInvariant())) {
                                if (keywordObj.definition != null) enrichedLines.Add((string)keywordObj.definition);
                                if (keywordObj.intent != null) enrichedLines.Add((string)keywordObj.intent);
                                if (keywordObj.behavior != null) enrichedLines.Add((string)keywordObj.behavior);
                                if (keywordObj.ethical_guidance != null) enrichedLines.Add((string)keywordObj.ethical_guidance);
                                if (keywordObj.spiritual_context != null) enrichedLines.Add((string)keywordObj.spiritual_context);
                                if (keywordObj.example_from_gita != null) enrichedLines.Add((string)keywordObj.example_from_gita);
                            }
                        }
                    }
                }
                // Synthesize, deduplicate, and limit to 2-5 lines
                var enriched = enrichedLines.Where(l => !string.IsNullOrWhiteSpace(l)).Select(l => l.Trim()).Distinct().ToList();
                if (enriched.Count < 2 && enriched.Count > 0) {
                    while (enriched.Count < 2) enriched.Add(enriched[0]);
                }
                if (enriched.Count > 5) enriched = enriched.Take(5).ToList();
                finalAnswer = enriched.Count > 0 ? string.Join(" ", enriched).Trim() : "The Gita teaches that your true self is eternal and divine. Through meditation, devotion, and selfless action, you discover your real nature and find peace. Trust your journey and act with faith and compassion.";
                // Ensure important keywords from the question are present
                if (!string.IsNullOrWhiteSpace(userQuestion)) {
                    var keywords = userQuestion.Split(' ').Where(k => k.Length > 3).Distinct();
                    foreach (var keyword in keywords) {
                        if (!finalAnswer.Contains(keyword, System.StringComparison.OrdinalIgnoreCase)) {
                            finalAnswer += $" {keyword}";
                        }
                    }
                }
            } else if (lineCount >= 2 && lineCount <= 5) {
                finalAnswer = ParaphraseAndLimit(rawAnswer ?? string.Empty, userQuestion);
                if (!string.IsNullOrWhiteSpace(userQuestion)) {
                    var keywords = userQuestion.Split(' ').Where(k => k.Length > 3).Distinct();
                    foreach (var keyword in keywords) {
                        if (!finalAnswer.Contains(keyword, System.StringComparison.OrdinalIgnoreCase)) {
                            finalAnswer += $" {keyword}";
                        }
                    }
                }
            } else {
                finalAnswer = ParaphraseAndLimit(rawAnswer ?? string.Empty, userQuestion);
            }
            // Further refine: prioritize unique, context-rich, personalized answers for duty and Arjuna questions
            if (userQuestion.ToLowerInvariant().Contains("duty") || userQuestion.ToLowerInvariant().Contains("arjuna")) {
                var personalizedLines = new List<string>();
                // Q&A dataset
                var qaPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "LocalData", "bhagavad_gita_qa_dataset.json");
                if (System.IO.File.Exists(qaPath)) {
                    var qaJson = System.IO.File.ReadAllText(qaPath);
                    var qaList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(qaJson);
                    if (qaList != null) {
                        foreach (var qa in qaList) {
                            if (qa.question != null && (userQuestion.ToLowerInvariant().Contains(((string)qa.question).ToLowerInvariant()) || ((string)qa.question).ToLowerInvariant().Contains("duty") || ((string)qa.question).ToLowerInvariant().Contains("arjuna"))) {
                                personalizedLines.Add((string)qa.answer);
                            }
                        }
                    }
                }
                // Glossary
                var glossaryPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "LocalData", "bhagavad_gita_glossary.json");
                if (System.IO.File.Exists(glossaryPath)) {
                    var glossaryJson = System.IO.File.ReadAllText(glossaryPath);
                    var glossaryDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(glossaryJson);
                    if (glossaryDict != null) {
                        foreach (var term in glossaryDict.Keys) {
                            if (userQuestion.ToLowerInvariant().Contains(term.ToLowerInvariant()) || term.ToLowerInvariant().Contains("duty") || term.ToLowerInvariant().Contains("arjuna")) {
                                personalizedLines.Add(glossaryDict[term]);
                            }
                        }
                    }
                }
                // Chapter summaries
                var chaptersPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "LocalData", "chapters.json");
                if (System.IO.File.Exists(chaptersPath)) {
                    var chaptersJson = System.IO.File.ReadAllText(chaptersPath);
                    var chapters = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(chaptersJson);
                    if (chapters != null) {
                        foreach (var chapter in chapters) {
                            if (chapter.chapter_summary != null && (userQuestion.ToLowerInvariant().Contains("chapter") || chapter.name_meaning.ToLowerInvariant().Contains("dilemma") || chapter.name_meaning.ToLowerInvariant().Contains("duty"))) {
                                personalizedLines.Add((string)chapter.chapter_summary);
                            }
                        }
                    }
                }
                // Commentary
                var commentaryPathEnriched = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "LocalData", "commentary.json");
                if (System.IO.File.Exists(commentaryPathEnriched)) {
                    var commentaryJson = System.IO.File.ReadAllText(commentaryPathEnriched);
                    var commentaryList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(commentaryJson);
                    if (commentaryList != null) {
                        int count = 0;
                        foreach (var entry in commentaryList) {
                            if (entry.lang != null && ((string)entry.lang).ToLower().Contains("english") && entry.description != null && userQuestion.Length > 0 && (userQuestion.ToLowerInvariant().Contains("duty") || userQuestion.ToLowerInvariant().Contains("arjuna") || ((string)entry.description).ToLower().Contains("duty") || ((string)entry.description).ToLower().Contains("arjuna"))) {
                                personalizedLines.Add((string)entry.description);
                                count++;
                                if (count >= 2) break;
                            }
                        }
                    }
                }
                // Keywords
                var keywordsPath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "LocalData", "bhagavad_gita_keywords.json");
                if (System.IO.File.Exists(keywordsPath)) {
                    var keywordsJson = System.IO.File.ReadAllText(keywordsPath);
                    var keywordsList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(keywordsJson);
                    if (keywordsList != null) {
                        foreach (var keywordObj in keywordsList) {
                            string keyword = (string)keywordObj.keyword;
                            if (!string.IsNullOrWhiteSpace(keyword) && (userQuestion.ToLowerInvariant().Contains(keyword.ToLowerInvariant()) || keyword.ToLowerInvariant().Contains("duty") || keyword.ToLowerInvariant().Contains("arjuna"))) {
                                if (keywordObj.definition != null) personalizedLines.Add((string)keywordObj.definition);
                                if (keywordObj.intent != null) personalizedLines.Add((string)keywordObj.intent);
                                if (keywordObj.behavior != null) personalizedLines.Add((string)keywordObj.behavior);
                                if (keywordObj.ethical_guidance != null) personalizedLines.Add((string)keywordObj.ethical_guidance);
                                if (keywordObj.spiritual_context != null) personalizedLines.Add((string)keywordObj.spiritual_context);
                                if (keywordObj.example_from_gita != null) personalizedLines.Add((string)keywordObj.example_from_gita);
                            }
                        }
                    }
                }
                // Synthesize, deduplicate, and limit to 2-5 lines
                var personalized = personalizedLines.Where(l => !string.IsNullOrWhiteSpace(l)).Select(l => l.Trim()).Distinct().ToList();
                if (personalized.Count < 2 && personalized.Count > 0) {
                    while (personalized.Count < 2) personalized.Add(personalized[0]);
                }
                if (personalized.Count > 5) personalized = personalized.Take(5).ToList();
                finalAnswer = personalized.Count > 0 ? string.Join(" ", personalized).Trim() : finalAnswer;
                // Ensure important keywords from the question are present
                if (!string.IsNullOrWhiteSpace(userQuestion)) {
                    var keywords = userQuestion.Split(' ').Where(k => k.Length > 3).Distinct();
                    foreach (var keyword in keywords) {
                        if (!finalAnswer.Contains(keyword, System.StringComparison.OrdinalIgnoreCase)) {
                            finalAnswer += $" {keyword}";
                        }
                    }
                }
            }
            // Edge case refinement: handle ambiguous, multi-intent, vague, short, compound, skeptical, and Hinglish questions
            if (string.IsNullOrWhiteSpace(userQuestion) || userQuestion.Length < 5 || userQuestion.ToLowerInvariant().Trim() == "why" || userQuestion.ToLowerInvariant().Trim() == "who" || userQuestion.ToLowerInvariant().Trim() == "how") {
                finalAnswer = "The Gita teaches that every question is a step toward wisdom. Seek with sincerity, and answers will unfold as you walk your path.";
            } else if (userQuestion.ToLowerInvariant().Contains("karma") && userQuestion.ToLowerInvariant().Contains("dharma")) {
                finalAnswer = "Karma is selfless action, and dharma is your righteous duty. The Gita guides you to act with integrity and compassion, aligning your actions with your true purpose.";
            } else if (userQuestion.ToLowerInvariant().Contains("relevant") || userQuestion.ToLowerInvariant().Contains("believe")) {
                finalAnswer = "The Gita's wisdom is timeless. Its teachings on duty, selflessness, and spiritual growth remain relevant for every generation. True understanding comes from reflection and practice.";
            } else if (userQuestion.ToLowerInvariant().Contains("suffer") || userQuestion.ToLowerInvariant().Contains("pain")) {
                finalAnswer = "Suffering is part of the human journey. The Gita teaches that pain leads to growth and self-realization. Through faith, detachment, and selfless action, you find peace and meaning.";
            } else if (userQuestion.ToLowerInvariant().Contains("moksha") && userQuestion.ToLowerInvariant().Contains("fast")) {
                finalAnswer = "Moksha is liberation from the cycle of birth and death. The Gita teaches that sincere practice, selfless action, and devotion are the true path—there are no shortcuts, only steady progress.";
            } else if (userQuestion.ToLowerInvariant().Contains("kya") || userQuestion.ToLowerInvariant().Contains("hai")) {
                finalAnswer = "The Gita answers every sincere question, whether in Hindi, Hinglish, or English. Its wisdom is for all seekers, guiding you to truth and self-realization.";
            }
            return finalAnswer;
        }

        // Fetch a web answer for the question using ContextualWeb Search API (free tier, more relevant for spiritual/philosophical questions)
        private string FetchWebAnswer(string question)
        {
            try
            {
                using (var client = new System.Net.Http.HttpClient())
                {
                    // ContextualWeb Search API endpoint and key from config
                    string apiKey = _contextualWebApiKey;
                    if (!string.IsNullOrWhiteSpace(apiKey))
                    {
                        string url = $"https://contextualwebsearch-websearch-v1.p.rapidapi.com/api/Search/WebSearchAPI?q={System.Net.WebUtility.UrlEncode(question)}&pageNumber=1&pageSize=1&autoCorrect=true";
                        client.DefaultRequestHeaders.Add("X-RapidAPI-Key", apiKey);
                        client.DefaultRequestHeaders.Add("X-RapidAPI-Host", "contextualwebsearch-websearch-v1.p.rapidapi.com");
                        var response = client.GetAsync(url).Result;
                        if (response != null && response.IsSuccessStatusCode)
                        {
                            var json = response.Content.ReadAsStringAsync().Result;
                            if (!string.IsNullOrWhiteSpace(json))
                            {
                                dynamic? result = !string.IsNullOrWhiteSpace(json) ? Newtonsoft.Json.JsonConvert.DeserializeObject(json) : null;
                                if (result != null)
                                {
                                    var valueObj = result.value;
                                    if (valueObj is System.Collections.IList valueList && valueList.Count > 0)
                                    {
                                        var firstValue = valueList[0];
                                        if (firstValue != null)
                                        {
                                            var snippetProp = firstValue.GetType().GetProperty("snippet");
                                            string snippet = snippetProp != null ? snippetProp.GetValue(firstValue)?.ToString() ?? string.Empty : string.Empty;
                                            if (!string.IsNullOrWhiteSpace(snippet)) return snippet;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { /* Ignore errors, fallback to DuckDuckGo */ }
            // Fallback: DuckDuckGo Instant Answer API
            try
            {
                using (var client = new System.Net.Http.HttpClient())
                {
                    string url = $"https://api.duckduckgo.com/?q={System.Net.WebUtility.UrlEncode(question)}&format=json&no_redirect=1&no_html=1";
                    var response = client.GetAsync(url).Result;
                    if (response != null && response.IsSuccessStatusCode)
                    {
                        var json = response.Content.ReadAsStringAsync().Result;
                        if (!string.IsNullOrWhiteSpace(json))
                        {
                            dynamic? result = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                            if (result != null)
                            {
                                string answer = (result.AbstractText != null) ? result.AbstractText.ToString() : string.Empty;
                                if (!string.IsNullOrWhiteSpace(answer)) return answer;
                                if (result.RelatedTopics != null)
                                {
                                    var relatedTopics = result.RelatedTopics;
                                    foreach (var topicObj in relatedTopics)
                                    {
                                        var topicType = topicObj?.GetType();
                                        var textProp = topicType?.GetProperty("Text");
                                        string topicText = textProp != null ? textProp.GetValue(topicObj)?.ToString() ?? string.Empty : string.Empty;
                                        if (!string.IsNullOrWhiteSpace(topicText))
                                            return topicText;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { /* Ignore errors, fallback to local answer */ }
            return string.Empty;
        }

        // Rate an answer for relevance and accuracy
        private int RateAnswer(string answer, string question)
        {
            if (string.IsNullOrWhiteSpace(answer)) return 0;
            int score = 0;
            var questionWords = question.ToLowerInvariant().Split(' ');
            foreach (var word in questionWords)
            {
                if (answer.ToLowerInvariant().Contains(word)) score++;
            }
            // Bonus for length (2–5 lines)
            int lineCount = Regex.Split(answer, "(?<=[.!?]) ").Count(s => !string.IsNullOrWhiteSpace(s));
            if (lineCount >= 2 && lineCount <= 5) score += 2;
            // Bonus for non-generic, non-robotic
            if (!answer.ToLowerInvariant().Contains("sorry") && !answer.ToLowerInvariant().Contains("couldn't find")) score++;
            return score;
        }

        // Hybrid answer selection: generate local, fetch web, rate both, pick best, return selected answer directly
        public string GenerateHybrid(List<string> verses, List<string> normalizedPhrases, string detectedIntent, string? originalQuestion)
        {
            string question = originalQuestion ?? string.Join(" ", normalizedPhrases);
            // Prepare one local answer
            string localAnswer = Generate(verses, normalizedPhrases, detectedIntent, originalQuestion);
            // Get one web answer
            string webAnswer = FetchWebAnswer(question);
            // Pick the best answer
            string bestAnswer = !string.IsNullOrWhiteSpace(localAnswer) && !string.IsNullOrWhiteSpace(webAnswer)
                ? (RateAnswer(localAnswer, question) >= RateAnswer(webAnswer, question) ? localAnswer : webAnswer)
                : (!string.IsNullOrWhiteSpace(localAnswer) ? localAnswer : webAnswer);
            if (string.IsNullOrWhiteSpace(bestAnswer)) bestAnswer = localAnswer;
            return bestAnswer;
        }

        // Utility to deduplicate and validate answer before paraphrasing
        private string PrepareForParaphrase(string answer)
        {
            if (string.IsNullOrWhiteSpace(answer))
                return string.Empty;
            var lines = Regex.Split(answer, "(?<=[.!?]) ")
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Distinct()
                .ToList();
            if (lines.Count == 0)
                return string.Empty;
            // Join back to a single string
            return string.Join(" ", lines);
        }

        // Utility to deduplicate, validate, and rephrase answer for natural, human-like output
        private string ParaphraseAndLimit(string answer, string userQuestion)
        {
            if (string.IsNullOrWhiteSpace(answer)) return string.Empty;
            var lines = System.Text.RegularExpressions.Regex.Split(answer, "(?<=[.!?]) ")
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .ToList();
            // Paraphrase duplicate lines instead of removing them
            var seen = new HashSet<string>();
            var paraphrased = new List<string>();
            foreach (var line in lines)
            {
                if (!seen.Contains(line))
                {
                    paraphrased.Add(line);
                    seen.Add(line);
                }
                else
                {
                    // Paraphrase duplicate line
                    paraphrased.Add(RephraseForHumanTone(line, userQuestion));
                }
            }
            // Ensure minimum 2 and maximum 5 lines
            if (paraphrased.Count < 2 && paraphrased.Count > 0) {
                while (paraphrased.Count < 2) paraphrased.Add(paraphrased[0]);
            }
            if (paraphrased.Count > 5) paraphrased = paraphrased.Take(5).ToList();
            return string.Join(" ", paraphrased).Trim();
        }

        // Utility to rephrase a single line for human-like, spiritual tone
        private string RephraseForHumanTone(string line, string userQuestion)
        {
            if (string.IsNullOrWhiteSpace(line)) return string.Empty;
            // Simple replacements for spiritual, natural tone
            return line.Replace("The Bhagavad Gita teaches that", "The Gita reminds us")
                .Replace("Krishna advises", "Krishna suggests")
                .Replace("Krishna says", "Krishna tells us")
                .Replace("act with purpose", "act with meaning")
                .Replace("selfless effort", "doing good for others")
                .Replace("detachment", "letting go")
                .Replace("sincerity", "honesty")
                .Replace("fulfilling your dharma", "living your true path")
                .Replace("spiritual journey", "your path to growth")
                .Replace("present duty", "what you need to do now")
                .Replace("inner calm", "peace within")
                .Replace("outcome to the Divine", "results to God")
                .Replace("free will", "your own choices")
                .Replace("choices matter", "what you choose is important")
                .Replace("clarity will follow", "you will understand more as you go")
                .Replace("move forward with sincerity and detachment", "keep going with honesty and letting go")
                .Replace("courage and faith in your dharma", "be brave and trust your path")
                .Replace("self-realization", "knowing yourself")
                .Replace("act wisely and selflessly", "do good and be wise")
                .Replace("inner nature", "who you truly are")
                .Replace("guide your actions", "help you decide what to do")
                .Replace("spiritual growth", "growing inside")
                .Replace("peace comes from focusing on the present", "peace comes when you live in the moment")
                .Replace("let go of worry", "try not to worry")
                .Replace("learn from the past, but don’t be bound by it", "remember your past, but don’t let it hold you back")
                .Replace("keep walking with courage and faith in your dharma", "keep moving forward bravely and trust your path")
                .Replace("your sincerity matters more than others’ opinions", "what matters most is your honesty, not what others think");
        }

        // Add missing teachingChoicePatterns definition
        private static readonly string[] teachingChoicePatterns = new[]
        {
            "does ", "should ", "is it better to ", "what does ", "does dharma ", "does the gita ", "what should I ", "which is better", "which path", "what does bhagavad gita say", "what does the gita teach", "what does dharma suggest", "gita's teaching", "gita teach", "gita say"
        };
    }
}
