using System.Collections.Generic;

namespace BhagwatGitaChatbot.Services
{
    public static class SynonymNormalizer
    {
        private static readonly Dictionary<string, string> Synonyms = new()
        {
            { "dharm", "dharma" },
            { "dharam", "dharma" },
            { "karm", "karma" },
            { "karam", "karma" },
            { "karmic", "karma" },
            { "karma", "karma" },
            { "dharma", "dharma" },
            { "anger", "krodh" },
            { "fear", "bhay" },
            { "mukti", "moksha" },
            { "moksh", "moksha" },
            { "moksha", "moksha" },
            { "gyan", "jnana" },
            { "gyaan", "jnana" },
            { "jnana", "jnana" },
            { "prem", "love" },
            { "shanti", "peace" },
            { "shant", "peace" },
            { "ahimsa", "non-violence" },
            { "tyag", "renunciation" },
            { "tyaga", "renunciation" },
            { "vairagya", "detachment" },
            { "vairag", "detachment" },
            { "aatma", "atman" },
            { "atma", "atman" },
            { "atman", "atman" },
            // Add more as needed
        };

        public static List<string> Normalize(List<string> phrases)
        {
            var normalized = new List<string>();
            foreach (var phrase in phrases)
            {
                if (Synonyms.TryGetValue(phrase.ToLower(), out var norm))
                    normalized.Add(norm);
                else
                    normalized.Add(phrase);
            }
            return normalized;
        }
    }
}
