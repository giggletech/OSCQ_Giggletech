using System;
using System.Collections.Generic;
using System.Timers; // Explicitly use System.Timers.Timer to avoid ambiguity
using VRC.OSCQuery; // Import the OSCQuery library
using System.Threading.Tasks;

class Program
{
    // List to store discovered services
    private static List<OSCQueryServiceProfile> _profiles = new List<OSCQueryServiceProfile>();

    static async Task Main(string[] args) // Make Main async to await ListDiscoveredServices
    {
        // Get available TCP and UDP ports
        var tcpPort = Extensions.GetAvailableTcpPort();
        var udpPort = Extensions.GetAvailableUdpPort();

        // Set up the OSCQuery service
        var oscQuery = new OSCQueryServiceBuilder()
            .WithTcpPort(tcpPort)
            .WithUdpPort(udpPort)
            .WithServiceName("Giggletech")
            .WithDefaults()  // <------------------------------- This must go after, or else it will set things to default, can possibly remove later
            .Build();

        // Log the TCP and UDP ports
        Console.WriteLine($"Started OSCQueryService at TCP {tcpPort}, UDP {udpPort}");

        // Add an OSC endpoint that accepts string data
        oscQuery.AddEndpoint("/my/fancy/path", "s", Attributes.AccessValues.WriteOnly, new object[] { "This is my endpoint" });

        Console.WriteLine("Endpoint '/my/fancy/path' added. Access it at:");
        Console.WriteLine($"http://localhost:{tcpPort}/my/fancy/path or http://localhost:{tcpPort}?explorer");

        // Discover and store running services
        DiscoverServices(oscQuery);

        // Set a timer to refresh service discovery every 5 seconds
        var refreshTimer = new System.Timers.Timer(5000); // Use System.Timers.Timer explicitly
        refreshTimer.Elapsed += (s, e) =>
        {
            oscQuery.RefreshServices();
        };
        refreshTimer.Start();

        // Keep running until the user decides to stop the service
        while (true)
        {
            // Wait for user input to remove the endpoint or stop the service
            Console.WriteLine("Press 'R' to remove the endpoint, 'L' to list services, or 'Q' to stop the service.");
            var key = Console.ReadKey();

            if (key.Key == ConsoleKey.R)
            {
                // Remove the endpoint
                oscQuery.RemoveEndpoint("/my/fancy/path");
                Console.WriteLine("\nEndpoint '/my/fancy/path' removed.");
            }
            else if (key.Key == ConsoleKey.L)
            {
                // List services and check for a specific endpoint
                Console.WriteLine("\nListing discovered services...");
                await ListDiscoveredServices(); // Await the async method
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

    // Function to discover services and store them
    static void DiscoverServices(OSCQueryService queryService)
    {
        // Find and store all currently running services
        foreach (var service in queryService.GetOSCQueryServices())
        {
            AddProfileToList(service);
        }

        // Subscribe to new services that get discovered in the future
        queryService.OnOscQueryServiceAdded += (profile) =>
        {
            AddProfileToList(profile);
        };
    }

    // Function to add a profile to the list and log it
    static void AddProfileToList(OSCQueryServiceProfile profile)
    {
        _profiles.Add(profile);
        Console.WriteLine($"Added {profile.name} to list of profiles");
    }

    // Function to list all discovered services and check for a specific endpoint
    static async Task ListDiscoveredServices()
    {
        foreach (var profile in _profiles)
        {
            Console.WriteLine($"Service: {profile.name}, Address: {profile.address}:{profile.port}");

            // Check if the service has a specific endpoint (e.g., "/cool/endpoint")
            var tree = await Extensions.GetOSCTree(profile.address, profile.port);
            var node = tree.GetNodeWithPath("/avatar");

            if (node != null)
            {
                Console.WriteLine($"Found endpoint '/cool/endpoint' on service {profile.name}");
            }
            else
            {
                Console.WriteLine($"No endpoint '/cool/endpoint' found on service {profile.name}");
            }
        }
    }
}
