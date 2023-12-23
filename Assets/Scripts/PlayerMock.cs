using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Cubivox.Renderobjects;

/// <summary>
/// This class is a Mock of the Player class and is temporary for testing systems.
/// </summary>
public class PlayerMock : MonoBehaviour
{
    // Start is called before the first frame update
    /*void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyUp(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float step = 0.3f;
            while(step < 20)
            {
                Vector3 position = ray.origin + (ray.direction * step);
                position.z -= 1;
                position.x -= position.x < 0 ? 1 : 0;
                if (WorldManager.GetInstance().LoadedBlockExists((int)position.x, (int)position.y, (int)position.z))
                {
                    Vector3 finalPos = new Vector3(position.x, position.y, position.z);
                    WorldManager.GetInstance().RemoveLoadedBlock((int)finalPos.x, (int)finalPos.y, (int)finalPos.z);
                    break;
                }

                step += 0.3f;
            }
            
            
            
            /*Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.SphereCast(ray, 0.05f, out hit, 20))
            {
                int dx = Mathf.FloorToInt(hit.point.x / RenderChunk.CHUNK_SIZE);
                int dy = Mathf.FloorToInt((hit.point.y % 12 == 0 ? 11 : hit.point.y) / RenderChunk.CHUNK_SIZE);
                int dz = Mathf.FloorToInt(hit.point.z / RenderChunk.CHUNK_SIZE);
                RenderChunk renderChunk = WorldManager.GetInstance().GetLoadedChunk(new Vector3(dx, dy, dz));
                int posX = Fix(hit.point.x);
                int posY = Fix(hit.point.y - 0.2f);
                int posZ = Fix(hit.point.z);
                Debug.Log(hit.point);
                Debug.Log(posX + ";" + posY + ";" + posZ);
                RenderBlock block = renderChunk.GetOctChunk()[posX, posY, posZ];
                renderChunk.RemoveBlock(block);
                renderChunk.RegenerateChunkObject(WorldManager.GetInstance().GetTextureAtlas());
            }
    // * /
        }

        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float step = 0.3f;
            while (step < 20)
            {
                Vector3 position = ray.origin + (ray.direction * step);
                position.z -= 1;
                position.x -= position.x < 0 ? 1 : 0;
                if (WorldManager.GetInstance().LoadedBlockExists((int)position.x, (int)position.y, (int)position.z))
                {
                    Vector3 finalPos = new Vector3(position.x, position.y, position.z);
                    WorldManager.GetInstance().RemoveLoadedBlock((int)finalPos.x, (int)finalPos.y, (int)finalPos.z);
                    break;
                }

                step += 0.3f;
            }
        }
    }*/
}
