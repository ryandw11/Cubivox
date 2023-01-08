using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CubivoxClient;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public TMPro.TMP_InputField usernameText;
    public TMPro.TMP_InputField ipText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnConnectClick()
    {
        Debug.Log("Swapping Scenes...");
        ClientCubivox.GetClientInstance().ConnectToServer(ipText.text, 5555, usernameText.text);
        SceneManager.LoadScene("MainScene");
    }
}
