using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class AudioSystem : MonoBehaviour
{
    [HideInInspector] public int counter;
    [HideInInspector] public static int FFTSIZE = 1024;
    [HideInInspector] public static float[] samples;
    [HideInInspector] public static float audioAmp = 0f;

    public GameObject particleSysHolder;
    ParticleSystem particleSys2;

    GameObject cube_sys2;
    GameObject sys2_parent;
    AudioSource source;
    float audioTime;
    void Start()
    {
        counter = 1;
        // Debug.Log("counter starting... " + counter);

        source = GetComponent<AudioSource>();   
        samples = new float[FFTSIZE];   

        particleSys2 = particleSysHolder.GetComponent<ParticleSystem>();
    }

    void Update()
    {
        audioTime += Time.deltaTime  * audioAmp;
        // Debug.Log(audioTime);

        ManageAudioSys();
        ManageCubeSys();
        ManageParticleSys();
    }

    void ManageCubeSys()
    {
        cube_sys2 = GameObject.Find("cube_system2");

        if (cube_sys2)
        {
            sys2_parent = cube_sys2.transform.GetChild(0).gameObject;
            // Debug.Log(sys2_parent);

            if (CubeParent.time >= 168)
            {
                sys2_parent.SetActive(false);
            }
            else if (CubeParent.time >= 250)
            {
                 sys2_parent.SetActive(true);
            }
        }
    }

    void ManageAudioSys() //imported from Prof. Lee's AudioSpectrum script
    {
        source.GetSpectrumData(samples, 0, FFTWindow.Hanning);

        audioAmp = 0f;
        for (int i = 0; i < FFTSIZE; i++)
        {
            audioAmp += samples[i];
        }   
    }

    void ManageParticleSys()
    {
        //523 , cube sys 4
        if (audioTime >= 230f)
        {
            particleSys2.Play();
        }
    }
}
