using System;
using extOSC;
using TMPro;
using UnityEngine;

public class Receiver : MonoBehaviour
{
    [SerializeField] TMP_Text display;
    public string address = "/address";
    public OSCReceiver receiver;

    void Start()
    {
        if (receiver == null)
        {
            receiver = gameObject.AddComponent<OSCReceiver>();
        }

        Debug.Log($"OSC Receiver started on port {receiver.LocalPort} listening to address {address}");
        receiver.LocalPort = 9000;
        receiver.Bind(address, MessageReceived);
    }

    void MessageReceived(OSCMessage message)
    {
        if (message.Values.Count > 0)
        {
            display.text = message.Values[0].FloatValue.ToString();
            Debug.Log($"Message Received: {message.Values[0].FloatValue}");
        }
        else
        {
            Debug.LogWarning("Received message does not contain a float value.");
        }
    }
}
