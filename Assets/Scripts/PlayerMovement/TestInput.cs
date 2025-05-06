using UnityEngine;

public class TestInput : MonoBehaviour
{
    void Start()
    {
        Debug.Log("SCRIPT IS RUNNING");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("SPACE PRESSED");
        }
    }
}