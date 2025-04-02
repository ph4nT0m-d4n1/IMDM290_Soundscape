using System.Diagnostics;
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
    //private Process OSC_Process; //reference to the Python process

    public string pythonPath = "Python"; // Path to your Python executable. If Python is in your system PATH, you can use just "Python".
    
    public string scriptPath = "c:/Users/danny/uproj/IMDM290_Soundscape/Assets/Scripts/Python-OSC/sender.py"; //file path to the Python script to be executed.

    #endregion

    void Start()
    {
        _=RunPy(); // discard the task to suppress compiler warning CS4014 (call is not awaited)
    }

    /// <summary>
    /// Asynchronously executes the Python script as a separate process.
    /// Captures and logs both standard output and error output from the Python script.
    /// </summary>
    /// <returns> A Task representing the asynchronous operation. </returns>
    async Task RunPy()
    {
        // configuring the process startup parameters
        ProcessStartInfo senderPy = new ProcessStartInfo();
        senderPy.FileName = pythonPath;                // set the executable to our Python interpreter
        senderPy.Arguments = scriptPath;               // pass the script path as an argument
        senderPy.UseShellExecute = false;              // don't use the OS shell to start the process
        senderPy.CreateNoWindow = false;                // run without creating a console window
        senderPy.RedirectStandardOutput = true;        // capture the standard output
        senderPy.RedirectStandardError = true;         // capture the error output

        try
        {
            // starts the Python process
            Process OSC_Process = Process.Start(senderPy);
            
            if (OSC_Process == null)
            {
                Debug.LogError("Failed to start the process.");
                return;
            }

            // read output streams asynchronously to avoid deadlocks
            Task<string> outputTask = OSC_Process.StandardOutput.ReadToEndAsync();
            Task<string> errorTask = OSC_Process.StandardError.ReadToEndAsync();
            
            // wait for the process to exit without blocking Unity's main thread
            await Task.Run(() => OSC_Process.WaitForExit());
            
            // retrieve the output and error text
            string output = await outputTask;
            string error = await errorTask;

            // log the process output to Unity's console
            Debug.Log("Process Output:\n" + output);
            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError("Process Error:\n" + error);
            }

            Debug.Log("Process exited with exit code: " + OSC_Process.ExitCode);
        }
        catch (System.Exception exception)
        {
            // log any exceptions that occur during process execution
            Debug.LogError("An error occurred: " + exception.Message);
        }
    }
}