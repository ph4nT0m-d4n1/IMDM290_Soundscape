using System;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages a guided prompt system for a Soundscape Therapy application.
/// Handles sequential prompts, user responses, and navigation through a series of questions.
/// </summary>
public class Prompt : MonoBehaviour
{
    #region class variables
    [SerializeField] GameObject nextButton; //reference to the game object containing the button

    public static int counter; //counter to keep track of the current prompt index
    public static List<int> responses = new List<int>(); //list to store user responses

    static TMP_Text promptText; //prompt text being displayed
    static bool skip; //indicates whether the current question is being skipped
    static bool startFade = false; //determines when to run the fadeText animation coroutine in Update()

    #endregion


    #region prompt list
    /// <summary>
    /// Array of predefined prompts/instructions for the therapy session.
    /// Includes welcome messages, instructions, and questions.
    /// </summary>
    public static string[] prompts = {
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
        "Please take a moment to reflect on your state of mind and rate this emotion accordingly...",
        "Thank you for responding to our survey_",
    };

    #endregion

    
    void Start()
    {
        // reset counter and skip values
        counter = 0;
        skip = false;

        // initialize prompt system
        promptText = GameObject.Find("Prompt Text").GetComponent<TMP_Text>();
        promptText.text = prompts[counter];
    }

    void Update()
    {
        NextPrompt(); //handles moving to the next prompt

        // special handling for interactive questions (after initial instructions)
        if (counter >= 6 && counter != 14 && counter != 7)
        {
            nextButton.SetActive(false); // hide next button during interactive questions
            
            // check for skipping or answering questions
            SkipQuestion();
            AnswerQuestion();
        }
        else
        {
            nextButton.SetActive(true); // show next button during instructions
        }

        if (startFade == true)
        {
            StartCoroutine(fadeText(prompts[counter]));
            startFade = false;
        }
    }


    #region main methods
    /// <summary>
    /// Updates the prompt text based on the current counter.
    /// Handles displaying the current prompt or a final thank you message.
    /// </summary>
    public static void UpdatePromptText()
    {
        if (counter >= 0 && counter < prompts.Length)
        {
            // Set prompt text and log debug information
            //promptText.text = prompts[counter];
            startFade = true;

            Debug.Log("Responses: " + string.Join(",", responses));
        }
        else //UNTESTED
        {
            // Display final message when all prompts are completed
            promptText.text = "Please enjoy your soundscape_";
            Debug.LogWarning("Attempted to access prompt outside of array bounds.");
        }
    }
    
    /// <summary>
    /// Handles navigation to the next prompt during instruction screens.
    /// Advances to the next prompt when spacebar is pressed.
    /// </summary>
    static void NextPrompt()
    {
        // Only allow advancing during specific prompts (initial instructions, final prompts)
        if (counter < 6 || counter == 14 || counter == 7)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                counter += 1;
                UpdatePromptText();
            }
        }
    }

    /// <summary>
    /// Manages the question skipping mechanism.
    /// Allows users to skip a question by pressing 'F' and confirming.
    /// </summary>
    static void SkipQuestion()
    {
        if (Input.GetKeyDown(KeyCode.F)) // initiate skip process when 'F' is pressed
        {
            promptText.text = "LOL? FR? (Y / N)";
            skip = true;
        }

        if (skip) //handle skip confirmation
        {
            if (Input.GetKeyDown(KeyCode.Y)) // adds a 0 (skipped) response and move to next prompt
            {
                responses.Add(0);
                counter += 1;
                UpdatePromptText();
                skip = false;
            }
            else if (Input.GetKeyDown(KeyCode.N)) // cancel skip
            {
                UpdatePromptText();
                skip = false;
            }
        }
    }

    /// <summary>
    /// Handles user responses to interactive questions.
    /// Captures numeric key inputs (1-9) with special handling for 0 (representing 10).
    /// </summary>
    static void AnswerQuestion()
    {
        // Check for number key inputs (1-9)
        for (int i = 1; i <= 9; i++)
        {
            KeyCode keyCode = (KeyCode)Enum.Parse(typeof(KeyCode), $"Alpha{i}");
            
            if (Input.GetKeyDown(keyCode))
            {
                // records response and move to next prompt
                responses.Add(i);
                counter += 1;
                UpdatePromptText();
                
                break; // exits loop after capturing a key press
            }
        }

        // special handling for 0 key (representing 10)
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            responses.Add(10);
            counter += 1;
            UpdatePromptText();
        }
    }

    public static IEnumerator fadeText(string newText)
    {
        float alpha = 1f;

        while(alpha > 0.0f)
        {
            alpha -= Time.deltaTime / 1f ; //deltaTime / fadeTime
            promptText.color = new Color(promptText.color.r, promptText.color.g, promptText.color.b, alpha);
            yield return null;
        }
        promptText.text = newText;

        yield return new WaitForSeconds(1);
        while(alpha < 1.0f)
        {
            alpha += Time.deltaTime / 1f ; //deltaTime / fadeTime
            promptText.color = new Color(promptText.color.r, promptText.color.g, promptText.color.b, alpha);
            yield return null;
        }
    }

    #endregion
}