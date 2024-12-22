using CubivoxCore;
using CubivoxCore.Events.Local;
using CubivoxCore.Events.Global;
using CubivoxCore.Voxels;

using CubivoxClient;
using CubivoxClient.Protocol.ServerBound;
using CubivoxClient.Utils;
using CubivoxClient.Worlds;

using UnityEngine;
using CubivoxCore.BaseGame;

/// <summary>
/// Responsible for handling the Player's voxel place and breaking controls.
/// 
/// Voxels are not directly modified on the Client, instead the respective packet is sent
/// to the server. The server will send a Place or Break packet back, which is when the voxels
/// are modified.
/// 
/// The Place and Break events are triggered BEFORE the packets are sent to the server.
/// </summary>
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
        if ( clientCubivox.CurrentState != GameState.PLAYING )
        {
            return;
        }

        HandleBreakVoxelInput();
        HandlePlaceVoxelInput();
    }

    /// <summary>
    /// Responsible for checking a break voxel input and sending the packet to the server. 
    /// </summary>
    private void HandleBreakVoxelInput()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float step = 0f;
            while (step < 20)
            {
                Vector3 position = ray.origin + (ray.direction * step);

                ClientWorld currentWorld = WorldManager.GetInstance().GetCurrentWorld();
                VoxelDef air = (VoxelDef)Cubivox.GetItemRegistry().GetItem(Voxels.AIR);

                position.x = Mathf.FloorToInt(position.x);
                position.y = Mathf.FloorToInt(position.y);
                position.z = Mathf.FloorToInt(position.z);

                Voxel voxel = currentWorld.GetVoxel((int)position.x, (int)position.y, (int)position.z);

                if (voxel != null && voxel.GetVoxelDef() != air)
                {
                    // Send events and check for response.
                    // Priority: 1) VoxelDef Events, 2) General Events
                    VoxelDefBreakEvent voxelDefBreakEvent = new VoxelDefBreakEvent(clientCubivox.LocalPlayer, voxel.GetLocation());
                    Isolator.Isolate(() => voxel.GetVoxelDef()._BreakEvent?.Invoke(voxelDefBreakEvent));
                    if (voxelDefBreakEvent.IsCancelled)
                    {
                        return;
                    }

                    VoxelBreakEvent breakEvent = new VoxelBreakEvent(clientCubivox.LocalPlayer, voxel);
                    if (Cubivox.GetEventManager().TriggerEvent(breakEvent))
                    {
                        clientCubivox.SendPacketToServer(new BreakVoxelPacket(LocationUtils.VectorToLocation(position)));
                    }
                    return;
                }

                step += 0.1f;
            }
        }
    }

    /// <summary>
    /// Responsible for checking a place voxel input and sending a packet to the server.
    /// </summary>
    private void HandlePlaceVoxelInput()
    {
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
                VoxelDef air = (VoxelDef)Cubivox.GetItemRegistry().GetItem(Voxels.AIR);

                Voxel voxel = currentWorld.GetVoxel((int)position.x, (int)position.y, (int)position.z);

                if (voxel != null && voxel.GetVoxelDef() != air)
                {
                    Vector3 realVoxelPos = new Vector3((int)position.x, (int)position.y, (int)position.z);

                    // Get the current voxel that the user has selected.
                    VoxelDef placeVoxel = CurrentVoxel.GetInstance().GetCurrentVoxel();
                    Vector3 newPlacement = NewBlock(realVoxelPos, ray);

                    var newVoxelLocation = LocationUtils.VectorToLocation(newPlacement);

                    // Send events and check for response.
                    // Priority: 1) VoxelDef Events, 2) General Events

                    VoxelDefPlaceEvent voxelDefPlaceEvent = new VoxelDefPlaceEvent(clientCubivox.LocalPlayer, newVoxelLocation);
                    Isolator.Isolate(() => placeVoxel._PlaceEvent?.Invoke(voxelDefPlaceEvent));
                    if (voxelDefPlaceEvent.IsCancelled)
                    {
                        return;
                    }

                    VoxelPlaceEvent voxelPlaceEvent = new VoxelPlaceEvent(clientCubivox.LocalPlayer, new ClientVoxel(newVoxelLocation, placeVoxel));
                    if (Cubivox.GetEventManager().TriggerEvent(voxelPlaceEvent))
                    {
                        clientCubivox.SendPacketToServer(new PlaceVoxelPacket(placeVoxel, newVoxelLocation));
                    }

                    return;
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
