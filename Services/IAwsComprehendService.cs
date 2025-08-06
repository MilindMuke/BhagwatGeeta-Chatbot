using System.Collections.Generic;
using System.Threading.Tasks;

namespace BhagwatGitaChatbot.Services
{
    public interface IAwsComprehendService
    {
        string DetectLanguage(string text);
        List<string> ExtractKeyPhrases(string text);
    }
}
