using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{

    public float mouseSensitivity = 100f;

    float xRotation = 0f;
    float YRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        //around x axis (Look up and down)
        xRotation -= mouseY;

        //avoid over-rotate 
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //around y axis (Look up and down)
        YRotation += mouseX;

        //applying rotations
        transform.localRotation = Quaternion.Euler(xRotation, YRotation, 0f);

    }
}