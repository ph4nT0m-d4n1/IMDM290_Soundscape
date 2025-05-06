import time
from pythonosc.dispatcher import Dispatcher
from pythonosc import osc_server
import threading
import sys

class OtherScript:
    def __init__(self):
        self.running = True
        self.server_thread = None

    def shutdown(self):
        print("Shutting down OtherScript...")
        self.running = False
        sys.exit(0)
        print("OtherScript has been shut down.")

    def run(self):
        # Start the OSC server
        self.start_osc_server()

        while self.running:
            # Simulate some work
            print("OtherScript is running...")
            time.sleep(1)  # Sleep to simulate work being done
        print("Exited the run loop.")

    def start_osc_server(self):
        IP = "127.0.0.1"
        RECEIVE_PORT = 5005

        dispatcher = Dispatcher()
        dispatcher.map("/test/1", self.handle_shutdown, "test")

        self.server = osc_server.ThreadingOSCUDPServer((IP, RECEIVE_PORT), dispatcher)
        print("OSC Server is up")
        print(f"Listening for messages on '/test/1'")

        # Start the server in a separate thread
        self.server_thread = threading.Thread(target=self.server.serve_forever)
        self.server_thread.start()

    def handle_shutdown(self, unused_addr, args, int_value):
        print(f"Received value: {int_value}")
        if int_value == 3:
            print(f"Exit command received with value {int_value}, shutting down OtherScript.")
            self.shutdown()  # Call the shutdown method

if __name__ == "__main__":
    other_script_instance = OtherScript()
    other_script_instance.run()
