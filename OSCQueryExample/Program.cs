using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using VRC.OSCQuery;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

class Program
{
    // List to store discovered services
    private static List<OSCQueryServiceProfile> _profiles = new List<OSCQueryServiceProfile>();
    private static OSCQueryService oscQuery;
    private static int udpPort;

    // Import the necessary methods for hiding the console window
    [DllImport("kernel32.dll")]
    static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    const int SW_HIDE = 0;

    static async Task Main(string[] args)
    {
        // Hide the console window
        var handle = GetConsoleWindow();
        ShowWindow(handle, SW_HIDE);

        // Set up the HTTP listener for remote control
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:8080/"); // Local HTTP server to handle requests

        try
        {
            listener.Start();
            LogMessage("HTTP server listening on http://localhost:8080/");
        }
        catch (HttpListenerException ex)
        {
            LogMessage($"Failed to start HTTP listener: {ex.Message}");
            return;
        }

        // Start handling remote commands asynchronously
        Task listenerTask = HandleRemoteCommands(listener);

        LogMessage("Press 'Q' to quit at any time...");

        // Keep the application alive until 'Q' is pressed
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

        // Await the listener task to make sure it's properly handled
        await listenerTask;
    }

    // Function to handle HTTP requests for remote control
    static async Task HandleRemoteCommands(HttpListener listener)
    {
        while (listener.IsListening)
        {
            HttpListenerContext context;
            try
            {
                context = await listener.GetContextAsync();
            }
            catch (HttpListenerException ex)
            {
                LogMessage($"Listener stopped or failed: {ex.Message}");
                break;
            }

            string command = context.Request.RawUrl.Trim('/').ToLower();
            LogMessage($"Received command: {command}");

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

        LogMessage($"OSCQuery service started at TCP {tcpPort}, UDP {udpPort}");
        LogMessage($"OSC Messages on UDP at: {udpPort}");

        // Add an OSC endpoint
        oscQuery.AddEndpoint("/avatar", "s", Attributes.AccessValues.WriteOnly, new object[] { "This is my avatar endpoint" });
    }

    // Function to stop the OSCQuery service
    static void StopService()
    {
        if (oscQuery != null)
        {
            oscQuery.Dispose();
            LogMessage("OSCQueryService stopped.");
        }
    }

    // Logging function
    static void LogMessage(string message)
    {
        string logFilePath = "service_log.txt"; // Log file path
        File.AppendAllText(logFilePath, $"{DateTime.Now}: {message}{Environment.NewLine}");
    }
}
