using UnityEngine;
using UnityEngine.UI;

public class NextButton : MonoBehaviour
{
    Button next;
    void Start()
    {
        next = gameObject.GetComponent<Button>();
        next.onClick.AddListener(OnClick);
    }

    void Update()
    {
        if (Prompt.counter >= 6 && Prompt.counter != 7 && Prompt.counter != 14)
        {
            gameObject.SetActive(false);
        }
    } 

    public void OnClick()
    {
        if (Prompt.counter < 6 || Prompt.counter == 14 || Prompt.counter == 7)
        {
            Prompt.counter += 1;
            Prompt.prompt_text.text = Prompt.prompts[Prompt.counter];
            Debug.Log(Prompt.counter);
        }
    }
}
