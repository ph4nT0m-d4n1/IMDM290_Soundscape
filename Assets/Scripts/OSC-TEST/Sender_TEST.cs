using UnityEngine;
using extOSC;

public class Sender_TEST : MonoBehaviour
{
    public OSCTransmitter transmitter;
    private string address = "/test/1";
    private bool hasTriggeredShutdown = false;

    void Start()
    {
        if (transmitter == null)
        {
            transmitter = gameObject.AddComponent<OSCTransmitter>();
        }

        transmitter.RemotePort = 5005;
        transmitter.RemoteHost = "127.0.0.1";
    }

    public void TriggerServerShutdown()
    {
        if (!hasTriggeredShutdown)
        {
            var message = new OSCMessage(address);
            message.AddValue(OSCValue.Int(3));
            transmitter.Send(message);
            Debug.Log("Shutdown message sent to OSC server");
            hasTriggeredShutdown = true;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !hasTriggeredShutdown)
        {
            TriggerServerShutdown();
        }
    }
}