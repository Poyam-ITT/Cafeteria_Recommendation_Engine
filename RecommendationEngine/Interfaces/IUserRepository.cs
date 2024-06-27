using RecommendationEngine.Models;

namespace RecommendationEngine.Interfaces
{
    public interface IUserRepository
    {
        void Save(User user);
        User FindById(int id);
        User FindByEmployeeIdAndName(string employeeId, string name);
    }
}
