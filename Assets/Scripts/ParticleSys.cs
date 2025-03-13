using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleSys : MonoBehaviour
{
    ParticleSystem particleSys;
    float simSpeed;
    void Start()
    {
        particleSys = gameObject.GetComponent<ParticleSystem>();
        var col = particleSys.colorOverLifetime;
        col.enabled = true;
        
        // Debug.Log(particleSys);
    }

    void Update()
    {
        DancingParticles();
        ColorFulParticles();

        if (CubeParent.time >= 31.25f && CubeParent.time <= 32f)
        {
            particleSys.Stop();
        }
        
    }

    void DancingParticles()
    {
        var main = particleSys.main; //must turn this into a variable before use
        main.simulationSpeed = simSpeed;

        simSpeed = AudioSystem.audioAmp * 0.75f;
        // Debug.Log(simSpeed);
    }

    void ColorFulParticles()
    {
        var col = particleSys.colorOverLifetime;
        
        Gradient grad = new Gradient();
        
        grad.SetKeys(new GradientColorKey[] { new GradientColorKey(Color.blue, 0.0f), new GradientColorKey(Color.red, 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) } );

        col.color = grad;
    }

    
}
