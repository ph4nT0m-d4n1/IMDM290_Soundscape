using System;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using extOSC;
using UnityEngine.UIElements;

/// <summary>
/// Manages a guided prompt system for a Soundscape Therapy application.
/// Handles sequential prompts, user responses, and navigation through a series of questions.
/// </summary>
public class Prompt : MonoBehaviour
{
    #region class variables

    [Header("Prompt System")]
    // [SerializeField] GameObject nextButton; //reference to the game object containing the button

    [HideInInspector] public static int counter; //counter to keep track of the current prompt index
    public static List<int> responses = new List<int>(); //list to store user responses

    static TMP_Text promptText; //prompt text being displayed
    static TMP_Text F_next; //suggestive text for user input
    static UnityEngine.UI.Image background; //black background for UI canvas
    static bool skip; //indicates whether the current question is being skipped
    static bool startPromptFade; //determines when to run the fadePromptText animation coroutine
    static bool startNextFade; //determines when to run the fadeNextText animation coroutine
    static bool startBackgroundFade; //determines when to run the fadeOutBackground animation coroutine
    static bool userInput; //determines whether the user can provide input at the moment

    [Header("RunPy")]
    [SerializeField] private RunPyScript runPy; //reference to the game object containing the RunPyScript component

    [Header("OSC Settings")]
    public OSCTransmitter mainTransmitter;
    public OSCTransmitter bandTransmitter;
    string main_address = "/main_exit";
    string band_address = "/bandOSC_exit";
    bool hasTriggeredShutdown = false;

    string ipAddress = "127.0.0.1";

    #endregion


    #region prompt list
    /// <summary>
    /// Array of predefined prompts/instructions for the therapy session.
    /// Includes welcome messages, instructions, and questions.
    /// </summary>
    public static string[] prompts = {
        "Hello.", //0
        "Welcome to your Soundscape Therapy Session.", //1
        "Please answer each question to the best of your ability on a scale of 1 - 10.", //2
        "Use the number row on your keyboard to provide responses.", //3
        "The number 0 will be used for the value of 10.", //4
        "The F Key can be used to skip questions.", //5
        "When you are ready, we will begin...", //6

        "How clear is your mind at this moment?", //Q1 - Vinyl SFX
        "Have you fully woken up today?", //Q2 - Pluck
        "Do you feel a sense of relaxation?", //Q3 - Pad
        "Is your mind racing at this moment?", //Q4 - Lead
        "How comfortable are you right now?", //Q5 - Bass
        "Are you enjoying the weather today?", //Q6 - SFX2
        "Do you think your day will get better?", //Q7 - Arp
        "How often do you see your family?", //Q8 - Main Lead
        "Have you been feeling loved recently?", //Q9 - Vocal Chop
        "Do you love yourself?", //Q10 - Drums
        
        "I'm glad you stayed.", //17
        "Enjoy this moment while it's here.", //18
        "Enjoy the sound and sight of You.", //19
        "You may use WASD to move around the space.", //20
        "You may use the mouse to look around.", //21
        "When you are done, press Q to stop the EEG processing", //22
        "" //23
    };

    #endregion

    #region unity methods
    void Awake()
    {
        // reset counter and skip values
        counter = 0;
        skip = false;
        startPromptFade = false;
        userInput = true; // allow user input at the start

        // initialize prompt system
        promptText = GameObject.Find("Prompt Text").GetComponent<TMP_Text>();
        F_next = GameObject.Find("F Next").GetComponent<TMP_Text>();
        background = GameObject.Find("Background").GetComponent<UnityEngine.UI.Image>();
        
        // nextButton.SetActive(true); // ensure next button is visible at the start

        OSC_ServerInit();
    }
    
    void Start()
    {
        // set initial prompt text and suggestive text
        F_next.text = "Press Space to continue..."; // set initial suggestive text
        promptText.text = prompts[counter];

        Debug.Log(prompts.Length);

        // initialize RunPyScript component
        runPy.RunPythonScript();
        hasTriggeredShutdown = false; // reset the shutdown flag when the script is run
    }

