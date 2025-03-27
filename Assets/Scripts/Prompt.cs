using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Prompt : MonoBehaviour
{
    // all debug statements are for testing, ignore them lol
    [HideInInspector] public static int counter; //initializing a counter for the prompts
    public static TMP_Text prompt_text; //the text object

    //initializing prompts in an array
    public static string [] prompts = {
        "Hello, welcome to Soundscape Therapy_",
        "Please answer each question to the best of your ability on a scale of 1 - 10_",
        "Use the number row on your keyboard to provide responses_",
        "The number 0 will be used for the value of 10_",
        "When you are ready, we will begin_",
        "Feel free to use the F key to skip any questions_",
        "How are you feeling today?",
        "Let's explore further",
        "Tell me about your happiness today... how would you quantify your happiness?",
        "How content are you in this current moment?",
        "Are you frustrated in this current moment?",
        "How anxious are you?",
        "How tired are you?",
        "a secret ninth question about the cheesiest bread", //placeholder?
        "Perhaps you are currently feeling a sense of peace after this experience...",
        "Please take a moment to reflect on your state of mind and rate this emotion accordingly..."
    };

    List<int> responses = new List<int>();

    bool skip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        counter = 0;
        skip = false;

        prompt_text = gameObject.GetComponent<TMP_Text>();
        prompt_text.text = prompts[counter];
    }

    // Update is called once per frame
    void Update()
    {
        NextPrompt();
        SkipQuestion();
        AnswerQuestion();
    }
    
    public static void NextPrompt()
    {
        if (counter < 6 || counter == 14 || counter == 7)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                counter += 1;
                prompt_text.text = prompts[counter];
                Debug.Log(counter);
            }
        }
    }

    void SkipQuestion()
    {
        if (counter >= 6 && counter != 14 && counter != 7 && counter < prompts.Length && responses.Count < 8)
        {
            if (Input.GetKeyDown(KeyCode.F)) //skipping functionality
            {
                prompt_text.text = "LOL? FR? (Y / N)";
                skip = true;
            }

            if (skip == true)
            {
                if (Input.GetKeyDown(KeyCode.Y))
                {
                    responses.Add(0);
                    counter += 1;
                    prompt_text.text = prompts[counter];

                    Debug.Log(counter);
                    Debug.Log("Responses: " + string.Join(",", responses));

                    skip = false;
                }
                else if (Input.GetKeyDown(KeyCode.N))
                {
                    prompt_text.text = prompts[counter];

                    skip = false;
                }
            }
            
        }
    }

    void AnswerQuestion()
    {
        if (counter >= 6 && counter != 7 && counter != 14 && counter < prompts.Length && responses.Count < 8)
        {
            for (int i = 1; i <= 9; i++)
            {
                KeyCode keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), $"Alpha{i}");
                
                if (Input.GetKeyDown(keyCode))
                {
                    responses.Add(i);
                    counter += 1;
                    prompt_text.text = prompts[counter];
                    
                    Debug.Log(counter);
                    Debug.Log("Responses: " + string.Join(",", responses));
                    
                    break; // exit loop after hitting a key
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha0)) //special handling for 0 to represent 10
            {
                responses.Add(10);
                counter += 1;
                prompt_text.text = prompts[counter];
                
                Debug.Log(counter);
                Debug.Log("Responses: " + string.Join(",", responses));
            }
        }
    }
}
