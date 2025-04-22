using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

/// <summary>
/// This class handles executing a Python script from within Unity.
/// It launches the Python interpreter as a separate process and records its output.
/// </summary>
public class RunPyScript : MonoBehaviour
{
    #region global reference variables

    //path to your Python executable. If Python is in your PATH, you can just use "Python" or "python3"
    [SerializeField] private string pythonPath = "Python"; 
    
    //sample mac python3 path
    ///"Library/Frameworks/Python.framework/Versions/3.12/bin/python3"

    [SerializeField] private string scriptPath = "Assets/Scripts/Python-OSC/sender.py"; //path to the Python script to be executed
    private int processId = 0; //process ID of the running Python script

    #endregion

    void Start()
    {
        // check if the Python executable is available
        if (string.IsNullOrEmpty(pythonPath))
        {
            Debug.LogError("Python path is not set.");
            return;
        }

        // check if the script path is valid
        if (string.IsNullOrEmpty(scriptPath))
        {
            Debug.LogError("Script path is not set.");
            return;
        }
    }

    public void RunPythonScript()
    {

        _ = RunScript(); // discard the task to suppress compiler warning CS4014 (call is not awaited)

    }

    /// <summary>
    /// Asynchronously executes the Python script as a separate process.
    /// Captures and logs both standard output and error output from the Python script.
    /// </summary>
    /// <returns> A Task representing the asynchronous operation. </returns>
    async Task RunScript()
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
            processId = process.Id; // store the process ID

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
            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError("Process Error:\n" + error);
            }

            Debug.Log("Process exited with exit code: " + process.ExitCode);
        }
        catch (System.Exception exception)
        {
            // log any exceptions that occur during process execution
            Debug.LogError("An error occurred: " + exception.Message);
        }
    }

    /// <summary>
    /// Terminates the running Python process if it is still active.
    /// This method is not very graceful, but it works.
    /// </summary>
    public void KillProcess()
    {
        if (processId > 0)
        {
            try
            {
                Process process = Process.GetProcessById(processId);
                if (!process.HasExited)
                {
                    process.Kill();
                    Debug.Log("Process killed.");
                }
            }
            catch (System.Exception exception)
            {
                Debug.LogError("Failed to kill the process: " + exception.Message);
            }
        }
    }
}