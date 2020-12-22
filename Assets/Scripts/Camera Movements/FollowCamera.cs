using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class FollowCamera : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            transform.position = Camera.main.transform.position;
        }
    }
}
