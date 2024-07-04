using RecommendationEngine.Models;

namespace RecommendationEngine.Interfaces
{
    public interface IEmployeeProfileService
    {
        void SaveProfile(EmployeeProfile profile);
        EmployeeProfile GetProfile(int userId);
    }
}
