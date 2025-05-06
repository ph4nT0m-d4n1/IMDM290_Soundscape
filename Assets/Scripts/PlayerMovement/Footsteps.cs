using UnityEngine;

public class Footsteps : MonoBehaviour
{
    public AudioSource footstepsSound;
    //public AudioSource sprintSound;

    void Update()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            footstepsSound.enabled = true;
            /*
            if (Input.GetKey(KeyCode.LeftShift))
            {
                footstepsSound.enabled = false;
                sprintSound.enabled = true;
            }
            else
            {
                footstepsSound.enabled = true;
                sprintSound.enabled = false;
            }
            */
        }
        else
        {
            footstepsSound.enabled = false;
            //sprintSound.enabled = false;
        }
    }
}
