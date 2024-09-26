mod oscq_giggletech;

fn main() {
    // Call the synchronous function to initialize and get the UDP port
    let udp_port = oscq_giggletech::initialize_and_get_udp_port();

    // Print the UDP port value
    println!("Final UDP Port as i32: {}", udp_port);
}