    void Update()
    {
        if (userInput == true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (counter < prompts.Length - 1)
                {
                    NextPrompt(); // move to the next prompt
                }
                else
                {
                    Debug.Log("End of prompts reached.");
                    
                    startNextFade = true;
                    if (counter == 7 && startNextFade == true)
                    {
                        StartCoroutine(FadeNextText("")); // clear suggestive text
                        startNextFade = false;
                    }
                }
            }

            // special handling for interactive questions (after initial instructions)
            if (counter > 6 && counter < 17 || counter == 23)
            {
                // nextButton.SetActive(false); // hide next button during interactive questions

                startNextFade = true;
                startBackgroundFade = true;
                if (counter > 6 && counter < 8 && startNextFade == true)
                {
                    StartCoroutine(FadeNextText("")); // clear suggestive text
                    startNextFade = false;
                }
                
                // check for skipping or answering questions
                SkipQuestion();
                AnswerQuestion();
            }
            else if (counter == 17)
            {
                // nextButton.SetActive(true); // show next button during instructions
                startNextFade = true;
                if (startNextFade == true)
                {
                    StartCoroutine(FadeNextText("Press Space to Continue...")); // bring back suggestive text
                    startNextFade = false;
                }
            }
            else if (counter == 22)
            {
                startNextFade = true;
                if (startNextFade == true)
                {
                    StartCoroutine(FadeNextText("")); // clear suggestive text
                    startNextFade = false;
                }
            }

            if (startPromptFade == true)
            {
                StartCoroutine(FadePromptText(prompts[counter]));
                startPromptFade = false;
            }

            if (startBackgroundFade == true)
            {
                StartCoroutine(FadeOutBackground(background.color.a));
                startBackgroundFade = false;
            }
        }

        // OSC shutdown message
        if (!hasTriggeredShutdown) // check if the last prompt is reached
        {
            if (Input.GetKeyDown(KeyCode.Q)) // allow user to quit python processes
            {
                var band_message = new OSCMessage(band_address);
                band_message.AddValue(OSCValue.Int(0));
                bandTransmitter.Send(band_message);
                Debug.Log("emotiv band-OSC shutdown message sent");

                var main_message = new OSCMessage(main_address);
                main_message.AddValue(OSCValue.Int(0));
                mainTransmitter.Send(main_message);
                Debug.Log("emotiv main shutdown message sent");
                
                hasTriggeredShutdown = true;
                
                runPy.KillProcess();
            }
        }
    }

    #endregion

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
            startPromptFade = true;

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

    #region coroutine methods
    /// <summary>
    /// Coroutine to fade the prompt text in and out.
    /// Fades out the current text, updates it, and fades it back in.   
    /// </summary>
    public static IEnumerator FadePromptText(string nextText)
    {
        userInput = false; // disable user input during fade

        float alpha = 1f;
        float fadeTime = 1f; // duration of the fade effect in seconds

        while(alpha > 0.0f)
        {
            alpha -= Time.deltaTime / (fadeTime * 0.80f); 
            promptText.color = new Color(promptText.color.r, promptText.color.g, promptText.color.b, alpha);
            yield return null;
        }

        promptText.text = nextText;

        if (counter > 6 && counter < 17)
        {
            yield return new WaitForSeconds(4f); // longer pause between prompts if the user is being asked questions
        }
        else 
        {
            yield return new WaitForSeconds(0.5f);
        }
        
        
        while(alpha < 1.0f)
        {
            alpha += Time.deltaTime / (fadeTime * 1f);
            promptText.color = new Color(promptText.color.r, promptText.color.g, promptText.color.b, alpha);
            yield return null;
        }

        userInput = true; // re-enable user input after fade
    }

    public static IEnumerator FadeNextText(string nextText)
    {
        float alpha = 1f;
        float fadeTime = 2f; // duration of the fade effect in seconds

        while (alpha > 0.0f)
        {
            alpha -= Time.deltaTime / (fadeTime * 0.75f);
            F_next.color = new Color(F_next.color.r, F_next.color.g, F_next.color.b, alpha);
            yield return null;
        }

        F_next.text = nextText;

        while (alpha < 1.0f)
        {
            alpha += Time.deltaTime / (fadeTime * 1.25f);
            F_next.color = new Color(F_next.color.r, F_next.color.g, F_next.color.b, alpha);
            yield return null;
        }
    }

    public static IEnumerator FadeOutBackground(float currentAlpha) // decreases the opacity of the canvas background image by 10%
    {
        float targetAlpha = 1f - ((counter - 7f)*0.05f);
        if (counter >= 17)
        {
            targetAlpha = 0f;
        }
        float newAlpha = currentAlpha;
        float fadeTime = 20; // duration of the fade effecct in seconds

        while (background.color.a > targetAlpha)
        {
            newAlpha -= Time.deltaTime / (fadeTime);
            background.color = new Color(background.color.r, background.color.g, background.color.b, newAlpha);
            yield return null;
        }
    }

    #endregion

    #region OSC methods
    /// <summary>
    /// Initializes the OSC server settings.
    /// Sets up the main and band transmitters with the specified IP address and ports.
    /// </summary>
    void OSC_ServerInit()
    {
        if (mainTransmitter == null)
        {
            mainTransmitter = gameObject.AddComponent<OSCTransmitter>();

            if (bandTransmitter == null)
            {
                bandTransmitter = gameObject.AddComponent<OSCTransmitter>();
            }
        }

        mainTransmitter.RemoteHost = ipAddress;
        mainTransmitter.RemotePort = 5005;

        bandTransmitter.RemoteHost = ipAddress;
        bandTransmitter.RemotePort = 5006;
    }

    #endregion
}