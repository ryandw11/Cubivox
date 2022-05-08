using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldLoadingScreen : MonoBehaviour
{
    public bool active = true;

    public GameObject panel;
    public Text loadingText;

    public GameObject playerObject;

    // Start is called before the first frame update
    void Start()
    {
        if(!active)
        {
            panel.SetActive(false);
            gameObject.SetActive(false);
        }
        else
        {
            panel.SetActive(true);
            playerObject.GetComponent<Rigidbody>().useGravity = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(WorldManager.GetInstance().NumberOfLoadedChunks() == 1226)
        {
            panel.SetActive(false);
            gameObject.SetActive(false);
            playerObject.GetComponent<Rigidbody>().useGravity = true;
            Debug.Log("Loading Complete");
        }
        loadingText.text = "Chunks Loaded: " + (2000 - WorldManager.GetInstance().NumberOfLoadedChunks()) + " / 1226";
    }
}
