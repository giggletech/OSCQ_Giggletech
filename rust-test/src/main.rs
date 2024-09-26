mod oscq_giggletech;

#[tokio::main]
async fn main() {
    // Call the single function to initialize and get the UDP port
    let udp_port = oscq_giggletech::initialize_and_get_udp_port().await;

    // Print the UDP port value
    println!("UDP Port value (before conversion): {}", udp_port);

    // Convert the UDP port to i32 (if it's not already)
    let udp_port_i32 = udp_port as i32;

    // Now, udp_port_i32 holds the i32 value of the UDP port
    println!("Final UDP Port as i32: {}", udp_port_i32);

    // You can continue using udp_port_i32 in your larger codebase as needed.
}
