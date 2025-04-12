from datetime import datetime

import numpy as np
from mne import Info, create_info
from mne.io.array import RawArray
from pylsl import StreamInlet, resolve_streams
import mne
import matplotlib
import matplotlib.pyplot as plt
matplotlib.use('TkAgg')
import os

# from config import SRATE


def get_info() -> Info:
    ch_names = ['AF3', 'F7', 'F3', 'FC5', 'T7', 'P7',
                'O1', 'O2', 'P8', 'T8', 'FC6', 'F4', 'F8', 'AF4']

    info = create_info(
        sfreq=256,
        ch_names=ch_names,
        ch_types=['eeg'] * len(ch_names)
    )
    
    return info


def main():
    # first resolve an EEG stream on the lab network
    print("looking for an EEG stream...")
    streams = resolve_streams('type', 'EEG')

    # create a new inlet to read from the stream
    inlet = StreamInlet(streams[0])

    previous_sample = None
    buffer = []
    while True:
        if len(buffer) == 256 * 15:  # wait - seconds
            break

        sample, _ = inlet.pull_sample()
        # sample = [el / 1000000 for el in sample]  # convert to microvolts
        if any (el < 10 for el in sample):
            if previous_sample is not None:
                sample = previous_sample

        buffer.append(sample)
        previous_sample = sample

    info = get_info()
    raw = RawArray(np.array(buffer).T, info)
    raw.save("data_raw.fif".format(datetime.now()), overwrite = True)

    # plot
    file_path_1 = "data_raw.fif"

    # Load the raw EEG file
    raw_1 = mne.io.read_raw_fif(file_path_1, preload=True)

    raw_1.crop(tmax=5).load_data()

    channels = raw_1.info['ch_names']

    def raw_plot(raw):
        raw.plot(n_channels=14, scalings=10719657774e-6)
        plt.show()
        
    raw_plot(raw_1)

if __name__ == '__main__':
    main()
