using System.Collections.Generic;

namespace BhagwatGitaChatbot.Services
{
    public interface IResponseGenerator
    {
        string Generate(List<string> verses, List<string> normalizedPhrases);
        string Generate(List<string> verses, List<string> normalizedPhrases, string detectedIntent);
    }
}
