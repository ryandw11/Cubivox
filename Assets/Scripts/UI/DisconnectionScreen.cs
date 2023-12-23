using CubivoxClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisconnectionScreen : MonoBehaviour
{
    public GameObject background;
    public TMPro.TextMeshProUGUI reasonText;

    private ClientCubivox clientCubivox;

    // Start is called before the first frame update
    void Start()
    {
        background.SetActive(false);

        if(ClientCubivox.HasInstance())
        {
            clientCubivox = ClientCubivox.GetClientInstance();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(clientCubivox == null && ClientCubivox.HasInstance())
        {
            clientCubivox = ClientCubivox.GetClientInstance();
        }
        else if(clientCubivox == null)
        {
            return;
        }

        if(clientCubivox.CurrentState == GameState.DISCONNECTED || clientCubivox.CurrentState == GameState.TITLE_SCREEN)
        {
            background.SetActive(true);
            reasonText.text = clientCubivox.DisconnectionReason;
        }
        else
        {
            background.SetActive(false);
        }
    }

    public void HandleOkClick()
    {
        clientCubivox.CurrentState = GameState.TITLE_SCREEN;
        SceneManager.LoadScene("TitleScreen");
    }
}
