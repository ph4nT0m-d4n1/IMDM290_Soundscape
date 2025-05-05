# from emotiv_lsl.emotiv_epoc_x import EmotivEpocX

# if __name__ == "__main__":
#     emotiv_epoc_x = EmotivEpocX()
#     emotiv_epoc_x.main_loop()
"""Small example to start Emotiv device streams in separate threads using hid and emotiv_lsl library.
This script enumerates Emotiv devices and starts a thread for each device to handle its data stream."""
from threading import Thread
from emotiv_lsl.emotiv_epoc_x import EmotivEpocX

from pythonosc.dispatcher import Dispatcher
from pythonosc import osc_server

import threading
import hid
import sys

def start_device_stream(device_path):
    """Start the Emotiv device stream in a separate thread."""
    try:
        device = EmotivEpocX(device_path=device_path)
        device.main_loop()
    except Exception as e:
        print(f"Error starting device stream for path {device_path}: {e}")

def start_osc_server():
    """Start the OSC listener in a separate thread."""
    IP = "127.0.0.1"
    RECEIVE_PORT = 5005

    dispatcher = Dispatcher()
    dispatcher.map("/test/1", handle_shutdown(), "test")

    server = osc_server.ThreadingOSCUDPServer((IP, RECEIVE_PORT), dispatcher)
    print("OSC Server is up")
    print(f"Listening for messages on '/test/1'")

    # Start the server in a separate thread
    server_thread = threading.Thread(target=server.serve_forever)
    server_thread.start()

def handle_shutdown(unused_addr, args, int_value):
    print(f"Received value: {int_value}")
    if int_value == 3:
        print(f"Exit command received with value {int_value}, shutting down OtherScript.")
        sys.exit(0)

if __name__ == "__main__":
    emotiv_devices = [device for device in hid.enumerate() if device['manufacturer_string'] == 'Emotiv' and device['usage'] == 2]

    threads = []
    for device in emotiv_devices:
        device_path = device['path']
        print(f"Starting thread for device: {device_path}")
        thread = Thread(target=start_device_stream, args=(device_path,))
        threads.append(thread)
        thread.start()
    
    for thread in threads:
        thread.join()
