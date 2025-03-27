using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Prompt : MonoBehaviour
{
    [HideInInspector] public static int counter; //initializing a counter for the prompts
    TMP_Text prompt_text; //the text object

    //initializing prompts in an array
    string [] prompts = {
        "Hello, welcome to Soundscape Therapy",
        "Please answer each question to the best of your ability on a scale of 1 - 10",
        "Use the options on the screen or the number row on your keyboard",
        "The number 0 will be used for the value of 10",
        "When you are ready, we will begin.",
        "Feel free to use the F key to skip any questions!",
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
    
    void NextPrompt()
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
            // Use a switch statement or a dictionary to map key codes to values
            for (int i = 1; i <= 2; i++) // Modify this range as needed
            {
                KeyCode keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), $"Alpha{i}");
                
                if (Input.GetKeyDown(keyCode))
                {
                    responses.Add(i);
                    counter += 1;
                    prompt_text.text = prompts[counter];
                    
                    Debug.Log(counter);
                    Debug.Log("Responses: " + string.Join(",", responses));
                    
                    break; // Exit loop after processing a key press
                }
            }

            // Special handling for 0 key (representing 10)
            if (Input.GetKeyDown(KeyCode.Alpha0))
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
