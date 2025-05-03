using System;
using extOSC;
using TMPro;
using UnityEngine;

public class Receiver_Test : MonoBehaviour
{
    [SerializeField] TMP_Text display; //text object. visible in inspector
    public string address = "/address"; //address our Python script is sending data to
    public OSCReceiver receiver;

    void Start()
    {
        if (receiver == null) //if an OSCReceiver object is not found, add one
        {
            receiver = gameObject.AddComponent<OSCReceiver>();
        }

        Debug.Log($"OSC Receiver started on port {receiver.LocalPort} listening to address {address}"); //logs a start message to the console

        receiver.LocalPort = 9000;
        receiver.Bind(address, MessageReceived);
    }

    /// <summary>
    /// Updates a TMP_Text component based on incoming messages.
    /// Logs incoming and error messages to the console.
    /// </summary>
    /// <param name="message"> the information sent via the Python OSC client </param>
    protected void MessageReceived(OSCMessage message)
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
