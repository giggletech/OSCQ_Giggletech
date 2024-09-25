mod oscq_giggletech;

#[tokio::main]
async fn main() {
    // Call the single function to initialize and get the UDP port
    let udp_port = oscq_giggletech::initialize_and_get_udp_port().await;

    // Now, `udp_port` holds the value and can be used later in the program
    println!("Final UDP Port to use: {}", udp_port);

    // You can continue using `udp_port` in your larger codebase as needed.
}
