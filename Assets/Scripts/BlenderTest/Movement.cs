using UnityEngine;

public class Movement : MonoBehaviour
{
    private float radius;                // Orbit radius
    public float orbitSpeed = 100f;           // Degrees per second
    public float verticalAmplitude = 1f;    // Height of sine wave
    public float verticalFrequency = 1f;     //Speed of sine wave

    private Vector3 orbitAxis = Vector3.up;  // Axis to orbit around (Y by default)
    private float currentAngle;
    private Vector3 orbitCenter;
    private float timeCounter = 0f;

    void Start()
    {
        orbitCenter = Camera.main.transform.position;

        Vector3 toObject = transform.position - orbitCenter;
        toObject.y = 0f;

        currentAngle = Mathf.Atan2(toObject.z, toObject.x) * Mathf.Rad2Deg;

        radius = toObject.magnitude + 0.25f;

        UpdatePosition(); // Set initial position
    }

    void Update()
    {
        orbitCenter = Camera.main.transform.position; // Update if camera moves
        currentAngle += orbitSpeed * Time.deltaTime;
        timeCounter += Time.deltaTime;
        UpdatePosition();
    }

    void UpdatePosition()
    {
        // Calculate direction vector based on current angle
        float rad = currentAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * radius;

        float verticalOffset = Mathf.Sin(timeCounter * verticalFrequency * Mathf.PI * 2f) * verticalAmplitude;

        // Align the orbit to the camera's horizontal plane
        Vector3 forward = Camera.main.transform.forward;
        forward.y = 0f;
        forward.Normalize();
        Vector3 right = Camera.main.transform.right;

        // Convert offset to world space relative to camera
        Vector3 worldOffset = right * offset.x + forward * offset.z;

        Vector3 finalPosition = orbitCenter + worldOffset + new Vector3(0f, verticalOffset, 0f);
        transform.position = finalPosition;
    }
}