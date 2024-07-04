using RecommendationEngine.Interfaces;
using RecommendationEngine.Models;

namespace RecommendationEngine.Services
{
    public class EmployeeProfileService : IEmployeeProfileService
    {
        private readonly IEmployeeProfileRepository _repository;

        public EmployeeProfileService(IEmployeeProfileRepository repository)
        {
            _repository = repository;
        }

        public void SaveProfile(EmployeeProfile profile)
        {
            _repository.Save(profile);
        }

        public EmployeeProfile GetProfile(int userId)
        {
            return _repository.FindByUserId(userId);
        }
    }
}
