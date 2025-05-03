using UnityEngine;
using extOSC;
using TMPro;

public class Sender_TEST : MonoBehaviour
{
    [SerializeField] TMP_InputField input;
    public OSCTransmitter transmitter;
    public string address = "/SentMessage";

    void Start()
    {
        if (transmitter == null)
        {
            transmitter = gameObject.AddComponent<OSCTransmitter>();
        }

        transmitter.RemotePort = 5005;
        transmitter.RemoteHost = "127.0.0.1";
    }

    public void SentMessage()
    {
        var message = new OSCMessage("/SentMessage");
        message.AddValue(OSCValue.String(input.text));

        transmitter.Send(message);    
        Debug.Log($"Message Sent : {input.text}");
    }
}
