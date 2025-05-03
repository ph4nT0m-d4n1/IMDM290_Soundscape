using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the functionality of the Run button for executing Python scripts.
/// </summary>
public class PyButton : MonoBehaviour
{
    [SerializeField] private Button runButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private RunPyScript runPyScript; // reference to the RunPyScript component
    
    void Start()
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

    public void OnClickRun()
    {
        runPyScript.RunPythonScript();
    }
    
    public void OnClickExit()
    {
        runPyScript.KillProcess();
    }
    
    private void OnDestroy()
    {
        if (runButton != null)
        {
            runButton.onClick.RemoveListener(OnClickRun);
        }
    }
}