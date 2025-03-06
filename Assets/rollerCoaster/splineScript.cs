using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
[ExecuteInEditMode]
public class CircularFunctionSpline : MonoBehaviour
{
    public SplineContainer splineContainer;
    public int pointCount = 100;
    public float radius = 5f;
    public float waveAmplitude = 2f;
    public float waveFrequency = 2f;

    void OnValidate()
    {
        GenerateCircularSpline();
    }

    void GenerateCircularSpline()
    {
        if (splineContainer == null)
        {
            Debug.LogError("SplineContainer not assigned.");
            return;
        }

        Spline spline = new Spline();
        spline.SetTangentMode(TangentMode.AutoSmooth);
        float angleStep = 2f * Mathf.PI / pointCount;

        for (int i = 0; i < pointCount; i++)
        {
            float angle = i * angleStep;

            // Circle coordinates
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            // Sine wave applied to the Y-axis
            float y = Mathf.Sin(angle * waveFrequency) * waveAmplitude;

            // Create a knot at each point
            BezierKnot knot = new BezierKnot(new Vector3(x, y, z));

            

            spline.Add(knot);
        }
        // Close the spline loop to form a circle
        spline.Closed = true;
        splineContainer.Spline = spline;

    }
}
