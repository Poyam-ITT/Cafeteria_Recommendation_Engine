using RecommendationEngine.Interfaces;namespace RecommendationEngine.Services{    public class AuthService : IAuthService    {        private readonly IUserRepository _userRepository;        public AuthService(IUserRepository userRepository)        {            _userRepository = userRepository;        }        public bool Authenticate(string employeeId, string name, out string role, out int userId)
        {
            role = string.Empty;
            userId = 0;
            var user = _userRepository.FindByEmployeeIdAndName(employeeId, name);
            if (user != null)
            {
                role = user.Role;
                userId = user.Id;
                return true;
            }
            return false;
        }    }}