using UnityEngine;

public class SharpMovement : MonoBehaviour
{
    public float distanceFromCamera = 2f;
    public float oscillationDistance = 5f;
    public float speed = 1f;

    private Vector3 startPoint;
    private Vector3 endPoint;
    private float timeOffset;

    void Start()
    {
        Vector3 cameraForward = Camera.main.transform.forward.normalized;
        startPoint = Camera.main.transform.position + cameraForward * distanceFromCamera;
        endPoint = startPoint + cameraForward * oscillationDistance;
        timeOffset = Random.Range(0f, 100f); // Offset for multiple objects
    }

    void Update()
    {
        float t = LerpFraction((Time.time + timeOffset) * speed);
        transform.position = Vector3.Lerp(startPoint, endPoint, t);
    }

    // Custom method to compute a lerp fraction that oscillates between 0 and 1
    float LerpFraction(float time)
    {
        return 0.5f * (1 + Mathf.Sin(time)); // Alternates smoothly from 0 to 1 and back
    }
}