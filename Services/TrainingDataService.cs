using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace BhagwatGitaChatbot.Services
{
    public class TrainingDataEntry
    {
        public string? question { get; set; } = string.Empty;
        public string? answer { get; set; } = string.Empty;
        public string? question_type { get; set; } = string.Empty;
        public string? intent_hint { get; set; } = string.Empty;
    }

    public class TrainingDataService
    {
        private readonly List<TrainingDataEntry> _entries;

        public TrainingDataService(string filePath)
        {
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                _entries = JsonConvert.DeserializeObject<List<TrainingDataEntry>>(json) ?? new List<TrainingDataEntry>();
            }
            else
            {
                _entries = new List<TrainingDataEntry>();
            }
        }

        public TrainingDataEntry? FindBestMatch(string userQuestion)
        {
            if (string.IsNullOrWhiteSpace(userQuestion)) return null;
            var normalized = userQuestion.Trim().ToLowerInvariant();
            return _entries.FirstOrDefault(e => (e.question ?? string.Empty).Trim().ToLowerInvariant() == normalized);
        }

        public TrainingDataEntry? FindBestDirectMatch(string userQuestion)
        {
            if (string.IsNullOrWhiteSpace(userQuestion)) return null;
            var normalized = userQuestion.Trim().ToLowerInvariant();
            return _entries.FirstOrDefault(e => (e.question ?? string.Empty).Trim().ToLowerInvariant() == normalized);
        }

        // Levenshtein distance for fuzzy matching
        private int LevenshteinDistance(string s, string t)
        {
            if (string.IsNullOrEmpty(s)) return t.Length;
            if (string.IsNullOrEmpty(t)) return s.Length;
            int[,] d = new int[s.Length + 1, t.Length + 1];
            for (int i = 0; i <= s.Length; i++) d[i, 0] = i;
            for (int j = 0; j <= t.Length; j++) d[0, j] = j;
            for (int i = 1; i <= s.Length; i++)
            {
                for (int j = 1; j <= t.Length; j++)
                {
                    int cost = (s[i - 1] == t[j - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            return d[s.Length, t.Length];
        }

        public TrainingDataEntry FindBestFuzzyMatch(string userQuestion, int maxDistance = 8)
        {
            var normalized = userQuestion.Trim().ToLowerInvariant();
            TrainingDataEntry best = null;
            int bestDist = int.MaxValue;
            foreach (var e in _entries)
            {
                int dist = LevenshteinDistance(normalized, e.question.Trim().ToLowerInvariant());
                if (dist < bestDist && dist <= maxDistance)
                {
                    bestDist = dist;
                    best = e;
                }
            }
            return best;
        }

        public TrainingDataEntry FindByType(string type)
        {
            return _entries.FirstOrDefault(e => e.question_type == type);
        }

        public IEnumerable<TrainingDataEntry> AllEntries => _entries;
    }
}
