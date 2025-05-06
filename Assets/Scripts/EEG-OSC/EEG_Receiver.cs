using System;
using extOSC;
using UnityEngine;

public class EEG_Receiver : MonoBehaviour
{
    public static float full_value;
    public static float gamma_value;
    public static float alpha_value;
    public static float beta_value;
    public static float theta_value;
    
    public string[] addresses = {"/full-freq", "/gamma", "/alpha", "/beta", "/theta"};
    public OSCReceiver receiver;

    void Start()
    {
        if (receiver == null)
        {
            receiver = gameObject.AddComponent<OSCReceiver>();
        }

        // set port BEFORE binding
        receiver.LocalPort = 6969;
        
        // bind each address
        foreach (string address in addresses)
        {
            receiver.Bind(address, FrequencyReceived);
            Debug.Log($"OSC Receiver started on port {receiver.LocalPort} listening to address {address}");
        }
    }

    protected void FrequencyReceived(OSCMessage message)
    {
        if (message.Values.Count > 0)
        {
            float value = message.Values[0].FloatValue;
            Debug.Log($"Message Received at {message.Address}: {value}");
            
            switch (message.Address)
            {
                case "/full-freq":
                    full_value = value;
                    Debug.Log($"Full frequency value: {full_value}");
                    break;
                case "/gamma":
                    gamma_value = value;
                    Debug.Log($"Gamma frequency value: {gamma_value}");
                    break;
                case "/alpha":
                    alpha_value = value;
                    Debug.Log($"Alpha frequency value: {alpha_value}");
                    break;
                case "/beta":
                    beta_value = value;
                    Debug.Log($"Beta frequency value: {beta_value}");
                    break;
                case "/theta":
                    theta_value = value;
                    Debug.Log($"Theta frequency value: {theta_value}");
                    break;
            }
        }
        else
        {
            Debug.LogWarning($"Received message at {message.Address} does not contain a valid float value.");
        }
    }
}