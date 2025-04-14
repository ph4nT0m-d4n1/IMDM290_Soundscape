import matplotlib.pyplot as plt

import mne

sample_data_raw_file = "EEG_raw.fif"
# raw = mne.io.read_raw_fif(sample_data_raw_file, preload=True)

epochs = mne.read_epochs(sample_data_raw_file, proj=True, preload=True, verbose=None)

df = epochs.to_data_frame()

csv_file_path = "faklwj.csv"
df.to_csv(csv_file_path, index=False)