using Unity.VisualScripting;
using UnityEngine;

public class CubeFluctuate : MonoBehaviour
{
    float xScale;
    float yScale;
    float zScale;
    float time;
    void Start()
    {
        xScale = gameObject.transform.localScale.x;
        yScale = gameObject.transform.localScale.y;
        zScale = gameObject.transform.localScale.z;
    }

    void Update()
    {
        time += Time.deltaTime * AudioSystem.audioAmp;
        // Debug.Log(time);

        gameObject.transform.localScale = new Vector3(0.1f, yScale * time * AudioSystem.audioAmp, 0.1f);
        
        ColorfulFluctuations();
    }

    void ColorfulFluctuations()
    {
        Renderer cubeRenderer = gameObject.GetComponent<Renderer>();

        if (CubeParent.time < 80)
        {
            float hue = 1f;
            Color color = Color.HSVToRGB(Mathf.Abs(hue), Mathf.Cos(AudioSystem.audioAmp / 10f), 1.5f + Mathf.Cos(time)); 
            cubeRenderer.material.color = color;
        }
        else
        {
            float hue = 1f;
            Color color = Color.HSVToRGB(Mathf.Abs(hue * Mathf.Cos(time)), Mathf.Cos(AudioSystem.audioAmp / 10f), 2f + Mathf.Cos(time)); 
            cubeRenderer.material.color = color;
        }
    }
}
