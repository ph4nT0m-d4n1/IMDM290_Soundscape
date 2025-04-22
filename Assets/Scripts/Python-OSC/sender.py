"""
A simple Python OSC client

"""
import time
import random
from pythonosc import udp_client

#127.0.0.1 is the IP reference to the local host, aka your device

IP = "127.0.0.1" #make sure this matches the IP address in the OSC receiver object
SEND_PORT = 9000 #make sure this matches the LocalPort in Receiver.cs

client = udp_client.SimpleUDPClient(IP, SEND_PORT)


if __name__ == "__main__":
    for x in range(5):
        num = random.random()
        print(num)
        client.send_message("/address", num)
        time.sleep(1.5)