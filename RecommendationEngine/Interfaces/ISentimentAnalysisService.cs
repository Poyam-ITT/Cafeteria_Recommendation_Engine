namespace RecommendationEngine.Interfaces
{
    public interface ISentimentAnalysisService
    {
        double AnalyzeSentiment(string text);
    }
}
