using RecommendationEngine.Models;

namespace RecommendationEngine.Interfaces
{
    public interface IEmployeeProfileRepository
    {
        void Save(EmployeeProfile profile);
        EmployeeProfile FindByUserId(int userId);
    }
}
