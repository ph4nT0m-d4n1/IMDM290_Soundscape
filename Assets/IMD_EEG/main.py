# from emotiv_lsl.emotiv_epoc_x import EmotivEpocX

# if __name__ == "__main__":
#     emotiv_epoc_x = EmotivEpocX()
#     emotiv_epoc_x.main_loop()

from threading import Thread
from emotiv_lsl.emotiv_epoc_x import EmotivEpocX
import hid

def start_device_stream(device_path):
    device = EmotivEpocX(device_path=device_path)
    device.main_loop()

if __name__ == "__main__":
    emotiv_devices = [device for device in hid.enumerate() if device['manufacturer_string'] == 'Emotiv' and device['usage'] == 1]

    threads = []
    for device in emotiv_devices:
        device_path = device['path']
        print(f"Starting thread for device: {device_path}")
        thread = Thread(target=start_device_stream, args=(device_path,))
        threads.append(thread)
        thread.start()
    
    for thread in threads:
        thread.join()
