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
    static bool startFade; //determines when to run the fadeText animation coroutine
    static bool userInput; //determines whether the user can provide input at the moment

    #endregion


    #region prompt list
    /// <summary>
    /// Array of predefined prompts/instructions for the therapy session.
    /// Includes welcome messages, instructions, and questions.
    /// </summary>
    public static string[] prompts = {
        "Hello_", //0
        "Welcome to your Soundscape therapy session_", //1
        "Please answer each question to the best of your ability on a scale of 1 - 10_", //2
        "Use the number row on your keyboard to provide responses_", //3
        "The number 0 will be used for the value of 10_", //4
        "The F Key will be used to skip questions_", //5
        "When you are ready, we will begin_", //6

        "How clear is your mind at this moment?_", //Q1 - Vinyl SFX
        "Have you fully woken up today?_", //Q2 - Pluck
        "Do you feel a sense of relaxation?_", //Q3 - Pad
        "Is your mind racing at this moment?_", //Q4 - Lead
        "How comfortable are you right now?_", //Q5 - Bass
        "Are you enjoying the weather today?_", //Q6 - SFX2
        "Do you think your day will get better?_", //Q7 - Arp
        "How long since you last spoke to your family?_", //Q8 - Main Lead
        "Have you been feeling loved recently?_", //Q9 - Vocal Chop
        "Do you love yourself?_", //Q10 - Drums
        
        "I'm glad you stayed_", //17
        "Enjoy this moment while it's here_", //18
        "Enjoy the sound and sight of You_" //19
    };

    #endregion

    void Awake()
    {
        // reset counter and skip values
        counter = 0;
        skip = false;
        startFade = false;
        userInput = true; // allow user input at the start
        nextButton.SetActive(true); // ensure next button is visible at the start
    }
    
    void Start()
    {
        // initialize prompt system
        promptText = GameObject.Find("Prompt Text").GetComponent<TMP_Text>();
        promptText.text = prompts[counter];
    }

    void Update()
    {
        if (userInput == true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                NextPrompt(); // handles moving to the next prompt
            }

            // special handling for interactive questions (after initial instructions)
            if (counter > 6 && counter < 17)
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
                StartCoroutine(FadeText(prompts[counter]));
                startFade = false;
            }
            
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
            // set prompt text and log debug information
            // initialize text fade effect
            startFade = true;

            Debug.Log("Current Prompt: " + counter);
            Debug.Log("Responses: " + string.Join(",", responses));
        }
        else
        {
            // if counter is out of bounds, display a warning and reset to the last valid prompt
            Debug.LogWarning("Attempted to access prompt outside of array bounds.");
        }
    }
    
    /// <summary>
    /// Handles navigation to the next prompt during instruction screens.
    /// Advances to the next prompt when spacebar is pressed.
    /// </summary>
    public static void NextPrompt()
    {
        // only allow advancing during specific prompts (initial instructions, final prompts)
        if (counter <= 6 || counter >= 17)
        {
            counter += 1;
            UpdatePromptText();
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
            promptText.text = "Are you sure you wish to proceed? (Y / N)_";
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

    #endregion

    #region coroutine method
    /// <summary>
    /// Coroutine to fade the prompt text in and out.
    /// Fades out the current text, updates it, and fades it back in.   
    /// </summary>
    public static IEnumerator FadeText(string nextText)
    {
        userInput = false; // disable user input during fade

        float alpha = 1f;

        while(alpha > 0.0f)
        {
            alpha -= Time.deltaTime / 1f ; //deltaTime / fadeTime
            promptText.color = new Color(promptText.color.r, promptText.color.g, promptText.color.b, alpha);
            yield return null;
        }

        promptText.text = nextText;

        yield return new WaitForSeconds(0.5f);
        while(alpha < 1.0f)
        {
            alpha += Time.deltaTime / 1f ; //deltaTime / fadeTime
            promptText.color = new Color(promptText.color.r, promptText.color.g, promptText.color.b, alpha);
            yield return null;
        }

        userInput = true; // re-enable user input after fade
    }

    #endregion
}