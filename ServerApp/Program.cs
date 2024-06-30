using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using RecommendationEngine.Interfaces;
using RecommendationEngine.Models;
using RecommendationEngine.Repositories;
using RecommendationEngine.Services;
using RecommendationEngine.Sockets;
using RecommendationEngine.Utils;

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
                .AddSingleton<IAuthService, AuthService>()
                .AddSingleton<IMenuService, MenuService>()
                .AddSingleton<IFeedbackService, FeedbackService>()
                .AddSingleton<INotificationService, NotificationService>()
                .AddSingleton<IChefService, ChefService>()
                .AddSingleton<IEmployeeService, EmployeeService>()
                .AddSingleton<IRecommendationEngine, RecommendationEngine.Services.RecommendationEngine>() 
                .AddSingleton<ISentimentAnalysisService, SentimentAnalysisService>()
                .BuildServiceProvider();
            
            // Initialize Database
            DbUtils.InitializeDatabase();

            // Start Servers on different ports
            var serverPorts = new[] { 5000, 5001, 5002 };
            foreach (var port in serverPorts)
            {
                var server = new Server(port, serviceProvider);
                var serverThread = new Thread(server.Start);
                serverThread.Start();
            }
        }
    }
}
