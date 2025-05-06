using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using Debug = UnityEngine.Debug;

/// <summary>
/// This class handles executing a Python script from within Unity.
/// It launches the Python interpreter as a separate process and records its output.
/// </summary>
public class RunPyScript : MonoBehaviour
{
    #region global variables

    [Header("Python Executable")]
    // path to your Python executable. If Python is in your PATH, you can just use "Python" or "python3"
    // sample mac python3 path
    // /Library/Frameworks/Python.framework/Versions/3.12/bin/python3
    [SerializeField] private string pythonPath = "Python"; 
    public bool isPythonInPath = false; // set to true if Python is in your PATH

    [Header("Script Path")]
    [SerializeField] private string mainScript = "Assets/Scripts/Python-OSC/sender.py"; // relative path to the Python script to be executed

    [SerializeField] private string secondaryScript = null; // relative path to the secondary Python script to be executed, if any
    private int main_processId = 0; //process ID of the main Python script
    private int second_processId = 0; //process ID of the secondary Python script

    #endregion

    void Start()
    {
        if (isPythonInPath)
        {
            pythonPath = "Python"; // use "python3" if on macOS
        }
        else if (isPythonInPath == false)
        {
            pythonPath = "/Library/Frameworks/Python.framework/Versions/3.12/bin/python3"; // replace with your Python path
        }
    }

    public void RunPythonScript()
    {
        _ = RunScript(mainScript); // discard the task to suppress compiler warning CS4014 (call is not awaited)

        if (secondaryScript != null && secondaryScript != "")
        {
            // run the secondary script if provided
            _ = RunScript(secondaryScript);
        }
    }

    /// <summary>
    /// Asynchronously executes the Python script as a separate process.
    /// Captures and logs both standard output and error output from the Python script.
    /// </summary>
    /// <returns> A Task representing the asynchronous operation. </returns>
    async Task RunScript(string scriptPath)
    {
        // configuring the process startup parameters
        ProcessStartInfo pyScript = new ProcessStartInfo
        {
            FileName = pythonPath,                       // path to the Python executable
            Arguments = scriptPath,                      // path to the Python script
            UseShellExecute = false,                     // don't use the OS shell to start the process
            CreateNoWindow = true,                       // run without creating a console window
            RedirectStandardOutput = true,               // capture standard output
            RedirectStandardError = true                 // capture error output
        };

        try
        {
            // starts the Python process
            Process process = Process.Start(pyScript);

            if (process == null)
            {
                Debug.LogError("Failed to start the process.");
                return;
            }

            Debug.Log("Process started with ID: " + process.Id);
            if (main_processId == 0)
            {
                main_processId = process.Id; // store the process ID
            }
            else if (main_processId != 0)
            {
                second_processId = process.Id; // store the process ID
            }
            else
            {
                Debug.LogError("Failed to retrieve the process ID.");
                return;
            }

            
            // read output streams asynchronously to avoid deadlocks
            Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
            Task<string> errorTask = process.StandardError.ReadToEndAsync();

            // wait for the process to exit without blocking Unity's main thread
            await Task.Run(() => process.WaitForExit());

            // retrieve the output and error text
            string output = await outputTask;
            string error = await errorTask;

            // log the process output to Unity's console
            Debug.Log("Process Output:\n" + output);
            // if (!string.IsNullOrEmpty(error))
            // {
            //     Debug.LogError("Process Error:\n" + error);
            // }

            Debug.Log("Process exited with exit code: " + process.ExitCode);
        }
        catch (System.Exception exception)
        {
            // log any exceptions that occur during process execution
            Debug.LogError("An error occurred: " + exception.Message);
        }
    }

    /// <summary>
    /// Terminates the main Python process if it is still active.
    /// </summary>
    public void KillProcess()
    {
        if (main_processId > 0) // the main process is the only one that continues running
        {
            try
            {
                Process process = Process.GetProcessById(main_processId);
                process.WaitForExit(1500); // wait for a few seconds
                if (!process.HasExited)
                {
                    process.Kill();
                    Debug.Log("Process killed.");
                }
                main_processId = 0; // reset the process ID
                second_processId = 0; // reset the secondary process ID
            }
            catch (System.Exception exception)
            {
                Debug.LogError("Failed to kill the process: " + exception.Message);
            }
        }
        else
        {
            Debug.LogWarning("No process to kill.");
        }


    }
}