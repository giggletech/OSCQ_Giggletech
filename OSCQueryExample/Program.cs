using System;
using VRC.OSCQuery; // Import the OSCQuery library

class Program
{
    static void Main(string[] args)
    {
        // Get available TCP and UDP ports
        var tcpPort = Extensions.GetAvailableTcpPort();
        var udpPort = Extensions.GetAvailableUdpPort();

        // Set up the OSCQuery service
        var oscQuery = new OSCQueryServiceBuilder()
            
            .WithTcpPort(tcpPort)
            .WithUdpPort(udpPort)
            .WithServiceName("Giggletech")
            .WithDefaults()
            .Build();

        // Log the TCP and UDP ports
        Console.WriteLine($"Started OSCQueryService at TCP {tcpPort}, UDP {udpPort}");

        // Add an OSC endpoint that accepts string data
        oscQuery.AddEndpoint("/my/fancy/path", "s", Attributes.AccessValues.WriteOnly, new object[] { "This is my endpoint" });

        Console.WriteLine("Endpoint '/my/fancy/path' added. Access it at:");
        Console.WriteLine($"http://localhost:{tcpPort}/my/fancy/path or http://localhost:{tcpPort}?explorer");

        // Keep running until the user decides to stop the service
        while (true)
        {
            // Wait for user input to remove the endpoint or stop the service
            Console.WriteLine("Press 'R' to remove the endpoint or 'Q' to stop the service.");
            var key = Console.ReadKey();

            if (key.Key == ConsoleKey.R)
            {
                // Remove the endpoint
                oscQuery.RemoveEndpoint("/my/fancy/path");
                Console.WriteLine("\nEndpoint '/my/fancy/path' removed.");
            }
            else if (key.Key == ConsoleKey.Q)
            {
                // Stop the service and exit the loop
                oscQuery.Dispose();  // This stops the service
                Console.WriteLine("\nOSCQueryService stopped.");
                break; // Exit the loop and end the program
            }
        }
    }
}
