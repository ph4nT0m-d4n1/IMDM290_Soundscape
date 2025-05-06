using UnityEngine;
using extOSC;

public class Sender_TEST : MonoBehaviour
{
    public OSCTransmitter transmitter;
    public OSCTransmitter transmitter2;
    private string address1 = "/main_exit";
    public string address2 = "/bandOSC_exit";
    private bool hasTriggeredShutdown = false;

    private string ipAddress = "127.0.0.1";

    void Start()
    {
        if (transmitter == null)
        {
            transmitter = gameObject.AddComponent<OSCTransmitter>();

            if (transmitter2 == null)
            {
                transmitter2 = gameObject.AddComponent<OSCTransmitter>();
            }
        }

        transmitter.RemotePort = 5005;

        transmitter2.RemotePort = 5006;

        transmitter.RemoteHost = ipAddress;
        transmitter2.RemoteHost = ipAddress;
    }

    public void TriggerServerShutdown()
    {
        if (!hasTriggeredShutdown)
        {
            var message2 = new OSCMessage(address2);
            message2.AddValue(OSCValue.Int(0));
            transmitter2.Send(message2);
            Debug.Log("emotiv band-OSC shutdown message sent");

            var message = new OSCMessage(address1);
            message.AddValue(OSCValue.Int(0));
            transmitter.Send(message);
            Debug.Log("emotiv main-OSC shutdown message sent");
            
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