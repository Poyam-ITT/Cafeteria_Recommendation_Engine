namespace RecommendationEngine.Utils
{
    public static class SentimentLexicon
    {
        public static readonly Dictionary<string, double> Lexicon = new Dictionary<string, double>
        {
            { "good", 3.0 },
            { "great", 4.0 },
            { "excellent", 4.5 },
            { "amazing", 4.5 },
            { "bad", -2.0 },
            { "terrible", -3.0 },
            { "horrible", -4.0 },
            { "poor", -2.5 },
            { "awesome", 3.0 },
            { "fantastic", 3.5 },
            { "mediocre", -1.5 },
            { "average", 0.0 },
            { "satisfactory", 1.0 },
            { "disappointing", -2.5 },
            { "underwhelming", -1.5 },
            { "superb", 4.0 },
            { "wonderful", 3.5 },
            { "awful", -3.5 },
            { "brilliant", 3.5 },
            { "subpar", -2.0 },
            { "stellar", 4.0 },
            { "okay", 0.5 },
            { "dreadful", -3.5 },
            { "fabulous", 4.0 },
            { "decent", 1.5 },
            { "lousy", -2.5 },
            { "exceptional", 4.0 },
            { "fine", 1.0 },
            { "mediocre", -1.5 },
            { "commendable", 3.0 },
            { "unsatisfactory", -2.0 },
        };
    }
}
