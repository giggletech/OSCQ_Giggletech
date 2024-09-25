using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using VRC.OSCQuery; // Import the OSCQuery library
using System.Text;

class Program
{
    // List to store discovered services
    private static List<OSCQueryServiceProfile> _profiles = new List<OSCQueryServiceProfile>();
    private static OSCQueryService oscQuery;
    private static int udpPort;

    static async Task Main(string[] args)
    {
        // Set up the HTTP listener for remote control
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:8080/"); // Local HTTP server to handle requests
        listener.Start();
        Console.WriteLine("HTTP server listening on http://localhost:8080/");

        // Start handling remote commands
        Task.Run(() => HandleRemoteCommands(listener));

        Console.WriteLine("Press 'Q' to quit at any time...");

        while (true)
        {
            var key = Console.ReadKey();
            if (key.Key == ConsoleKey.Q)
            {
                StopService();
                listener.Stop();
                break;
            }
        }
    }

    // Function to handle HTTP requests for remote control
    static async Task HandleRemoteCommands(HttpListener listener)
    {
        while (true)
        {
            HttpListenerContext context = await listener.GetContextAsync();
            string command = context.Request.RawUrl.Trim('/').ToLower();

            if (command == "start")
            {
                StartService();
                byte[] buffer = Encoding.UTF8.GetBytes($"Service started on UDP {udpPort}");
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            else if (command == "stop")
            {
                StopService();
                byte[] buffer = Encoding.UTF8.GetBytes("Service stopped");
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            else if (command == "port")
            {
                // Return the current UDP port number
                byte[] buffer = Encoding.UTF8.GetBytes($"{udpPort}");
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            else
            {
                byte[] buffer = Encoding.UTF8.GetBytes("Unknown command");
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            context.Response.OutputStream.Close();
        }
    }

    // Function to start the OSCQuery service
    static void StartService()
    {
        // Get available TCP and UDP ports
        int tcpPort = Extensions.GetAvailableTcpPort();
        udpPort = Extensions.GetAvailableUdpPort();

        // Set up the OSCQuery service
        oscQuery = new OSCQueryServiceBuilder()
            .WithTcpPort(tcpPort)
            .WithUdpPort(udpPort)
            .WithServiceName("Giggletech")
            .WithDefaults()
            .Build();

        Console.WriteLine($"OSCQuery service started at TCP {tcpPort}, UDP {udpPort}");
        Console.WriteLine($"OSC Messages on UDP at: {udpPort}");

        // Add an OSC endpoint
        oscQuery.AddEndpoint("/avatar", "s", Attributes.AccessValues.WriteOnly, new object[] { "This is my avatar endpoint" });
    }

    // Function to stop the OSCQuery service
    static void StopService()
    {
        if (oscQuery != null)
        {
            oscQuery.Dispose();
            Console.WriteLine("OSCQueryService stopped.");
        }
    }
}
