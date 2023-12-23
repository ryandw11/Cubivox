using CubivoxClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public GameObject background;
    public TMPro.TextMeshProUGUI stepText;
    public TMPro.TextMeshProUGUI progressText;

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

        if(clientCubivox.CurrentState == GameState.CONNECTING || clientCubivox.CurrentState == GameState.CONNECTED_LOADING)
        {
            background.SetActive(true);
            if(clientCubivox.CurrentState == GameState.CONNECTING)
            {
                stepText.text = "Establishing Connection...";
                progressText.gameObject.SetActive(false);
            }
            else
            {
                stepText.text = "Loading World...";
                progressText.gameObject.SetActive(true);
                double percent = Mathf.Round((WorldManager.GetInstance().GetCurrentWorld().GetLoadedChunks().Count / 6400f) * 100);
                progressText.text = $"{percent}%";
            }
        }
        else
        {
            background.SetActive(false);
        }
    }
}
