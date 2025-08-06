using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace BhagwatGitaChatbot.Services
{
    public class KeywordContext
    {
        public string? Keyword { get; set; }
        public string? Definition { get; set; }
        public string? Intent { get; set; }
        public string? Behavior { get; set; }
        public string? EthicalGuidance { get; set; }
        public string? SpiritualContext { get; set; }
        public string? ExampleFromGita { get; set; }
    }

    public interface IKeywordContextService
    {
        IReadOnlyDictionary<string, KeywordContext> GetAllKeywordContexts();
        KeywordContext? GetContextForKeyword(string keyword);
    }

    public class KeywordContextService : IKeywordContextService
    {
        private readonly Dictionary<string, KeywordContext> _keywordContexts = new();

        public KeywordContextService(string jsonFilePath)
        {
            var json = File.ReadAllText(jsonFilePath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var contexts = JsonSerializer.Deserialize<List<KeywordContext>>(json, options);
            if (contexts != null)
            {
                foreach (var ctx in contexts)
                {
                    if (!string.IsNullOrWhiteSpace(ctx.Keyword))
                        _keywordContexts[ctx.Keyword.ToLowerInvariant()] = ctx;
                }
            }
        }

        public IReadOnlyDictionary<string, KeywordContext> GetAllKeywordContexts() => _keywordContexts;

        public KeywordContext? GetContextForKeyword(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) return null;
            _keywordContexts.TryGetValue(keyword.ToLowerInvariant(), out var ctx);
            return ctx;
        }
    }
}
