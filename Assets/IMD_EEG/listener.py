from pythonosc.dispatcher import Dispatcher
from pythonosc import osc_server

import sys
from threading import Thread

class OSC_Server():
    def __init__(self, address:str):
        self.running = True
        self.server_thread = None
        self.address = address

    def shutdown(self):
        print(f"shutting down {__name__}...")
        self.running = False
        print(f"{__name__} has been shut down.")

    def run(self):
        # start the OSC server
        self.start_osc_server()
        print("OSC_Server is running...")

    def start_osc_server(self):
        IP = "127.0.0.1"
        RECEIVE_PORT = 5005

        dispatcher = Dispatcher()
        dispatcher.map(self.address, self.handle_shutdown, "exit")

        while True: # loop until a valid port is found
            try:
                # attempt to create the OSC server
                self.server = osc_server.ThreadingOSCUDPServer((IP, RECEIVE_PORT), dispatcher)
                print("OSC Server is up")
                print(f"Listening for messages on {self.address}")

                # start the server in a separate thread
                self.server_thread = Thread(target=self.server.serve_forever)
                self.server_thread.start()
                break  # exit the loop if successful
            except OSError as e:
                print(f"Error: {e}. Trying a different port...")
                RECEIVE_PORT += 1  # increment the port number and try again


    def handle_shutdown(self, unused_addr, args, int_value):
        if int_value == 0:
            print(f"Exit command received with value {int_value}, shutting down {__name__}.")
            self.shutdown()  # call the shutdown method



