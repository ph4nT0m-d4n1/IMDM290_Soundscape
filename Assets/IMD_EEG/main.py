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

import hid
import sys

import listener as ls

def start_device_stream(device_path):
    """Start the Emotiv device stream in a separate thread."""
    try:
        device = EmotivEpocX(device_path=device_path)
        device.main_loop()
    except Exception as e:
        print(f"Error starting device stream for path {device_path}: {e}")


if __name__ == "__main__":
    emotiv_devices = [device for device in hid.enumerate() if device['manufacturer_string'] == 'Emotiv' and device['usage'] == 2]

    ls_server = ls.OSC_Server("/main_exit")
    
    try:
        ls_server.run()
        
        threads = []
        for device in emotiv_devices:
            device_path = device['path']
            print(f"Starting thread for device: {device_path}")
            thread = Thread(target=start_device_stream, args=(device_path,))
            threads.append(thread)
            thread.start()
    
        for thread in threads:
            thread.join()
    except ls.ShutdownException as e:  # catch the custom exception
        print(e)  # print the exception message
    finally:
        for thread in threads:
            thread.join()
        
    sys.exit(0)  # ensure the script exits cleanly