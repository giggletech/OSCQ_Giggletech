/*
    Giggletech OSCQuery Rust Module
    
    About this Module:

    This module handles the initialization and management of the `giggletech_oscq.exe` process.
    It reads configuration from a YAML file located in the `AppData\Local\Giggletech` directory,
    starts the process, and continuously checks the `httpPort` for the UDP port via an HTTP request
    to the `/port_udp` endpoint.

    If the UDP port is `0`, the module sends a start command to the `/start` endpoint.
    If a valid non-zero UDP port is received, it is returned for further use in the main program.
    The process is automatically restarted if the UDP port cannot be retrieved, ensuring that
    the process stays running until a valid UDP port is received.

    Key Functions:
    - `initialize_and_get_udp_port()` (async): 
        - Reads the config, starts the process, and continuously checks for a valid UDP port.
        - Returns the UDP port once it is successfully retrieved.


*/


use std::fs;
use std::path::PathBuf;
use std::process::{Command, Child};
use std::time::Duration;
use dirs::data_local_dir;
use serde::Deserialize;
use tokio::time::sleep;
use reqwest::Error;
use serde_yaml;

// Struct to deserialize the YAML config
#[derive(Debug, Deserialize)]
struct Config {
    httpPort: u16,
}

// Function to read and parse the YAML config file
fn read_config() -> Config {
    let mut config_path = data_local_dir().expect("Failed to get AppData\\Local directory");
    config_path.push("Giggletech");
    config_path.push("config_oscq.yml");

    let config_str = fs::read_to_string(config_path).expect("Failed to read config_oscq.yml");
    serde_yaml::from_str(&config_str).expect("Failed to parse config_oscq.yml")
}

// Function to start the giggletech process
fn run_giggletech() -> Child {
    let mut executable_path = data_local_dir().expect("Failed to get AppData\\Local directory");
    executable_path.push("Giggletech");
    executable_path.push("giggletech_oscq.exe");

    Command::new(executable_path)
        .spawn()
        .expect("Failed to start giggletech process")
}

// Async function to get the UDP port from the server
async fn get_udp_port(port: u16) -> Result<i32, Error> {
    let url = format!("http://localhost:{}/port_udp", port);
    let response = reqwest::get(&url).await?;
    let port_value: i32 = response.text().await?.trim().parse().unwrap_or(0); // Return 0 if parsing fails
    Ok(port_value)
}

// Async function to send the start command to the server
async fn start_server(port: u16) -> Result<(), Error> {
    let client = reqwest::Client::new();
    let url = format!("http://localhost:{}/start", port);
    client.get(&url)
        .send()
        .await?
        .error_for_status()?;
    Ok(())
}

// Combined async function to initialize, handle the giggletech process, and return the UDP port
pub async fn initialize_and_get_udp_port() -> i32 {
    // Step 1: Read the configuration
    let config = read_config();

    // Step 2: Start the giggletech process
    let mut process = run_giggletech();

    // Step 3: Loop until we get a non-zero UDP port
    loop {
        match get_udp_port(config.httpPort).await {
            Ok(0) => {
                // If UDP port is 0, send the start command
                println!("UDP port is 0, sending start command...");
                if let Err(e) = start_server(config.httpPort).await {
                    eprintln!("Failed to start server: {}", e);
                }
            }
            Ok(port_value) => {
                // If we get a valid non-zero port, return it
                println!("UDP port: {}", port_value);
                return port_value;
            }
            Err(_) => {
                // If the request fails, restart the process
                eprintln!("Failed to retrieve UDP port, restarting giggletech process...");
                let _ = process.kill(); // Kill the current process
                process = run_giggletech(); // Restart the process
            }
        }

        // Sleep asynchronously before the next check
        sleep(Duration::from_secs(1)).await;
    }
}
