using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class RunPyScript : MonoBehaviour
{
    void Start()
    {
        ProcessStartInfo start = new ProcessStartInfo();
        start.FileName = "c:/Users/danny/AppData/Local/Programs/Python/Python313/python.exe"; // Path to your Python executable
        start.Arguments = "c:/Users/danny/uproj/IMDM290_Soundscape/Assets/Scripts/Python-OSC/receiver.py"; // Path to your script
        start.UseShellExecute = false;
        start.CreateNoWindow = true;
        start.RedirectStandardOutput = true;
        start.RedirectStandardError = true;

        try
        {
            Process process = Process.Start(start);

            if (process == null)
            {
                Debug.LogError("Failed to start the process.");
                return;
            }

            // Read output and error streams
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            // Wait for the process to exit
            process.WaitForExit();

            Debug.Log("Process Output:\n" + output);
            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError("Process Error:\n" + error);
            }

            Debug.Log("Process exited with exit code: " + process.ExitCode);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("An error occurred: " + ex.Message);
        }
    }
}