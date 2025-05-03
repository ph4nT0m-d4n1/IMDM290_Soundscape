from pythonosc.dispatcher import Dispatcher
from pythonosc import osc_server

# Based on python-osc examples - https://github.com/attwad/python-osc

# Sets up Dispatcher which listens for messages and acts when receiving

IP = "127.0.0.1"
RECEIVE_PORT = 5005

if __name__ == "__main__":
    dispatcher = Dispatcher()
    dispatcher.map("/SentMessage", print)

    server = osc_server.ThreadingOSCUDPServer(
        (IP, RECEIVE_PORT), dispatcher)
    
    
    print("Server is up")
    print(f"IP : {IP}")
    print(f"Receiving Port : {RECEIVE_PORT}")
    server.serve_forever()