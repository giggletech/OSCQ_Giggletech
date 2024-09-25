use reqwest::Client;
use std::error::Error;
use tokio::time::{sleep, Duration};

#[tokio::main]
async fn main() -> Result<(), Box<dyn Error>> {
    let client = Client::new();

    // Function to start the OSCQuery service
    async fn start_service(client: &Client) {
        let start_url = "http://localhost:8080/start";
        match client.get(start_url).send().await {
            Ok(response) => {
                if response.status().is_success() {
                    println!("Service started successfully at {}", start_url);
                } else {
                    eprintln!("Failed to start the service at {}: {}", start_url, response.status());
                }
            }
            Err(e) => {
                eprintln!("Error connecting to {}: {}", start_url, e);
            }
        }
    }

    // Initially try to start the service
    start_service(&client).await;

    // Step 2: Periodically check if the server is running and retrieve the port
    let check_url = "http://localhost:8080";
    let port_url = "http://localhost:8080/port";
    loop {
        // Check if localhost:8080 is active (Giggletech OSCQuery server)
        match client.get(check_url).send().await {
            Ok(response) => {
                if response.status().is_success() {
                    println!("Giggletech OSCQuery server is running.");
                } else {
                    println!("Giggletech OSCQuery server is NOT running: {}", response.status());
                }
            }
            Err(_) => {
                println!("Giggletech OSCQuery server is NOT running.");
            }
        }

        // Step 3: Retrieve the OSC output port
        match client.get(port_url).send().await {
            Ok(response) => {
                if response.status().is_success() {
                    let port_text = response.text().await?;
                    let port: u16 = port_text.trim().parse().unwrap_or(0);  // Default to 0 if parsing fails
                    println!("OSC output to port \"{}\"", port);

                    // Step 4: If port is 0, start the service again
                    if port == 0 {
                        println!("Port is 0, attempting to start the service...");
                        start_service(&client).await;
                    }
                } else {
                    println!("Failed to retrieve port information: {}", response.status());
                }
            }
            Err(e) => {
                println!("Error connecting to {}: {}", port_url, e);
            }
        }

        // Sleep for a small amount of time (e.g., 5 seconds)
        sleep(Duration::from_secs(5)).await;
    }
}
