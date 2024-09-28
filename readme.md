
# Giggletech VRChat OSCQuery Helper | Rust Intergration

This repository contains two key components for running the **Giggletech VRChat OSCQuery Helper** service. This service manages communication between the Giggletech system and VRChat through OSC (Open Sound Control) via an OSCQuery server.

### Key Components:
- **Rust Module**: Manages the process lifecycle of the Giggletech OSCQuery server (`giggletech_oscq.exe`), ensuring it is always running and accessible.
- **C# OSCQuery Server**: The core OSCQuery service, handling commands via HTTP and providing information about the TCP and UDP ports used for communication.

## Components Overview

### 1. Rust Module: `oscq_giggletech`

The Rust module is responsible for starting, monitoring, and managing the Giggletech OSCQuery server. It ensures the service is always up and communicates with VRChat avatars and environments in real-time through OSC.

#### **Features:**
- **Initialize and Start**: Manages the `giggletech_oscq.exe` process lifecycle.
- **Check UDP Port**: Polls the OSCQuery server to retrieve the UDP port using an HTTP request.
- **Restart Process**: Automatically restarts the server if a failure is detected or if the UDP port is not returned.
- **Remote Commands**: Interacts with the OSCQuery server over HTTP to start, stop, or manage the service.

#### **Usage**:
```rust
fn main() {
    // Initialize and get the UDP port from the OSCQuery server
    let udp_port = oscq_giggletech::initialize_and_get_udp_port();
    println!("UDP Port: {}", udp_port);
}
```

#### **Key HTTP Endpoints** (provided by the C# server):
- `http://localhost:6969/info`: Get information about TCP, UDP, and HTTP ports.
- `http://localhost:6969/start`: Start the OSCQuery service.
- `http://localhost:6969/port_udp`: Retrieve the current UDP port.
- `http://localhost:6969/port_tcp`: Retrieve the current TCP port.
- `http://localhost:6969/stop`: Shut down the OSCQuery server.

### 2. C# OSCQuery Server: `giggletech_oscq.exe`

The C# server handles the core OSCQuery functionality, including starting and stopping the service and providing port information for communication with VRChat. It listens for HTTP commands and returns the active TCP and UDP ports in use.

#### **Features**:
- **Start/Stop OSCQuery Service**: Start or stop the OSCQuery service via HTTP.
- **Port Management**: Retrieve the TCP and UDP ports for OSC communication.
- **Configuration**: Reads from a YAML configuration file (`config_oscq.yml`) for setup, including HTTP port and service name.

#### **Example YAML Configuration (`config_oscq.yml`)**:
```yaml
httpPort: 6969
serviceName: "Giggletech VRChat Service"
```
The configuration file should be located in the same directory as the executable or in `%APPDATA%\Giggletech`.

## Setup Instructions

### 1. Clone the Repository
```bash
git clone https://github.com/your-repo/giggletech-vrchat-oscq-helper.git
cd giggletech-vrchat-oscq-helper
```

### 2. Build the Projects

#### **Rust Module**:
Ensure that you have Rust installed. Navigate to the Rust project directory (`oscq_giggletech`) and build the project:
```bash
cargo build --release
```

#### **C# OSCQuery Server**:
Ensure that you have .NET installed. Navigate to the C# project directory and build the project:
```bash
dotnet build
```

Create the `config_oscq.yml` file in the same directory as the executable or in `%APPDATA%\Giggletech`. Set up the HTTP port and service name as needed.

### 3. Running the Application

#### **Step 1: Start the C# OSCQuery Server**
Run the C# server directly to start the OSCQuery service:
```bash
./giggletech_oscq.exe
```
The service will begin listening on the HTTP port defined in `config_oscq.yml` (default: 6969).

#### **Step 2: Run the Rust Module**
The Rust module will monitor the `giggletech_oscq.exe` process and manage the server.
```bash
./target/release/oscq_giggletech
```
This will:
- Start the `giggletech_oscq.exe` process if it’s not running.
- Continuously check for the UDP port via `/port_udp` endpoint.
- Restart the `giggletech_oscq.exe` process if it fails.

### 4. Access the HTTP Commands
Once both components are running, you can interact with the OSCQuery service using HTTP clients like `curl` or a web browser:
```bash
curl http://localhost:6969/info
```

## Modular Design for Rust Projects

The Rust module (`oscq_giggletech`) is designed to be modular, enabling easy integration of OSCQuery functionality into any Rust project. It handles the process lifecycle of the C# OSCQuery server, ensuring that the server is always available and automatically restarts if necessary.

### **Key Benefits**:
- **Easy Integration**: Add OSCQuery functionality to your VRChat or other OSC-enabled applications.
- **Automatic Restart**: The module ensures that the OSCQuery server is always running and automatically restarts if it crashes or fails.
- **Remote Control**: Built-in HTTP commands to manage the server remotely.

## Troubleshooting

- **Permission Issues**: If you encounter permission errors, ensure the application has the necessary rights to write to directories like `%APPDATA%` or `%LOCALAPPDATA%`. Run the executable with administrator privileges if needed.
- **Process Not Starting**: Ensure that the `config_oscq.yml` file is correctly set up and the executable has write permissions.
- **Log Files**: Check `service_log_oscq.txt` for detailed logs regarding the OSCQuery service. This can help troubleshoot start/stop issues.

## Future Improvements
- **Auto-Start on Boot**: Consider setting up the service to start automatically with the system.
- **Admin Privileges**: Add a manifest file to request admin privileges where necessary, especially for installations requiring system-wide access.
