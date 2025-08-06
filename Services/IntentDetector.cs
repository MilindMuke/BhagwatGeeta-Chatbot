using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace BhagwatGitaChatbot.Services
{
    public class IntentDetector
    {
        private class QAItem { public string? question { get; set; } public string? answer { get; set; } }
        private List<QAItem> dataset = new List<QAItem>();
        private readonly IKeywordContextService _keywordContextService;

        public IntentDetector(string datasetPath, IKeywordContextService keywordContextService)
        {
            _keywordContextService = keywordContextService;

            if (File.Exists(datasetPath))
            {
                var json = File.ReadAllText(datasetPath);
                dataset = JsonConvert.DeserializeObject<List<QAItem>>(json) ?? new List<QAItem>();
            }
            else
            {
                dataset = new List<QAItem>();
            }
        }

        public KeywordContext? DetectIntent(string userQuestion)
        {
            if (string.IsNullOrWhiteSpace(userQuestion)) return null;
            var keywords = _keywordContextService.GetAllKeywordContexts().Keys;
            var foundKeyword = keywords.FirstOrDefault(k => userQuestion.ToLower().Contains(k));
            if (!string.IsNullOrWhiteSpace(foundKeyword))
            {
                return _keywordContextService.GetContextForKeyword(foundKeyword);
            }

            if (dataset == null || dataset.Count == 0) return null;
            var scored = dataset.Select(q => new { q, score = CosineSimilarity(userQuestion, q.question ?? string.Empty) })
                .OrderByDescending(x => x.score).FirstOrDefault();
            if (scored != null && scored.score > 0.5 && !string.IsNullOrWhiteSpace(scored.q.question))
            {
                var closest = scored.q.question;
                var keyword = keywords.FirstOrDefault(k => closest.ToLower().Contains(k));
                if (!string.IsNullOrWhiteSpace(keyword))
                    return _keywordContextService.GetContextForKeyword(keyword);
            }
            return null;
        }

        public string? DetectIntentString(string userQuestion)
        {
            var context = DetectIntent(userQuestion);
            return context?.Keyword;
        }

        private double CosineSimilarity(string a, string b)
        {
            var aWords = a.ToLower().Split(' ', ',', '.', '!', '?', ';', ':');
            var bWords = b.ToLower().Split(' ', ',', '.', '!', '?', ';', ':');
            var allWords = aWords.Concat(bWords).Distinct().ToList();
            var aVec = allWords.Select(w => aWords.Count(x => x == w)).ToArray();
            var bVec = allWords.Select(w => bWords.Count(x => x == w)).ToArray();
            double dot = 0, magA = 0, magB = 0;
            for (int i = 0; i < allWords.Count; i++)
            {
                dot += aVec[i] * bVec[i];
                magA += aVec[i] * aVec[i];
                magB += bVec[i] * bVec[i];
            }
            return magA == 0 || magB == 0 ? 0 : dot / (System.Math.Sqrt(magA) * System.Math.Sqrt(magB));
        }
    }
}
