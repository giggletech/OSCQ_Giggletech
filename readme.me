# Giggletech VRChat OSCQuery Helper

This repository contains two key components for running the **Giggletech VRChat OSCQuery Helper** service. The service manages the communication between the Giggletech system and VRChat through OSC (Open Sound Control) via an OSCQuery server.

- **Rust Module**: Manages the process lifecycle of the Giggletech OSCQuery server (`giggletech_oscq.exe`), ensuring it is always running and accessible.
- **C# OSCQuery Server**: The OSCQuery service itself, handling commands via HTTP and providing information about the TCP and UDP ports for communication.

## Components Overview

### 1. Rust Module: `oscq_giggletech`

This Rust module is responsible for starting, monitoring, and managing the Giggletech OSCQuery server. It continuously checks the server's status, retrieves the UDP port, and restarts the process if necessary.

#### **Modular Design**
The Rust module has been designed to be modular, making it easy to integrate the VRChat OSCQuery system into any Rust project. By including this module, developers can effortlessly incorporate OSCQuery functionality, which allows for real-time control and interaction with VRChat avatars and environments via OSC.

**Key Functions:**
- **Initialize and Start Process**: Starts the `giggletech_oscq.exe` process.
- **Check UDP Port**: Continuously polls the OSCQuery server for the UDP port using an HTTP request.
- **Restart Process**: Automatically restarts the server if the UDP port is not returned or if the process fails.
- **Remote Commands**: Interacts with the OSCQuery server over HTTP for additional commands like starting or stopping the server.

#### Usage:

```rust
fn main() {
    // Call the function to initialize and get the UDP port
    let udp_port = oscq_giggletech::initialize_and_get_udp_port();
    println!("UDP Port: {}", udp_port);
}

/*

Key HTTP Endpoints (provided by the C# server):

http://localhost:6969/info: Get information about the TCP, UDP, and HTTP ports.
http://localhost:6969/start: Start the OSCQuery service.
http://localhost:6969/port_udp: Retrieve the current UDP port.
http://localhost:6969/port_tcp: Retrieve the current TCP port.
http://localhost:6969/stop: Shut down the OSCQuery server.

2. C# OSCQuery Server: giggletech_oscq.exe
The C# project runs the OSCQuery server, which listens for HTTP commands and manages the UDP/TCP ports used for OSC communication with VRChat. It provides endpoints for controlling the service and querying for port information.

Key Features:

Start/Stop OSCQuery Service: Allows the OSCQuery server to be started or stopped remotely.
HTTP Listener: Receives commands on a configurable port (default: 6969).
Port Management: Returns the TCP and UDP ports in use via HTTP.


Configuration: Reads from config_oscq.yml to determine the HTTP port and service name.
*/
 Config File: config_oscq.yml
*/
httpPort: 6969
serviceName: "Giggletech VRChat Service"

//This YAML file is used to configure the HTTP listener and the service name. It should be placed in the same directory as the executable or in %APPDATA%\Giggletech.

/*
1. Clone the Repository
bash
Copy code
git clone https://github.com/your-repo/giggletech-vrchat-oscq-helper.git
cd giggletech-vrchat-oscq-helper

2. Build the Projects
Rust Module
Ensure you have Rust installed.
Navigate to the Rust project directory (oscq_giggletech) and build the project:
bash
Copy code
cargo build --release
This will compile the Rust module responsible for starting and managing the giggletech_oscq.exe process.

C# OSCQuery Server
Ensure you have dotnet installed.
Navigate to the C# project directory and build the project:
bash
Copy code
dotnet build
Create the config_oscq.yml file in the same directory as the executable or in %APPDATA%\Giggletech. Define your preferred HTTP port and service name.
3. Running the Application
Step 1: Start the C# OSCQuery Server
You can start the C# server (giggletech_oscq.exe) directly by running it. This will launch the OSCQuery service and start listening for HTTP commands.

bash
Copy code
./giggletech_oscq.exe
Once running, the service will listen on the port defined in config_oscq.yml (default: 6969).

Step 2: Run the Rust Module
The Rust module should be responsible for continuously monitoring the giggletech_oscq.exe process. You can start the Rust module to manage the server.

bash
Copy code
./target/release/oscq_giggletech
This will:

Start the giggletech_oscq.exe process if it’s not running.
Continuously check for the UDP port using the /port_udp endpoint.
Restart the giggletech_oscq.exe process if needed.
4. Access the HTTP Commands
Once both components are running, you can use any HTTP client (like curl or a browser) to interact with the OSCQuery service. Example:

bash
Copy code
curl http://localhost:6969/info
This will return the current status of the service, including the TCP, UDP, and HTTP ports.

Modular Design for Rust Projects
The Rust module (oscq_giggletech) is designed to be modular and can easily be incorporated into any Rust project. By integrating it, you can quickly add OSCQuery functionality to your VRChat or other OSC-enabled applications. The Rust module manages the process lifecycle of the C# OSCQuery server, ensuring the server is always available and restarts if necessary.

Key Benefits:
Easy Integration: Simply include the Rust module in your project to start and manage the OSCQuery server.
Automatic Restart: The module ensures that the OSCQuery server is always running and automatically restarts it if it crashes or fails to return a valid UDP port.
Remote Control: Built-in HTTP commands allow you to start, stop, or retrieve server information remotely.
Troubleshooting
Permission Issues:

If you encounter permission errors when running the C# executable, ensure it has the necessary permissions to write to directories like %APPDATA% or %LOCALAPPDATA%. You may need to run the application with administrator privileges.
Process Not Starting:

If the giggletech_oscq.exe process is not starting, ensure that the YAML configuration file is correctly set up and that the executable has the necessary write permissions.
Log Files:

Check the service_log_oscq.txt file for detailed logs of the OSCQuery service. This file will help you troubleshoot issues related to starting and stopping the service.
Future Improvements
Auto-Start on Boot: To ensure that the OSCQuery service is always running, consider setting up the service as a background process that starts automatically with the system.
Admin Privileges: Add a manifest file to request admin privileges if necessary, especially for installations that require access to protected directories or system-wide settings.

*/