from pylsl import StreamInlet, resolve_streams
import numpy as np
from hypyp import analyses
from collections import deque
from collections import OrderedDict

import csv
from datetime import datetime, timedelta
import os
import atexit
import argparse

import mne
from pynput import keyboard
from pythonosc import udp_client

from pythonosc.dispatcher import Dispatcher
from pythonosc import osc_server
from threading import Thread


import sys

start_time = datetime.now()

BUFFER_SIZE = 256*3 
sampling_rate = 256
channel_names = ['AF3', 'F7', 'F3', 'FC5', 'T7', 'P7', 'O1', 'O2', 'P8', 'T8', 'FC6', 'F4', 'F8', 'AF4']

freq_bands = {'full-freq': [3, 100], 'Gamma': [35, 100],'Alpha': [8, 12], 'Beta': [12, 35], 'Theta': [4, 8]}
freq_bands = OrderedDict(freq_bands)

raw1 = []

def save_raw_data(start_time, raw):
    # global result_folder_path
    info = mne.create_info(ch_names=channel_names, sfreq=sampling_rate, ch_types='eeg')
    raw_array = mne.io.RawArray(np.array(raw).T, info)
    raw_array.save(f'EEG_raw.fif', overwrite=True)

def cleanup_function():
    save_raw_data(start_time, raw1)

def main():
    atexit.register(cleanup_function)
    streams = resolve_streams()

    inlets = []

    for stream in streams:
        stream_inlet = StreamInlet(stream)
        info = stream_inlet.info()
        name = info.name()

        device_path = name.split(" ")[-1]
        print(f"Stream name: {name}, Device path: {device_path}")

        inlets.append(stream_inlet)
        print(f"Stream for device with path {device_path} initialized.")

    previous_sample = None
    data_buffer = deque(maxlen=BUFFER_SIZE)
    analysis_interval = BUFFER_SIZE // 2

    while server.running:
        sample, timestamp1 = inlets[0].pull_sample()

        if any (el < 10 for el in sample):
            if previous_sample is not None:
                sample = previous_sample

        raw1.append(sample)
        previous_sample = sample

        if sample is not None:
            data_buffer.append(sample)
            if len(data_buffer) >= BUFFER_SIZE:
            # if len(data_buffer1) == BUFFER_SIZE:
                data = np.array(data_buffer)

                # reshape to match hypyp function
                data = data.T.reshape(1, 14, BUFFER_SIZE)

                data_combined = np.array(data)
                analyze_data(data_combined, channel_names, start_time)

                # clear the first half of the buffer 
                for _ in range(analysis_interval):
                    data_buffer.popleft()


def analyze_data(data_combined, channel_names, start_time):
    dummy_data = np.array([data_combined[0], data_combined[0]]) #using dummy data for a workaround hypyp error
    
    complex_signal = analyses.compute_freq_bands(dummy_data, sampling_rate, freq_bands)
    
    print("Complex signal shape:", complex_signal.shape)

    # check if first dimension represents frequency bands or subjects
    if complex_signal.shape[0] == 2: 
        subject_data = complex_signal[0] # get one of the data sets (identical in this case)
        
       #send each frequency band to the OSC client
        for i, band_name in enumerate(freq_bands.keys()):
            if i < subject_data.shape[0]:  #ensuring index is within bounds
                power = get_power(subject_data[i])
                client.send_message(f"/{band_name.lower()}", float(power))
                print(f"Sent {band_name.lower()} power: {power}")
    else:
        print("Unexpected data structure. Expected 2D array.")
        quit()
    
def get_power(signal):
    magnitudes = np.abs(signal)
    return float(np.sqrt(np.mean(magnitudes ** 2)))

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
        print(f"shutting down {__name__}...")
        self.running = False
        self.server_thread.join()
        atexit._run_exitfuncs()

    def run(self):
        # start the OSC server
        self.start_osc_server()
        print("OSC_Server is running...")

    def start_osc_server(self):
        IP = "127.0.0.1"

        dispatcher = Dispatcher()
        dispatcher.map(self.address, self.handle_shutdown, "exit")

        while True:  # loop until a valid port is found
            try:
                # attempt to create the OSC server
                self.server = osc_server.ThreadingOSCUDPServer((IP, self.port), dispatcher)
                print("OSC Server is up")
                print(f"Listening for messages on {self.address}")
                print(f"Listening for messages on port {self.port}")

                # start the server in a separate thread
                self.server_thread = Thread(target=self.server.serve_forever)
                self.server_thread.start()
                break  # exit the loop if successful
            except OSError as e:
                print(f"Error: {e}. Trying a different port...")
                self.port += 1  # increment the port number and try again

    def handle_shutdown(self, unused_addr, args, int_value):
        if int_value == 0:
            print(f"Exit command received with value {int_value}, shutting down {__name__}.")
            self.shutdown()  # call the shutdown method
            

if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("--ip", default="127.0.0.1", help="The ip of the OSC server")
    parser.add_argument("--port", type=int, default=6969, help="The port the OSC server is listening on")
    parser.add_argument("--folderpath", type=str, default="", help="The folder where to save the trial results.")
    args = parser.parse_args()

    server = OSC_Server("/bandOSC_exit", 5006)

    try:
        server.run()
        client = udp_client.SimpleUDPClient(args.ip, args.port)
        main()
    except ShutdownException as e:  # catch the custom exception
        print(e)  # print the exception message
        sys.exit(0)