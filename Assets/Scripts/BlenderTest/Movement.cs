using UnityEngine;

public class Movement : MonoBehaviour
{
    public float radius = 10f;                // Orbit radius
    public float orbitSpeed = 100f;           // Degrees per second
    public float angleOffset = 0f;           // Starting angle in degrees

    private Vector3 orbitAxis = Vector3.up;  // Axis to orbit around (Y by default)
    private float currentAngle;
    private Vector3 orbitCenter;

    void Start()
    {
        orbitCenter = Camera.main.transform.position;
        currentAngle = angleOffset;
        UpdatePosition(); // Set initial position
    }

    void Update()
    {
        orbitCenter = Camera.main.transform.position; // Update if camera moves
        currentAngle += orbitSpeed * Time.deltaTime;
        UpdatePosition();
    }

    void UpdatePosition()
    {
        // Calculate direction vector based on current angle
        float rad = currentAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * radius;

        // Align the orbit to the camera's horizontal plane
        Vector3 forward = Camera.main.transform.forward;
        forward.y = 0f;
        forward.Normalize();
        Vector3 right = Camera.main.transform.right;

        // Convert offset to world space relative to camera
        Vector3 worldOffset = right * offset.x + forward * offset.z;

        transform.position = orbitCenter + worldOffset;
    }
}