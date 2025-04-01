import time
import random
from pythonosc import udp_client

IP = "127.0.0.1"
SEND_PORT = 9000

client = udp_client.SimpleUDPClient(IP, SEND_PORT)

print("Starting OSC sender...")

if __name__ == "__main__":
    for x in range(10):
        num = random.random()
        print(num)
        client.send_message("/address", num)
        time.sleep(1.5)