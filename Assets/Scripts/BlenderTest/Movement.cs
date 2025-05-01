using UnityEngine;

public class Movement : MonoBehaviour
{
    public float radius = 20f;              // Max distance from camera
    public float speed = 1f;               // Oscillation speed
    public float angleOffset = 90f;         // Angle around the camera (in degrees)

    private Vector3 directionFromCamera;
    private Vector3 cameraPosition;
    private float timeOffset;

    void Start()
    {
        // Get camera's position
        cameraPosition = Camera.main.transform.position;

        // Compute angle in radians
        float angleRad = angleOffset * Mathf.Deg2Rad;

        // Use camera's right and forward to define a horizontal plane
        Vector3 right = Camera.main.transform.right;
        Vector3 forward = Vector3.Cross(Vector3.up, right).normalized;

        // Get a point on the circle's perimeter relative to camera
        directionFromCamera = (right * Mathf.Cos(angleRad) + forward * Mathf.Sin(angleRad)).normalized;

        timeOffset = Random.Range(0f, 100f); // Optional variety
    }

    void Update()
    {
        float t = LerpFraction((Time.time + timeOffset) * speed);
        transform.position = cameraPosition + directionFromCamera * radius * t;
    }
    float LerpFraction(float time)
    {
        return 0.5f * (1 + Mathf.Sin(time)); // Alternates smoothly from 0 to 1 and back
    }
}