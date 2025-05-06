using System.Threading.Tasks;
using extOSC;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the functionality of the buttons for executing and terminating Python scripts.
/// </summary>
public class PyButton : MonoBehaviour
{
    #region global variables

    [Header("Button & Script Settings")]
    [SerializeField] private Button runButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private RunPyScript runPyScript; // reference to the RunPyScript component

    [Header("OSC Settings")]
    public OSCTransmitter mainTransmitter;
    public OSCTransmitter bandTransmitter;
    string main_address = "/main_exit";
    string band_address = "/bandOSC_exit";
    bool hasTriggeredShutdown = false;

    string ipAddress = "127.0.0.1";

    #endregion
    void Start()
    {
        PyButtonInit();
        OSC_ServerInit();
    }

    void PyButtonInit()
    {
        // Get the button component
        if (runButton == null)
        {
            runButton = GameObject.Find("runButton").GetComponent<Button>();
        }
        if (exitButton == null)
        {
            exitButton = GameObject.Find("exitButton").GetComponent<Button>();
        }
        
        // Add click listener
        runButton.onClick.AddListener(OnClickRun);
        exitButton.onClick.AddListener(OnClickExit);
        
        // If runPyScript is not assigned in the inspector, try to find it in the scene
        if (runPyScript == null)
        {
            runPyScript = FindFirstObjectByType<RunPyScript>();
            if (runPyScript == null)
            {
                Debug.LogError("RunPyScript component not found in the scene! Please assign it in the Inspector.");
            }
        }
    }
    
    void OSC_ServerInit()
    {
        if (mainTransmitter == null)
        {
            mainTransmitter = gameObject.AddComponent<OSCTransmitter>();

            if (bandTransmitter == null)
            {
                bandTransmitter = gameObject.AddComponent<OSCTransmitter>();
            }
        }

        mainTransmitter.RemoteHost = ipAddress;
        mainTransmitter.RemotePort = 5005;

        bandTransmitter.RemoteHost = ipAddress;
        bandTransmitter.RemotePort = 5006;
    }

    public void OnClickRun()
    {
        runPyScript.RunPythonScript();
        hasTriggeredShutdown = false; // reset the shutdown flag when the script is run
    }
    
    public void OnClickExit()
    {
        if (!hasTriggeredShutdown)
        {
            var band_message = new OSCMessage(band_address);
            band_message.AddValue(OSCValue.Int(0));
            bandTransmitter.Send(band_message);
            Debug.Log("emotiv band-OSC shutdown message sent");

            var main_message = new OSCMessage(main_address);
            main_message.AddValue(OSCValue.Int(0));
            mainTransmitter.Send(main_message);
            Debug.Log("emotiv main shutdown message sent");
            
            hasTriggeredShutdown = true;
        }
        runPyScript.KillProcess();
    }

    // private void OnDestroy()
    // {
    //     if (runButton != null)
    //     {
    //         runButton.onClick.RemoveListener(OnClickRun);
    //     }
    // }
}