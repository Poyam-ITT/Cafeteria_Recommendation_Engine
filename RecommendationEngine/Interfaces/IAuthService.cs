namespace RecommendationEngine.Interfaces
{
    public interface IAuthService
    {
        bool Authenticate(string employeeId, string name, out string role);
    }
}
