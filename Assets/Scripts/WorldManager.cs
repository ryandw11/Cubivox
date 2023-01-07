using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

using CubivoxCore;
using CubivoxCore.BaseGame;
using CubivoxCore.Worlds;
using CubivoxClient.Worlds;
using CubivoxClient.BaseGame;
using CubivoxClient.Texturing;

using Unity.Jobs;
using CubivoxClient;

/// <summary>
/// This keeps track of the <see cref="World"/> that the client is currently in.
/// <br/>
/// Obtain the instance of this class through <see cref="GetInstance"/>.
/// </summary>
public class WorldManager : MonoBehaviour
{
    private ClientWorld currentWorld;
    private static WorldManager instance;

    public static ClientVoxel[,,] defaultVoxels = new ClientVoxel[ClientChunk.CHUNK_SIZE, ClientChunk.CHUNK_SIZE, ClientChunk.CHUNK_SIZE];
    public static ClientVoxel[,,] defaultVoxelsAir = new ClientVoxel[ClientChunk.CHUNK_SIZE, ClientChunk.CHUNK_SIZE, ClientChunk.CHUNK_SIZE];

    private Thread handleChunkLoading;
    private Queue<Action> mainThreadQueue = new Queue<Action>();

    private Vector3 mainCameraPos;
    int RenderDistance = 16;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        currentWorld = new ClientWorld();

        for (int x = 0; x < ClientChunk.CHUNK_SIZE; x++)
        {
            for (int y = 0; y < ClientChunk.CHUNK_SIZE; y++)
            {
                for (int z = 0; z < ClientChunk.CHUNK_SIZE; z++)
                {
                    defaultVoxels[x, y, z] = new ClientVoxel(new Location(x, y, z), (VoxelDef)Cubivox.GetItemRegistry().GetItem(new ControllerKey(Cubivox.GetInstance(), "TESTBLOCK")));
                }
            }
        }
        for (int x = 0; x < ClientChunk.CHUNK_SIZE; x++)
        {
            for (int y = 0; y < ClientChunk.CHUNK_SIZE; y++)
            {
                for (int z = 0; z < ClientChunk.CHUNK_SIZE; z++)
                {
                    defaultVoxelsAir[x, y, z] = new ClientVoxel(new Location(x, y, z), (VoxelDef)Cubivox.GetItemRegistry().GetItem(new ControllerKey(Cubivox.GetInstance(), "AIR")));
                }
            }
        }

        handleChunkLoading = new Thread(() => {
            while (true)
            {
                foreach (KeyValuePair<Location, ClientChunk> chunk in currentWorld.GetLoadedChunks())
                {
                    if (!chunk.Value.IsLoaded()) continue;

                    /*if (Vector3.Distance(mainCameraPos / ClientChunk.CHUNK_SIZE, LocationUtils.LocationToVector(chunk.GetLocation())) > RenderDistance / 2 + 1)
                    {
                        bool res = currentWorld.GetLoadedChunks().TryRemove(chunk.GetLocation(), out _);
                        CubivoxManager.GetInstance().AddAction(() => chunk.DestroyGameObject());
                    }*/

                    //if (Vector3.Distance(mainCameraPos / 16, chunk.GetPosition()) < 4 || Vector3.Distance(mainCameraPos / 16, chunk.GetPosition()) > 7) continue;

                    Vector3 pos = LocationUtils.LocationToVector(chunk.Key);
                    if (Vector3.Distance(new Vector3(pos.x + 1, pos.y, pos.z), mainCameraPos / ClientChunk.CHUNK_SIZE) < RenderDistance / 2)
                    {
                        LoadChunk(new Location(pos.x + 1, pos.y, pos.z));
                    }
                    if (Vector3.Distance(new Vector3(pos.x - 1, pos.y, pos.z), mainCameraPos / ClientChunk.CHUNK_SIZE) < RenderDistance / 2)
                    {
                        LoadChunk(new Location(pos.x - 1, pos.y, pos.z));
                    }
                    if (Vector3.Distance(new Vector3(pos.x, pos.y, pos.z + 1), mainCameraPos / ClientChunk.CHUNK_SIZE) < RenderDistance / 2)
                    {
                        LoadChunk(new Location(pos.x, pos.y, pos.z + 1));
                    }
                    if (Vector3.Distance(new Vector3(pos.x, pos.y, pos.z - 1), mainCameraPos / ClientChunk.CHUNK_SIZE) < RenderDistance / 2)
                    {
                        LoadChunk(new Location(pos.x, pos.y, pos.z - 1));
                    }


                }

                // Prevent heavy use of the CPU.
                Thread.Sleep(50);
            }
        });

        // TODO :: Don't hard code this in the future.

        StartCoroutine(Generate());
    }

    IEnumerator Generate()
    {
        for (int x = -10; x < 10; x++)
        {
            for (int y = 0; y < 16; y++)
            {
                for (int z = -10; z < 10; z++)
                {
                    GameObject gameObject = new GameObject($"Chunk{{{x}, {y}, {z}}}");
                    gameObject.transform.position = new Vector3(x * 16, y * 16, z * 16);
                    MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
                    MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
                    MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
                    meshRenderer.sharedMaterial = ((ClientTextureAtlas)Cubivox.GetTextureAtlas()).GetMaterial();
                    ClientChunk clientChunk = gameObject.AddComponent<ClientChunk>();
                }
                yield return null;
            }
        }

        //handleChunkLoading.Start();
    }

    void LoadChunk(Location position)
    {
        if (currentWorld.GetLoadedChunks().ContainsKey(position)) return;

        AddAction(() =>
        {
            GameObject gameObject = new GameObject($"Chunk{{{position.x}, {position.y}, {position.z}}}");
            gameObject.transform.position = new Vector3((float)position.x * 16, (float)position.y * 16, (float)position.z * 16);
            MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = ((ClientTextureAtlas)Cubivox.GetTextureAtlas()).GetMaterial();
            ClientChunk clientChunk = gameObject.AddComponent<ClientChunk>();
        });
    }

    public void AddAction(Action action)
    {
        lock (mainThreadQueue)
        {
            mainThreadQueue.Enqueue(action);
        }
    }

    // Update is called once per frame
    void Update()
    {
        mainCameraPos = Camera.main.transform.position;

        lock(mainThreadQueue)
        {
            if (mainThreadQueue.Count > 0)
            {
                mainThreadQueue.Dequeue().Invoke();
            }
            if (mainThreadQueue.Count > 0)
            {
                mainThreadQueue.Dequeue().Invoke();
            }
            if (mainThreadQueue.Count > 0)
            {
                mainThreadQueue.Dequeue().Invoke();
            }
        }
    }

    public ClientWorld GetCurrentWorld()
    {
        return currentWorld;
    }

    public static WorldManager GetInstance()
    {
        return instance;
    }
}
