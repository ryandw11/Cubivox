using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatUI : MonoBehaviour
{
    public static ChatUI Instance;

    private TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        if (Instance != null)
        {
            throw new System.Exception("Error: ChatUI instance already exists!");
        }
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void SendChatMessage(string message)
    {
        text.text += "\n" + message;
    }
}
