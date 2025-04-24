using System;
using extOSC;
using TMPro;
using UnityEngine;

public class EEG_Receiver : MonoBehaviour
{
    [SerializeField] TMP_Text full_freq;
    [SerializeField] TMP_Text gamma_freq;
    [SerializeField] TMP_Text alpha_freq;
    [SerializeField] TMP_Text beta_freq;
    [SerializeField] TMP_Text theta_freq;
    
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
            receiver.Bind(address, MessageReceived);
            Debug.Log($"OSC Receiver started on port {receiver.LocalPort} listening to address {address}");
        }
    }

    protected void MessageReceived(OSCMessage message)
    {
        if (message.Values.Count > 0)
        {
            float value = message.Values[0].FloatValue;
            Debug.Log($"Message Received at {message.Address}: {value}");
            
            switch (message.Address)
            {
                case "/full-freq":
                    if (full_freq != null) full_freq.text = value.ToString("F2");
                    break;
                case "/gamma":
                    if (gamma_freq != null) gamma_freq.text = value.ToString("F2");
                    break;
                case "/alpha":
                    if (alpha_freq != null) alpha_freq.text = value.ToString("F2");
                    break;
                case "/beta":
                    if (beta_freq != null) beta_freq.text = value.ToString("F2");
                    break;
                case "/theta":
                    if (theta_freq != null) theta_freq.text = value.ToString("F2");
                    break;
            }
        }
        else
        {
            Debug.LogWarning($"Received message at {message.Address} does not contain a valid float value.");
        }
    }
}