"""This program listens to several addresses
"""
import argparse
import math

from pythonosc.dispatcher import Dispatcher
from pythonosc import osc_server

IP = "127.0.0.1"
RECEIVE_PORT = 5005

def prep_exit():
    """Send exit codes"""
    pass

if __name__ == "__main__":
    dispatcher = Dispatcher() # create a dispatcher to handle incoming OSC messages
    dispatcher.map("/exit_code", print)

    server = osc_server.ThreadingOSCUDPServer(
        (IP, RECEIVE_PORT), dispatcher)
    print("Server is up")
    print(f"IP : {IP}")
    print(f"Receiving Port : {RECEIVE_PORT}")
    server.serve_forever()



