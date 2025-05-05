"""This program listens to several addresses
"""
from pythonosc.dispatcher import Dispatcher
from pythonosc import osc_server

IP = "127.0.0.1"
RECEIVE_PORT = 5005

def test_exit(unused_addr, args, int_value):
    print(f"Received value: {int_value}")
    if int_value == 3:
        print(f"Exit command received with value {int_value}, shutting down OtherScript.")

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
    
    server.serve_forever()



