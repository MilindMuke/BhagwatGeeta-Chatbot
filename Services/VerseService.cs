using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BhagwatGitaChatbot.Services
{
    public class VerseService : IVerseService
    {
        private readonly string _dataPath = Path.Combine(Directory.GetCurrentDirectory(), "LocalData");

        public List<string> FindRelevantVerses(List<string> normalizedPhrases, string userQuestion = null)
        {
            var verses = new List<string>();
            var allVerses = new List<Verse>();
            foreach (var file in Directory.GetFiles(_dataPath, "*.json"))
            {
                var json = File.ReadAllText(file);
                List<Verse>? verseList = null;
                try
                {
                    verseList = JsonConvert.DeserializeObject<List<Verse>>(json);
                }
                catch
                {
                    var jObj = JObject.Parse(json);
                    var arrayToken = jObj["chapters"] ?? jObj["verses"] ?? jObj["data"];
                    if (arrayToken != null && arrayToken.Type == JTokenType.Array)
                        verseList = arrayToken.ToObject<List<Verse>>();
                }
                if (verseList != null)
                    allVerses.AddRange(verseList);
            }
            // Semantic similarity: compare userQuestion to verse descriptions
            if (!string.IsNullOrEmpty(userQuestion))
            {
                var scored = allVerses
                    .Where(v => !string.IsNullOrEmpty(v?.Description))
                    .Select(v => new { Verse = v, Score = CosineSimilarity(userQuestion, v?.Description ?? string.Empty) })
                    .OrderByDescending(x => x.Score)
                    .Take(3)
                    .Select(x => x.Verse.Description ?? x.Verse.Text ?? string.Empty)
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToList();
                if (scored.Count > 0)
                    return scored;
            }
            // Fallback: keyword matching
            foreach (var verse in allVerses)
            {
                var keywords = verse?.Keywords ?? new List<string>();
                if (keywords.Count == 0 && !string.IsNullOrEmpty(verse?.Description))
                {
                    keywords = (verse.Description ?? string.Empty).Split(' ', ',', '.', '!', '?', ';', ':').Select(k => k.ToLower()).Distinct().ToList();
                }
                if (normalizedPhrases != null && normalizedPhrases.Any(p => keywords.Contains(p)))
                    verses.Add(verse?.Description ?? verse?.Text ?? string.Empty);
            }
            return verses;
        }

        public List<string> FindRelevantVerses(List<string> normalizedPhrases)
        {
            return FindRelevantVerses(normalizedPhrases, null);
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

    public class Verse
    {
        public string? Text { get; set; }
        public string? Description { get; set; }
        public List<string> Keywords { get; set; } = new List<string>();
    }
}
