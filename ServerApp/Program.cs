using Microsoft.Extensions.DependencyInjection;
using RecommendationEngine.Interfaces;
using RecommendationEngine.Repositories;
using RecommendationEngine.Repository;
using RecommendationEngine.Services;
using RecommendationEngine.Utils;
using ServerApp.Handler;
using ServerApp.Socket;
using System.Net.Sockets;

namespace ServerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // Setup Dependency Injection
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IUserRepository, UserRepository>()
                .AddSingleton<IMenuItemRepository, MenuItemRepository>()
                .AddSingleton<IFeedbackRepository, FeedbackRepository>()
                .AddSingleton<IRolledOutItemRepository, RolledOutItemRepository>()
                .AddSingleton<INotificationRepository, NotificationRepository>()
                .AddSingleton<IEmployeeProfileRepository, EmployeeProfileRepository>()
                .AddSingleton<IAuthService, AuthService>()
                .AddSingleton<IMenuService, MenuService>()
                .AddSingleton<IFeedbackService, FeedbackService>()
                .AddSingleton<INotificationService, NotificationService>()
                .AddSingleton<IChefService, ChefService>()
                .AddSingleton<IEmployeeService, EmployeeService>()
                .AddSingleton<IEmployeeProfileService, EmployeeProfileService>()
                .AddSingleton<IRecommendationEngine, RecommendationEngine.Services.RecommendationEngine>() 
                .AddSingleton<ISentimentAnalysisService, SentimentAnalysisService>()
                .AddSingleton<IClientHandler, ClientHandler>()
                .AddSingleton<IRequestHandler, RequestHandler>()
                .BuildServiceProvider();

            try
            {
                DbUtils.InitializeDatabase();

                int startingPort = 5000;
                int totalOptions = 10;

                for (int i = 0; i < totalOptions; i++)
                {
                    int port = startingPort + i;
                    var server = new Server(port, serviceProvider);

                    Thread serverThread = new Thread(server.Start);
                    serverThread.Start();
                }

                Console.WriteLine("Server running...");
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Socket error on port: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error occurred: {ex.Message}");
            }
        }
    }
}
