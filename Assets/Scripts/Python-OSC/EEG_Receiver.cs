using System;
using extOSC;
using TMPro;
using UnityEngine;

public class EEG_Receiver : MonoBehaviour
{
    //the text objects that will display the values
    [SerializeField] TMP_Text full_freq;
    [SerializeField] TMP_Text gamma_freq;
    [SerializeField] TMP_Text alpha_freq;
    [SerializeField] TMP_Text beta_freq;
    [SerializeField] TMP_Text theta_freq;

    //the float values that will be updated (can be used for other purposes)
    
    public float full_value;
    public float gamma_value;
    public float alpha_value;
    public float beta_value;
    public float theta_value;
    
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

    public void FrequencyReceived(OSCMessage message)
    {
        if (message.Values.Count > 0)
        {
            float value = message.Values[0].FloatValue;
            Debug.Log($"Message Received at {message.Address}: {value}");
            
            switch (message.Address)
            {
                case "/full-freq":
                    if (full_freq != null) full_freq.text = value.ToString("F2");
                    full_value = value;
                    Debug.Log($"Full frequency value: {full_value}");
                    break;
                case "/gamma":
                    if (gamma_freq != null) gamma_freq.text = value.ToString("F2");
                    gamma_value = value;
                    Debug.Log($"Gamma frequency value: {gamma_value}");
                    break;
                case "/alpha":
                    if (alpha_freq != null) alpha_freq.text = value.ToString("F2");
                    alpha_value = value;
                    Debug.Log($"Alpha frequency value: {alpha_value}");
                    break;
                case "/beta":
                    if (beta_freq != null) beta_freq.text = value.ToString("F2");
                    beta_value = value;
                    Debug.Log($"Beta frequency value: {beta_value}");
                    break;
                case "/theta":
                    if (theta_freq != null) theta_freq.text = value.ToString("F2");
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