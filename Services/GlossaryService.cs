using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace BhagwatGitaChatbot.Services
{
    public class GlossaryService
    {
        private readonly Dictionary<string, string> _glossary;

        public GlossaryService(string jsonFilePath)
        {
            var json = File.ReadAllText(jsonFilePath);
            _glossary = JsonSerializer.Deserialize<Dictionary<string, string>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new Dictionary<string, string>();
        }

        public string? GetDefinition(string term)
        {
            if (string.IsNullOrWhiteSpace(term)) return null;
            _glossary.TryGetValue(term.Trim().ToLowerInvariant(), out var definition);
            return definition;
        }

        public IEnumerable<string> GetAllTerms() => _glossary.Keys;
    }
}
