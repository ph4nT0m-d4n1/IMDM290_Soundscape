using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the functionality of the <c>Next</c> button in the Soundscape Therapy application.
/// Allows navigation through initial prompts and specific screens.
/// </summary>
public class NextButton : MonoBehaviour
{
    Button next; //button component attached to the gameObject

    void Start()
    {
        next = gameObject.GetComponent<Button>(); // get the button component from the current gameobject
        next.onClick.AddListener(OnClickNext); //add a click event listener to the button
    }

    /// <summary>
    /// Handles the button click event for navigating through prompts.
    /// Advances to the next prompt for specific screens in the application.
    /// </summary>
    public void OnClickNext()
    {
        // only allow advancing during initial instructions, final screens, and specific intermediate screens
        // conditions match those in the Prompt script's NextPrompt method
       
        if (Prompt.counter < 6 || Prompt.counter == 14 || Prompt.counter == 7)
        {
            Prompt.counter += 1;
            Prompt.UpdatePromptText();
        }
    }
}