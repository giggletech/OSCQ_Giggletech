use std::process::{Command, Stdio};
use std::io::{BufRead, BufReader};

fn main() {
    let exe_path = r"G:\Repos\OSCQ_Giggletech\Giggletech_OSCQuery\bin\Debug\net8.0\giggletech_oscq.exe";
    
    println!("Starting EXE as foreground process...");

    let mut child = Command::new(exe_path)
        .stdout(Stdio::piped())  // Capture stdout
        .stderr(Stdio::piped())  // Capture stderr
        .spawn()
        .expect("Failed to start EXE");

    // Capture and print stdout
    if let Some(stdout) = child.stdout.take() {
        let reader = BufReader::new(stdout);
        for line in reader.lines() {
            match line {
                Ok(l) => println!("stdout: {}", l),
                Err(e) => eprintln!("Error reading stdout: {}", e),
            }
        }
    }

    // Capture and print stderr
    if let Some(stderr) = child.stderr.take() {
        let reader = BufReader::new(stderr);
        for line in reader.lines() {
            match line {
                Ok(l) => eprintln!("stderr: {}", l),
                Err(e) => eprintln!("Error reading stderr: {}", e),
            }
        }
    }

    let status = child.wait().expect("Failed to wait on child process");

    if status.success() {
        println!("EXE completed successfully!");
    } else {
        println!("EXE failed.");
    }
}
