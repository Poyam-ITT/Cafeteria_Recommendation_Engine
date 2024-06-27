using System;
using System.Linq;
using RecommendationEngine.Interfaces;
using RecommendationEngine.Utils;

namespace RecommendationEngine.Services
{
    public class SentimentAnalysisService : ISentimentAnalysisService
    {
        public double AnalyzeSentiment(string text)
        {
            var words = text.Split(new[] { ' ', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
            var score = 0.0;

            foreach (var word in words)
            {
                if (SentimentLexicon.Lexicon.TryGetValue(word.ToLower(), out var wordScore))
                {
                    score += wordScore;
                }
            }

            return score;
        }
    }
}
