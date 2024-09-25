﻿// Giggletech VRChat OSCQuery Helper
// Usage
//  Get Info:   http://localhost:6969/info
//  Start       http://localhost:6969/start
// UDP Port     http://localhost:6969/port_udp
// TCP Port     http://localhost:6969/port_tcp
// Shut Down    http://localhost:6969/stop
// Change 6969 if you changed the helper Port


using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using VRC.OSCQuery;
using System.Text;
using System.IO;
using System.Threading;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

class Program
{
    // List to store discovered services
    private static List<OSCQueryServiceProfile> _profiles = new List<OSCQueryServiceProfile>();
    private static OSCQueryService oscQuery;
    private static int udpPort;
    private static int tcpPort;

    // HTTP listener port and service name (from YAML config)
    private static int httpPort;
    private static string serviceName;

    // Token for stopping the application from the HTTP request
    private static CancellationTokenSource cts = new CancellationTokenSource();

    static async Task Main(string[] args)
    {
        // Load the initial configuration from the YAML file (to get the HTTP listener port and service name)
        LoadConfigFromYaml();

        // Set up the HTTP listener for remote control using the port from the YAML file
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add($"http://localhost:{httpPort}/"); // Local HTTP server to handle requests

        try
        {
            listener.Start();
            LogMessage($"HTTP server listening on http://localhost:{httpPort}/");
        }
        catch (HttpListenerException ex)
        {
            LogMessage($"Failed to start HTTP listener: {ex.Message}");
            return;
        }

        // Start handling remote commands asynchronously
        Task listenerTask = HandleRemoteCommands(listener, cts.Token);

        // Await the listener task to keep the application alive
        await listenerTask;
    }

    // Function to handle HTTP requests for remote control
    static async Task HandleRemoteCommands(HttpListener listener, CancellationToken token)
    {
        while (listener.IsListening && !token.IsCancellationRequested)
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
                byte[] buffer = Encoding.UTF8.GetBytes($"Service started...");
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            else if (command == "stop")
            {
                StopService();
                byte[] buffer = Encoding.UTF8.GetBytes("Service stopped. Application shutting down...");
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                
                // Signal the application to stop by canceling the token
                cts.Cancel();
                
                // Ensure all responses are flushed before closing
                context.Response.OutputStream.Close();

                // Shutdown the application
                ShutdownApplication();
            }
            else if (command == "info")
            {
                // Return the current TCP, UDP, and HTTP listener port numbers
                byte[] buffer = Encoding.UTF8.GetBytes($"Giggletech OSCQuery Helper\nTCP Port: {tcpPort}\nUDP Port: {udpPort}\nHTTP Listener Port: {httpPort}\nService Name: {serviceName}");

                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            else if (command == "port_udp")
            {
                // Return only the UDP port
                byte[] buffer = Encoding.UTF8.GetBytes($"{udpPort}");
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            else if (command == "port_tcp")
            {
                // Return only the TCP port
                byte[] buffer = Encoding.UTF8.GetBytes($"{tcpPort}");
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            else
            {
                byte[] buffer = Encoding.UTF8.GetBytes("Unknown command");
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            }

            context.Response.OutputStream.Close();
        }

        // Close the listener once the loop exits
        listener.Stop();
    }

    // Function to start the OSCQuery service
    static void StartService()
    {
        // If an OSCQuery service is already running, stop and dispose of it
        if (oscQuery != null)
        {
            StopService();
            LogMessage("A service instance was already running. The previous service has been stopped and a new instance will be started.");
        }

        // Get available TCP and UDP ports for OSCQuery (do NOT read from YAML)
        tcpPort = Extensions.GetAvailableTcpPort();
        udpPort = Extensions.GetAvailableUdpPort();

        // Set up the OSCQuery service
        oscQuery = new OSCQueryServiceBuilder()
            .WithTcpPort(tcpPort)
            .WithUdpPort(udpPort)
            .WithServiceName(serviceName) // Use the service name from the YAML file
            .WithDefaults()
            .Build();
        
        LogMessage($"OSCQuery service started at TCP {tcpPort}, UDP {udpPort}, Service Name: {serviceName}");
        LogMessage($"OSC Messages on UDP at: {udpPort}");

        // Add an OSC endpoint
        oscQuery.AddEndpoint("/avatar", "s", Attributes.AccessValues.WriteOnly, new object[] { "This is my avatar endpoint" });
    }

    // Function to stop the OSCQuery service
    static void StopService()
    {
        // Dispose of the OSCQuery service if it exists
        if (oscQuery != null)
        {
            oscQuery.Dispose();
            oscQuery = null;  // Set to null to indicate the service is stopped
            LogMessage("OSCQueryService stopped.");
        }
    }

    // Function to shut down the application gracefully
    static void ShutdownApplication()
    {
        LogMessage("Shutting down application...");
        
        // Exit the application with code 0 (success)
        Environment.Exit(0);
    }

    // Logging function
    static void LogMessage(string message)
    {
        string logFilePath = "service_log_oscq.txt"; // Log file path
        File.AppendAllText(logFilePath, $"{DateTime.Now}: {message}{Environment.NewLine}");
    }

    // Load the HTTP port and service name configuration from YAML file
    static void LoadConfigFromYaml()
    {
        try
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();

            var yamlContent = File.ReadAllText("config_oscq.yml");
            var config = deserializer.Deserialize<Dictionary<string, object>>(yamlContent);

            httpPort = Convert.ToInt32(config["httpPort"]);
            serviceName = config["serviceName"].ToString();

            LogMessage($"Loaded configuration: HTTP Listener Port {httpPort}, Service Name {serviceName}");
        }
        catch (Exception ex)
        {
            LogMessage($"Error loading configuration: {ex.Message}");
            httpPort = 8080;  // Default HTTP listener port
            serviceName = "Giggletech";  // Default service name
        }
    }
}
