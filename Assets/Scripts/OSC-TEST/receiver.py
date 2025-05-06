from pythonosc.dispatcher import Dispatcher
from pythonosc import osc_server
import other  # Import the other script
import threading

IP = "127.0.0.1"
RECEIVE_PORT = 5005

# create an instance of OtherScript
other_script_instance = other.Other_TEST()

def test_exit(unused_addr, args, int_value):
    print(f"Received value: {int_value}")
    if int_value == 3:
        print(f"Exit command received with value {int_value}, shutting down OtherScript.")
        other_script_instance.shutdown()  # Call the shutdown method

if __name__ == "__main__":
    address = "/test/1"
    dispatcher = Dispatcher()
    dispatcher.map(address, test_exit, "test")

    server = osc_server.ThreadingOSCUDPServer((IP, RECEIVE_PORT), dispatcher)
    
    print("Server is up")
    print(f"IP : {IP}")
    print(f"Receiving Port : {RECEIVE_PORT}")
    print(f"Listening for messages on '{address}'")
    print("Send an int value of 3 to this address to shut down OtherScript")
    
    # run OtherScript in a separate thread
    other_script_thread = threading.Thread(target=other_script_instance.run)
    other_script_thread.start()
    
    server.serve_forever()
