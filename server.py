import socket
import time

# Define the server's IP address and port
HOST = '127.0.0.1'
PORT = 8888

# Create a socket object
server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

# Bind the socket to the address and port
server_socket.bind((HOST, PORT))

# Listen for incoming connections
server_socket.listen(1)
print(f"Server is listening on {HOST}:{PORT}")

while True:
    # Accept a connection from a client
    client_socket, client_address = server_socket.accept()
    print(f"Connected to {client_address}")

    try:
        while True:
            # Get the current timestamp
            current_time = time.time()
            timestamp = str(current_time)

            # Send the timestamp to the client
            client_socket.send(timestamp.encode('utf-8'))

            # Wait for one second before sending the next timestamp
            time.sleep(1)

    except KeyboardInterrupt:
        print("Server stopped by user.")
        break

    finally:
        # Close the client socket
        client_socket.close()

# Close the server socket
server_socket.close()