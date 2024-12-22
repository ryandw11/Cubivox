using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public TMPro.TMP_InputField usernameText;
    public TMPro.TMP_InputField ipText;

    private CubivoxController controller;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("GameController");
        if (objs.Length == 0)
        {
            Debug.LogError("Controller not instantiated!");
            return;
        }
        controller = objs[0].GetComponent<CubivoxController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnConnectClick()
    {
        Debug.Log("Swapping Scenes...");
        // The actual connection to the server will be established when the MainScene loads.
        controller.PrepareConnectToServer(ipText.text, 5555, usernameText.text);
        SceneManager.LoadScene("MainScene");
    }
}
