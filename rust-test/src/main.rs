use std::process::{Command, Child};
use std::thread;
use std::time::Duration;
use reqwest::blocking::{get, Client};
use reqwest::Error;

// Function to start the giggletech process

use std::path::PathBuf;
use dirs::data_local_dir; // Import the dirs crate for AppData directory resolution

fn run_giggletech() -> Child {
    // Get the path to the `AppData\Local` folder
    let app_data_local_dir = data_local_dir().expect("Failed to get AppData\\Local directory");

    // Construct the full path to the Giggletech executable
    let mut executable_path = PathBuf::from(app_data_local_dir);
    executable_path.push("Giggletech");
    executable_path.push("giggletech_oscq.exe");

    // Start the process using the constructed path
    Command::new(executable_path)
        .spawn()
        .expect("Failed to start process")
}


// Function to retrieve the UDP port value from the server
fn get_udp_port() -> Result<i32, Error> {
    let response = get("http://localhost:6969/port_udp")?;
    let port: i32 = response.text()?.trim().parse().unwrap_or(0); // Default to 0 if parsing fails
    Ok(port)
}

// Function to send the start command to the server
fn start_server() -> Result<(), Error> {
    let client = Client::new();
    client.get("http://localhost:6969/start")
        .send()?
        .error_for_status()?; // Ensure we only succeed on a 2xx status
    Ok(())
}

fn main() {
    // Start the process initially
    let mut process = run_giggletech();

    // Main loop to monitor the process
    loop {
        // Try to retrieve the UDP port
        match get_udp_port() {
            Ok(0) => {
                // If port is 0, start the server
                println!("UDP port is 0, sending start command...");
                if let Err(e) = start_server() {
                    eprintln!("Failed to start server: {}", e);
                }
            }
            Ok(port) => {
                // If port is non-zero, print it
                println!("UDP port: {}", port);
            }
            Err(_) => {
                // If we fail to retrieve the port, restart the process
                eprintln!("Failed to retrieve UDP port, restarting giggletech_oscq.exe...");
                let _ = process.kill(); // Kill the current process if running
                process = run_giggletech(); // Restart the process
            }
        }

        // Sleep for a bit before checking again
        thread::sleep(Duration::from_secs(3)); // Adjust as needed
    }
}
