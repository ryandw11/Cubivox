using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerScript : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnStartLocalPlayer()
    {
        gameObject.tag = "ClientPlayer";
    }

    [Command]
    public void CmdSpawnObj(string name)
    {
        GameObject pref =(GameObject) Instantiate(Resources.Load(name, typeof(GameObject)));
        NetworkServer.Spawn(pref, netIdentity.connectionToClient);
    }

}
