using System.Collections.Generic;

namespace RecommendationEngine.Utils
{
    public static class SentimentLexicon
    {
        public static readonly Dictionary<string, double> Lexicon = new Dictionary<string, double>
        {
            { "good", 2.0 },
            { "great", 3.0 },
            { "excellent", 4.0 },
            { "amazing", 3.5 },
            { "bad", -2.0 },
            { "terrible", -3.0 },
            { "horrible", -4.0 },
            { "poor", -2.5 },
            // can add more
        };
    }
}
