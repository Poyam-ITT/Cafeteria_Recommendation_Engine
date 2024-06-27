using System;
using RecommendationEngine.Sockets;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter port number to connect to server (e.g., 5000, 5001, 5002):");
            var portInput = Console.ReadLine();

            if (int.TryParse(portInput, out var port))
            {
                var client = new Client(port);
                client.Connect();

                Console.WriteLine("Enter Employee ID:");
                var employeeId = Console.ReadLine();
                client.SendMessage(employeeId);

                Console.WriteLine("Enter Name:");
                var name = Console.ReadLine();
                client.SendMessage(name);

                var thread = new Thread(() => client.ReceiveMessages());
                thread.Start();

                while (true)
                {
                    var message = Console.ReadLine();
                    client.SendMessage(message);
                }
            }
            else
            {
                Console.WriteLine("Invalid port number.");
            }
        }
    }
}
