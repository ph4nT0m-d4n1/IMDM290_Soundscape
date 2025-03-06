using Unity.VisualScripting;
using UnityEngine;

public class CubeGrow : MonoBehaviour
{
    float time;
    float xScale;
    float yScale;
    float zScale;
    void Start()
    {
        xScale = gameObject.transform.localScale.x;
        yScale = gameObject.transform.localScale.y;
        zScale = gameObject.transform.localScale.z;
    }

    void Update()
    {
        time += Time.deltaTime * AudioSystem.audioAmp;
        gameObject.transform.localScale = new Vector3(0.1f, yScale * time, 0.1f);

        ColorfulGrowth();
    }

    void ColorfulGrowth()
    {
        Renderer cubeRenderer = gameObject.GetComponent<Renderer>(); //obtaining the cubes' renderer component

        if (CubeParent.time < 60)
        {
            float hue = 1f;
            Color color = Color.HSVToRGB(hue, Mathf.Cos(AudioSystem.audioAmp / 10f), 0.5f); 
            cubeRenderer.material.color = color;
        }
        else if (CubeParent.time > 60 && CubeParent.time < 80)
        {
            float hue = 1f;
            Color color = Color.HSVToRGB(hue, Mathf.Cos(AudioSystem.audioAmp / 10f), 2f + Mathf.Cos(time)); 
            cubeRenderer.material.color = color;
        }
        else
        {
            for (int i = 0; i < 100; i++)
            {
                float hue = (float) i / 100;
                Color color = Color.HSVToRGB(Mathf.Abs(hue * Mathf.Cos(AudioSystem.audioAmp)), Mathf.Cos(AudioSystem.audioAmp / 10f), 2f + Mathf.Cos(time)); 
                cubeRenderer.material.color = color;
            }
        }
    }
}
