using UnityEngine;
using System.Collections;

/// <summary>
/// Manages the sound system for a Soundscape Therapy application.
/// Handles activation of audio sources based on user responses to prompts.
/// Each question has multiple audio sources that can be activated based on the response value.
/// </summary>
public class SoundSystem : MonoBehaviour
{
    GameObject [] questions;
    int processedResponseCount = 0; // track how many responses we've already processed

    #region main methods

    void Awake()
    {
        SoundSysInit(); // ensure the questions are initialized and audio sources are disabled
        Debug.Log("SoundSystem initialized with " + questions.Length + " questions.");
    }
    void Start()
    {
        processedResponseCount = Prompt.responses.Count; // initialize processed response count
        Debug.Log("SoundSystem Start: Processed response count initialized to " + processedResponseCount);
    }

    void Update()
    {
        // only process new responses
        if (processedResponseCount < Prompt.responses.Count)
        {
            ProcessNewResponses();
        }
    }

    /// <summary>
    /// Initializes the sound system by setting up the track layers and setting their volumes to zero.
    /// </summary>
    /// <remarks>
    /// This method retrieves all child GameObjects of the SoundSystem GameObject,
    /// which represent individual questions, and disables their AudioSource components.
    /// </remarks>
    void SoundSysInit()
    {
        questions = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            questions[i] = transform.GetChild(i).gameObject;
            questions[i].SetActive(false);
            
            // get all AudioSource components for this question GameObject
            AudioSource[] audioSources = questions[i].GetComponents<AudioSource>();

            // set all AudioSource components' volume to 0
            for (int j = 0; j < audioSources.Length; j++)
            {
                audioSources[j].volume = 0f;
            }

        }
    }

    /// <summary>
    /// Processes new responses from the Prompt class and activates corresponding questions.
    /// </summary>
    /// <remarks>
    /// This method checks the responses list for new entries since the last processing,
    /// determines which question corresponds to each response, and activates the appropriate
    /// audio sources based on the response value.
    /// It assumes that the response value corresponds to the question index + 1,
    /// where a value of 1 activates the first question, 2 activates the second, and so on.
    /// </remarks>
    void ProcessNewResponses()
    {
        // process only the new responses
        for (int i = processedResponseCount; i < Prompt.responses.Count; i++)
        {
            int value = Prompt.responses[i];
            
            // determine which question was answered based on response index
            int questionNumber = i; // zero-based index for array access
            
            // check if the response value matches the question index+1
            // this assumes response value 1 activates questions[0], value 2 activates questions[1], etc
            if (questionNumber < questions.Length)
            {
                questions[i].SetActive(true);

                AudioSource[] q_audio = questions[i].GetComponents<AudioSource>();

                if (value != 0 && value <= 2)
                {
                    StartCoroutine(FadeInAudio(q_audio[0]));
                    Debug.Log($"Activated q{questionNumber + 1} at SUPER-LOW based on response");
                }
                else if (value >= 3 && value <= 4)
                {
                    StartCoroutine(FadeInAudio(q_audio[1]));
                    Debug.Log($"Activated q{questionNumber + 1} at LOW based on response");
                }
                else if (value >= 5 && value <= 6)
                {
                    StartCoroutine(FadeInAudio(q_audio[2]));
                    Debug.Log($"Activated q{questionNumber + 1} at MID based on response");
                }
                else if (value >= 7 && value <= 8)
                {
                    StartCoroutine(FadeInAudio(q_audio[3]));
                    Debug.Log($"Activated q{questionNumber + 1} at HIGH based on response");
                }
                else if (value >= 9 && value <= 10)
                {
                    StartCoroutine(FadeInAudio(q_audio[4]));
                    Debug.Log($"Activated q{questionNumber + 1} at SUPER-HIGH based on response");
                }
            }
        }
        
        // update the count of processed responses
        processedResponseCount = Prompt.responses.Count;
    }
    #endregion

    #region coroutine method
    /// <summary>
    /// Gradually increases the volume of an AudioSource to create a fade-in effect.
    /// The volume will increase from 0 to 1 over a period of approximately 7 seconds.
    /// </summary>
    public static IEnumerator FadeInAudio (AudioSource audio)
    {
        float volume = 0f;
        float fadeTime = 7f; // duration of the fade-in effect in seconds

        while(volume < 1.0f)
        {
            volume += Time.deltaTime / fadeTime ; //deltaTime / fadeTime
            audio.volume = volume;
            yield return null;
        }
    }
    #endregion
}
