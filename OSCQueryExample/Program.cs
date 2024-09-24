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
            .WithServiceName("Giggletech")
            .WithDefaults()
            .WithTcpPort(tcpPort)
            .WithUdpPort(udpPort)
            .Build();

        // Log the TCP and UDP ports
        Console.WriteLine($"Sideways Test");
        Console.WriteLine($"Started OSCQueryService at TCP {tcpPort}, UDP {udpPort}");

        // Wait for a key press to stop the service
        Console.WriteLine("Press any key to stop the service...");
        Console.ReadKey();

        // Stop the service
        oscQuery.Dispose();  // This stops the service

        Console.WriteLine("OSCQueryService stopped.");
    }
}
