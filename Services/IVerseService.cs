using System.Collections.Generic;
using BhagwatGitaChatbot.Models;

namespace BhagwatGitaChatbot.Services
{
    public interface IVerseService
    {
        List<string> FindRelevantVerses(List<string> normalizedPhrases);
        List<string> FindRelevantVerses(List<string> normalizedPhrases, string userQuestion);
    }
}
