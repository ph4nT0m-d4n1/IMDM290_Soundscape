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

    while True:
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
        # get first subject's data only (since both are identical)
        subject_data = complex_signal[0]
        
       #send each frequency band to the OSC client
        for i, band_name in enumerate(freq_bands.keys()):
            if i < subject_data.shape[0]:  #ensuring index is within bounds
                power = get_power(subject_data[i])
                client.send_message(f"/{band_name.lower()}", float(power))
                print(f"Sent {band_name.lower()} power: {power}")
    else:
        print("Unexpected data structure from compute_freq_bands")
        quit()
    
def get_power(signal):
    magnitudes = np.abs(signal)
    return float(np.sqrt(np.mean(magnitudes ** 2)))


if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("--ip", default="127.0.0.1", help="The ip of the OSC server")
    parser.add_argument("--port", type=int, default=6969, help="The port the OSC server is listening on")
    parser.add_argument("--folderpath", type=str, default="", help="The folder where to save the trial results.")
    args = parser.parse_args()

    # HARDCODED_MAIN_RESULTS_PATH = "User-study"
    # result_folder_path = os.path.join(HARDCODED_MAIN_RESULTS_PATH,args.folderpath)
    # os.makedirs(result_folder_path,exist_ok=True)

    client = udp_client.SimpleUDPClient(args.ip, args.port)
    
    main()