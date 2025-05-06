from pythonosc.dispatcher import Dispatcher
from pythonosc import osc_server
from threading import Thread

class ShutdownException(Exception):
    """Custom exception for shutting down the OSC server."""
    pass

class OSC_Server():
    def __init__(self, address:str, port:int=5005):
        self.running = True
        self.server_thread = None
        self.address = address
        self.port = port

    def shutdown(self):
        print(f"shutting down...")
        self.running = False
        self.server.shutdown()
        self.server_thread.join()

    def run(self):
        self.start_osc_server() # start the OSC server
        print("OSC_Server is running...")

    def start_osc_server(self):
        IP = "127.0.0.1" # localhost IP address

        dispatcher = Dispatcher()
        dispatcher.map(self.address, self.handle_shutdown, "exit")

        while True:  # loop until a valid port is found
            try:
                self.server = osc_server.ThreadingOSCUDPServer((IP, self.port), dispatcher) # attempt to create the OSC server
                
                self.server_thread = Thread(target=self.server.serve_forever) # start the server in a separate thread
                self.server_thread.start()
                
                print("OSC Listener Server is up")
                print(f"Listening for messages on port {self.port} and address {self.address}")
                break  # exit the loop if successful
            except OSError as e:
                print(f"Error: {e}. Trying a different port...")
                self.port += 1  # increment the port number and try again

    def handle_shutdown(self, unused_addr, args, int_value):
        if int_value == 0:
            print(f"Exit command received with value {int_value}, shutting down {__name__}.")
            self.shutdown()  # call the shutdown method



