using CubivoxCore;
using CubivoxCore.Voxels;

using CubivoxClient;
using CubivoxClient.Protocol.ServerBound;
using CubivoxClient.Worlds;

using UnityEngine;

public class PlayerVoxelControls : MonoBehaviour
{

    private ClientCubivox clientCubivox;

    // Start is called before the first frame update
    void Start()
    {
        clientCubivox = ClientCubivox.GetClientInstance();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float step = 0f;
            while (step < 20)
            {
                Vector3 position = ray.origin + (ray.direction * step);

                ClientWorld currentWorld = WorldManager.GetInstance().GetCurrentWorld();
                VoxelDef air = (VoxelDef)Cubivox.GetItemRegistry().GetItem(new ControllerKey(Cubivox.GetInstance(), "AIR"));

                position.x = Mathf.FloorToInt(position.x);
                position.y = Mathf.FloorToInt(position.y);
                position.z = Mathf.FloorToInt(position.z);

                Voxel voxel = currentWorld.GetVoxel((int)position.x, (int)position.y, (int)position.z);

                if (voxel != null && voxel.GetVoxelDef() != air)
                {
                    clientCubivox.SendPacketToServer(new BreakVoxelPacket(LocationUtils.VectorToLocation(position)));
                    break;
                }

                step += 0.1f;
            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float step = 0f;
            while (step < 20)
            {
                Vector3 position = ray.origin + (ray.direction * step);

                position.x = Mathf.FloorToInt(position.x);
                position.y = Mathf.FloorToInt(position.y);
                position.z = Mathf.FloorToInt(position.z);

                ClientWorld currentWorld = WorldManager.GetInstance().GetCurrentWorld();
                VoxelDef air = (VoxelDef)Cubivox.GetItemRegistry().GetItem(new ControllerKey(Cubivox.GetInstance(), "AIR"));

                Voxel voxel = currentWorld.GetVoxel((int)position.x, (int)position.y, (int)position.z);

                if (voxel != null && voxel.GetVoxelDef() != air)
                {
                    Vector3 realVoxelPos = new Vector3((int)position.x, (int)position.y, (int)position.z);                  

                    VoxelDef testBlock = (VoxelDef)Cubivox.GetItemRegistry().GetItem(new ControllerKey(Cubivox.GetInstance(), "TESTBLOCK"));
                    Vector3 newPlacement = NewBlock(realVoxelPos, ray);
                    clientCubivox.SendPacketToServer(new PlaceVoxelPacket(testBlock, LocationUtils.VectorToLocation(newPlacement)));
                    break;
                }

                step += 0.4f;
            }
        }
    }

    private Vector3 NewBlock(Vector3 realVoxelPos, Ray ray)
    {
        Vector3 invSlope = new Vector3(1 / ray.direction.x, 1 / ray.direction.y, 1 / ray.direction.z);
        Vector3 minVoxelPos = realVoxelPos;
        Vector3 maxVoxelPos = realVoxelPos + new Vector3(1, 1, 1);

        float tx1 = (minVoxelPos.x - ray.origin.x) * invSlope.x;
        float tx2 = (maxVoxelPos.x - ray.origin.x) * invSlope.x;
        float tmin = 20, tmax = 0;

        tmin = Mathf.Min(tx1, tx2);
        tmax = Mathf.Max(tx1, tx2);

        float ty1 = (minVoxelPos.y - ray.origin.y) * invSlope.y;
        float ty2 = (maxVoxelPos.y - ray.origin.y) * invSlope.y;

        tmin = Mathf.Max(tmin, Mathf.Min(ty1, ty2));
        tmax = Mathf.Min(tmax, Mathf.Max(ty1, ty2));


        float tz1 = (minVoxelPos.z - ray.origin.z) * invSlope.z;
        float tz2 = (maxVoxelPos.z - ray.origin.z) * invSlope.z;

        tmin = Mathf.Max(tmin, Mathf.Min(tz1, tz2));
        tmax = Mathf.Min(tmax, Mathf.Max(tz1, tz2));

        Vector3 pos = new Vector3(
            tmin * ray.direction.x + ray.origin.x,
            tmin * ray.direction.y + ray.origin.y,
            tmin * ray.direction.z + ray.origin.z);

        if (Mathf.Abs(pos.x - realVoxelPos.x) < 0.00001f) //left (-x) face
        {
            //return left face enum, or return the calcualtedposition
            //i myself return a modified block origin for a new block to be made
            realVoxelPos.x -= 1;
            return realVoxelPos;
        }

        if (Mathf.Abs(pos.x - realVoxelPos.x) > 0.99990) //right (+x) face
        {
            realVoxelPos.x += 1;
            return realVoxelPos;
        }

        if (Mathf.Abs(pos.y - realVoxelPos.y) < 0.00001f) // bot (-y) face
        {
            realVoxelPos.y -= 1;
            return realVoxelPos;
        }

        if (Mathf.Abs(pos.y - realVoxelPos.y) > 0.99990f) // top (+y) face
        {
            realVoxelPos.y += 1;
            return realVoxelPos;
        }

        if (Mathf.Abs(pos.z - realVoxelPos.z) < 0.00001f) // front (-z) face
        {
            realVoxelPos.z -= 1;
            return realVoxelPos;
        }

        if (Mathf.Abs(pos.z - realVoxelPos.z) > 0.99990f) // back (+z) face
        {
            realVoxelPos.z += 1;
            return realVoxelPos;
        }

        return realVoxelPos;
    }
}
