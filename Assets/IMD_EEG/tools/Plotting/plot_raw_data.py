import mne
import matplotlib

import matplotlib.pyplot as plt
matplotlib.use('TkAgg')
import os

# file_path_1 = "/Users/ngocdinh/Downloads/viz_participant1_raw.fif"
file_path_1 = "../../data/viz_participant2_raw.fif"

# Load the raw EEG file
raw_1 = mne.io.read_raw_fif(file_path_1, preload=True)
raw_1.filter(l_freq=1.0, h_freq=48.0)

# raw_1.crop().load_data() #, tmax=70 tmin=30

# epo1=mne.read_epochs(file_path_1, preload=True)
# epo1.drop_channels("TRIGGER")
# channels = epo1.info['ch_names']

def raw_plot(epo1):
    epo1.plot(n_channels=14, scalings=100, start=0, duration=70)
    plt.savefig('motion-eeg-plot.pdf', dpi=300)
    plt.show()
    
raw_plot(raw_1)