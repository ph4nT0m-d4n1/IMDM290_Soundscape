using UnityEngine;

public class ColliderScript : MonoBehaviour
{
    GameObject cubeSystem;

    GameObject audioSystem; 

    AudioSystem ASM;
    
    void Start()
    {
        cubeSystem = GameObject.Find("cube_system");
        audioSystem = GameObject.Find("audio_system");

        ASM = audioSystem.GetComponent<AudioSystem>();
    }

    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Trigger")
        {
            GameObject other_cube_system = Instantiate (cubeSystem, other.transform.position, Quaternion.identity);
            other_cube_system.name = "cube_system" + ASM.counter;
            ASM.counter += 1;

            // Debug.Log("cube system counter: " + ASM.counter);
        }
    }
}
