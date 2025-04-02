using UnityEngine;

public class SoundSystem : MonoBehaviour
{
    #region audio sources
    AudioSource [] q1_audio;
    AudioSource [] q2_audio;

    #endregion

    #region game objects
    [SerializeField] GameObject q1;
    [SerializeField] GameObject q2;

    #endregion

    void Start()
    {
        q1_audio = q1.GetComponents<AudioSource>();
        q2_audio = q2.GetComponents<AudioSource>();
    }

    void Update()
    {
        
    }

    void QuestionAnswered()
    {
        
    }
    
}
