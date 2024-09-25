use std::fs;
use std::path::PathBuf;
use std::process::{Command, Child};
use std::thread;
use std::time::Duration;
use dirs::data_local_dir;
use serde::Deserialize;
use reqwest::blocking::{get, Client};
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

// Function to get the UDP port from the server
fn get_udp_port(port: u16) -> Result<i32, Error> {
    let url = format!("http://localhost:{}/port_udp", port);
    let response = get(&url)?;
    let port_value: i32 = response.text()?.trim().parse().unwrap_or(0); // Return 0 if parsing fails
    Ok(port_value)
}

// Function to send the start command to the server
fn start_server(port: u16) -> Result<(), Error> {
    let client = Client::new();
    let url = format!("http://localhost:{}/start", port);
    client.get(&url)
        .send()?
        .error_for_status()?;
    Ok(())
}

// Function to handle the entire process flow
fn handle_giggletech(config: &Config, process: &mut Child) {
    match get_udp_port(config.httpPort) {
        Ok(0) => {
            // If UDP port is 0, start the server
            println!("UDP port is 0, sending start command...");
            if let Err(e) = start_server(config.httpPort) {
                eprintln!("Failed to start server: {}", e);
            }
        }
        Ok(port_value) => {
            // Print the UDP port if it's non-zero
            println!("UDP port: {}", port_value);
        }
        Err(_) => {
            // If the request fails, restart the process
            eprintln!("Failed to retrieve UDP port, restarting giggletech process...");
            let _ = process.kill(); // Kill the current process
            *process = run_giggletech(); // Restart the process
        }
    }
}

fn main() {
    // Read the configuration file once
    let config = read_config();

    // Start the giggletech process initially
    let mut process = run_giggletech();

    // Main loop focuses only on calling the handler
    loop {
        handle_giggletech(&config, &mut process);

        // Sleep before the next iteration
        thread::sleep(Duration::from_secs(10));
    }
}
