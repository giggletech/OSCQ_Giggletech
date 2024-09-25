mod oscq_giggletech;

fn main() {
    // Read the configuration file once
    let config = oscq_giggletech::read_config();

    // Initialize the giggletech process and get the UDP port
    let udp_port = oscq_giggletech::initialize_giggletech(&config);

    // Now, `udp_port` holds the value and can be used later in the program
    println!("Final UDP Port to use: {}", udp_port);

    // You can continue using `udp_port` in your larger codebase as needed.
}
