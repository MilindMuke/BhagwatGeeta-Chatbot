using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Comprehend;
using Amazon.Comprehend.Model;
using Amazon.Runtime;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace BhagwatGitaChatbot.Services
{
    public class AwsComprehendService : IAwsComprehendService
    {
        private readonly AmazonComprehendClient _client;

        public AwsComprehendService(IConfiguration config)
        {
            // Try to read credentials from appsettings.json
            var accessKey = config["AWS:AccessKey"];
            var secretKey = config["AWS:SecretKey"];
            var region = config["AWS:Region"] ?? "us-east-1";

            // If not found, try to read from rootkey.csv
            if (string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey))
            {
                var csvPath = Path.Combine(Directory.GetCurrentDirectory(), "rootkey.csv");
                if (File.Exists(csvPath))
                {
                    var lines = File.ReadAllLines(csvPath);
                    if (lines.Length > 1)
                    {
                        var parts = lines[1].Split(',');
                        if (parts.Length == 2)
                        {
                            accessKey = parts[0].Trim();
                            secretKey = parts[1].Trim();
                        }
                    }
                }
            }
            var credentials = new BasicAWSCredentials(accessKey, secretKey);
            _client = new AmazonComprehendClient(credentials, Amazon.RegionEndpoint.GetBySystemName(region));
        }

        public string DetectLanguage(string text)
        {
            throw new System.NotImplementedException("AWS Comprehend is disabled. Use LocalNlpService instead.");
        }

        public List<string> ExtractKeyPhrases(string text)
        {
            throw new System.NotImplementedException("AWS Comprehend is disabled. Use LocalNlpService instead.");
        }

        public async Task<string> DetectLanguageAsync(string text)
        {
            var request = new DetectDominantLanguageRequest { Text = text };
            var response = await _client.DetectDominantLanguageAsync(request);
            return response.Languages.Count > 0 ? response.Languages[0].LanguageCode : "unknown";
        }

        public async Task<List<string>> ExtractKeyPhrasesAsync(string text)
        {
            var request = new DetectKeyPhrasesRequest { Text = text, LanguageCode = "en" };
            var response = await _client.DetectKeyPhrasesAsync(request);
            var phrases = new List<string>();
            foreach (var kp in response.KeyPhrases)
                phrases.Add(kp.Text);
            return phrases;
        }
    }
}
