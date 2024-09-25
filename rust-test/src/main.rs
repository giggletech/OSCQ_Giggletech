use std::fs;
use std::path::PathBuf;
use std::process::{Command, Child};
use std::thread;
use std::time::Duration;
use dirs::data_local_dir;
use serde::Deserialize;
use reqwest::blocking::{get, Client};
use reqwest::Error;
use serde_yaml; // Import `serde_yaml` to parse YAML configuration

// Define the structure of your YAML configuration
#[derive(Debug, Deserialize)]
struct Config {
    httpPort: u16, // Match the field name in the YAML file
}

// Function to read the config_oscq.yml file and extract the port number
fn read_config(config_path: &PathBuf) -> Result<Config, serde_yaml::Error> {
    let config_str = fs::read_to_string(config_path).expect("Failed to read config_oscq.yml");
    serde_yaml::from_str(&config_str)
}

// Function to start the giggletech process
fn run_giggletech() -> Child {
    let app_data_local_dir = data_local_dir().expect("Failed to get AppData\\Local directory");

    let mut executable_path = PathBuf::from(&app_data_local_dir);
    executable_path.push("Giggletech");
    executable_path.push("giggletech_oscq.exe");

    Command::new(executable_path)
        .spawn()
        .expect("Failed to start process")
}

// Function to retrieve the UDP port value from the server
fn get_udp_port(port: u16) -> Result<i32, Error> {
    let url = format!("http://localhost:{}/port_udp", port);
    let response = get(&url)?;
    let port_value: i32 = response.text()?.trim().parse().unwrap_or(0); // Default to 0 if parsing fails
    Ok(port_value)
}

// Function to send the start command to the server
fn start_server(port: u16) -> Result<(), Error> {
    let client = Client::new();
    let url = format!("http://localhost:{}/start", port);
    client.get(&url)
        .send()?
        .error_for_status()?; // Ensure we only succeed on a 2xx status
    Ok(())
}

fn main() {
    // Get the path to the AppData\Local\Giggletech folder
    let mut config_path = data_local_dir().expect("Failed to get AppData\\Local directory");
    config_path.push("Giggletech");
    config_path.push("config_oscq.yml");

    // Read the configuration file and extract the port number
    let config = read_config(&config_path).expect("Failed to parse config_oscq.yml");

    // Start the process initially
    let mut process = run_giggletech();

    // Main loop to monitor the process
    loop {
        match get_udp_port(config.httpPort) {
            Ok(0) => {
                // If port is 0, send the start command
                println!("UDP port is 0, sending start command...");
                if let Err(e) = start_server(config.httpPort) {
                    eprintln!("Failed to start server: {}", e);
                }
            }
            Ok(port_value) => {
                // If port is non-zero, print it
                println!("UDP port: {}", port_value);
            }
            Err(_) => {
                // If we fail to retrieve the port, restart the process
                eprintln!("Failed to retrieve UDP port, restarting giggletech_oscq.exe...");
                let _ = process.kill(); // Kill the current process if running
                process = run_giggletech(); // Restart the process
            }
        }

        // Sleep for a bit before checking again
        thread::sleep(Duration::from_secs(2)); // Adjust as needed
    }
}
