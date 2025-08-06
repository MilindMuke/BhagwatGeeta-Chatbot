using Microsoft.AspNetCore.Mvc;
using BhagwatGitaChatbot.Models;
using BhagwatGitaChatbot.Services;

namespace BhagwatGitaChatbot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatbotController : ControllerBase
    {
        private readonly IAwsComprehendService _comprehend;
        private readonly IVerseService _verseService;
        private readonly IResponseGenerator _responseGenerator;
        private readonly IKeywordContextService _keywordContextService;
        private readonly IntentDetector _intentDetector;

        public ChatbotController(IAwsComprehendService comprehend, IVerseService verseService, IResponseGenerator responseGenerator, IKeywordContextService keywordContextService)
        {
            _comprehend = comprehend;
            _verseService = verseService;
            _responseGenerator = responseGenerator;
            _keywordContextService = keywordContextService;
            _intentDetector = new IntentDetector(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "LocalData", "bhagavad_gita_qa_dataset.json"), _keywordContextService);
        }

        [HttpPost]
        public IActionResult Post([FromBody] ChatRequest request)
        {
            try
            {
                var keyPhrases = _comprehend.ExtractKeyPhrases(request.Question);
                var normalized = SynonymNormalizer.Normalize(keyPhrases);
                var verses = _verseService.FindRelevantVerses(normalized, request.Question);
                var detectedIntent = _intentDetector.DetectIntentString(request.Question);
                // Use hybrid answer generation
                var answer = (_responseGenerator as ResponseGenerator)?.GenerateHybrid(verses, normalized, detectedIntent, request.Question)
                    ?? (_responseGenerator as ResponseGenerator)?.GenerateHybrid(verses, normalized, detectedIntent, request.Question);
                return Ok(new ChatResponse { Answer = answer });
            }
            catch (Exception ex)
            {
                return Ok(new ChatResponse { Answer = $"Error: {ex.Message}" });
            }
        }
    }
}
