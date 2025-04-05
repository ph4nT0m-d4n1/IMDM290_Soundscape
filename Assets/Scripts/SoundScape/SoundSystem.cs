using UnityEngine;

public class SoundSystem : MonoBehaviour
{
    GameObject [] questions;
    int processedResponseCount = 0; // track how many responses we've already processed

    void Start()
    {
        SoundSysInit();
    }

    void Update()
    {
        // only process new responses
        if (processedResponseCount < Prompt.responses.Count)
        {
            ProcessNewResponses();
        }
    }

    void SoundSysInit()
    {
        questions = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            questions[i] = transform.GetChild(i).gameObject;
            questions[i].SetActive(false);
            
            // get all AudioSource components for this question GameObject
            AudioSource[] audioSources = questions[i].GetComponents<AudioSource>();

            // disable all AudioSource components using a different loop variable
            for (int j = 0; j < audioSources.Length; j++)
            {
                audioSources[j].enabled = false;
            }
        }
    }

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

                if (value != 0 && value <= 3)
                {
                    q_audio[0].enabled = true;
                    Debug.Log($"Activated q{questionNumber + 1} at LOW based on response");
                }
                else if (value >= 4 && value <= 7)
                {
                    q_audio[1].enabled = true;
                    Debug.Log($"Activated q{questionNumber + 1} at MID based on response");
                }
                else if (value >= 8 && value <= 10)
                {
                    q_audio[2].enabled = true;
                    Debug.Log($"Activated q{questionNumber + 1} at HIGH based on response");
                }
            }
        }
        
        // update the count of processed responses
        processedResponseCount = Prompt.responses.Count;
    }
}
