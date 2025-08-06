using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq;

namespace BhagwatGitaChatbot.Services
{
    public class LocalNlpService : IAwsComprehendService
    {
        // Improved language detection: checks for alphabetic words and their ratio
        public string DetectLanguage(string text)
        {
            // Improved: check if most words are alphabetic and text is not empty
            var words = Regex.Split(text.ToLower(), @"\W+").Where(w => !string.IsNullOrWhiteSpace(w)).ToList();
            if (words.Count == 0) return "unknown";
            int alphaCount = words.Count(w => w.All(char.IsLetter));
            double ratio = (double)alphaCount / words.Count;
            return ratio > 0.6 ? "en" : "unknown";
        }

        // Simple key phrase extraction: returns words longer than 3 letters
        public List<string> ExtractKeyPhrases(string text)
        {
            var matches = Regex.Matches(text, @"\b\w{4,}\b");
            return matches.Select(m => m.Value.ToLower()).Distinct().ToList();
        }
    }
}
