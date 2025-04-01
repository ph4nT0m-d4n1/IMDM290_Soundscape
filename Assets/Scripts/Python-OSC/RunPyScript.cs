using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class RunPyScript : MonoBehaviour
{
    async void Start()
    {
        ProcessStartInfo start = new ProcessStartInfo();
        start.FileName = "c:/Users/danny/AppData/Local/Programs/Python/Python313/python.exe";
        start.Arguments = "c:/Users/danny/uproj/IMDM290_Soundscape/Assets/Scripts/Python-OSC/sender.py";
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

            // Read output asynchronously
            Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
            Task<string> errorTask = process.StandardError.ReadToEndAsync();
            
            // Don't block Unity main thread
            await Task.Run(() => process.WaitForExit());
            
            string output = await outputTask;
            string error = await errorTask;

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