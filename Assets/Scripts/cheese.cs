using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class cheese : MonoBehaviour
{
    float time;
    TMP_Text tmp_text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tmp_text = gameObject.GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        time += 1f;
        if (time > 3f)
        {
            tmp_text.text = "the cheesiest bread";
            time = 4f;
        }
        Debug.Log(time);
    }
}
