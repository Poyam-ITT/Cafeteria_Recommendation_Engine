using System;
using System.Net.Sockets;
using RecommendationEngine.Sockets;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Enter port number to connect to server");
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
            catch (FormatException ex)
            {
                Console.WriteLine($"The input format was invalid. Please try again. {ex.Message}");
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Socket error while connecting: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
