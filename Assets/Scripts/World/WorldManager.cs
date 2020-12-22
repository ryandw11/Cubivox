using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Sandbox.Renderobjects;
using Sandbox.Items;

/**
 * <summary>
 *  This handels the functionality of the world. eg, chunks.
 *  <para>
 *      This class holds: The list of loaded chunks, the texture atlas, the render distance value, and the thread for handling chunk loading.
 *  </para>
 *  <para>
 *      Use <see cref="GetInstance"/> to get the instance of this class.
 *  </para>
 * </summary>
 */
public class WorldManager : MonoBehaviour
{

    private static WorldManager instance;

    private TextureAtlas textureAtlas;
    private List<AtlasTexture> atlasTextures;

    private ConcurrentDictionary<Vector3, RenderChunk> chunks = new ConcurrentDictionary<Vector3, RenderChunk>();

    private int RenderDistance = 16;

    private Vector3 mainCameraPos;

    private Thread handleChunkLoading;

    private bool loaded = false;

    public Material mainMaterial;

    // Start is called before the first frame update

    private void Start()
    {
        WorldManager.instance = this;

        List<AtlasTexture> atlasTextures = new List<AtlasTexture>();
        foreach(Block obj in ItemManager.GetInstance().GetBlocks())
        {
            AtlasTexture text = new AtlasTexture("Textures/" + obj.GetTexture());
            obj.SetAtlasTexture(text);
            atlasTextures.Add(text);
            Debug.Log("test");
        }
        this.atlasTextures = atlasTextures;

        TextureAtlas atlas = new TextureAtlas(atlasTextures, Application.dataPath + "/test_atlas.png");

        mainMaterial = new Material(Shader.Find("Standard"));
        mainMaterial.mainTexture = atlas.GetTexture();

        this.textureAtlas = atlas;
        LoadChunk(new Vector3(0, 0, 0));

        ThreadManager.GetInstance().AddAction(() =>
        {
            for (int cx = -10; cx < 10; cx++)
            {
                for (int cz = -10; cz < 10; cz++)
                {
                    LoadChunk(new Vector3(cx, 0, cz));
                }
            }
            loaded = true;
        });

        // Setup the main thread for loading and unloading chunks.

        handleChunkLoading = new Thread(() => {
            while (true)
            {
                if (!loaded) continue;
                foreach (RenderChunk chunk in chunks.Values)
                {


                    if (Vector3.Distance(mainCameraPos / RenderChunk.CHUNK_SIZE, chunk.GetPosition()) > RenderDistance / 2 + 1)
                    {
                        bool res = chunks.TryRemove(chunk.GetPosition(), out _);
                    }

                    //if (Vector3.Distance(mainCameraPos / 16, chunk.GetPosition()) < 4 || Vector3.Distance(mainCameraPos / 16, chunk.GetPosition()) > 7) continue;

                    Vector3 pos = chunk.GetPosition();
                    if (Vector3.Distance(new Vector3(pos.x + 1, pos.y, pos.z), mainCameraPos / RenderChunk.CHUNK_SIZE) < RenderDistance / 2)
                    {
                        LoadChunk(new Vector3(pos.x + 1, pos.y, pos.z));
                    }
                    if (Vector3.Distance(new Vector3(pos.x - 1, pos.y, pos.z), mainCameraPos / RenderChunk.CHUNK_SIZE) < RenderDistance / 2)
                    {
                        LoadChunk(new Vector3(pos.x - 1, pos.y, pos.z));
                    }
                    if (Vector3.Distance(new Vector3(pos.x, pos.y, pos.z + 1), mainCameraPos / RenderChunk.CHUNK_SIZE) < RenderDistance / 2)
                    {
                        LoadChunk(new Vector3(pos.x, pos.y, pos.z + 1));
                    }
                    if (Vector3.Distance(new Vector3(pos.x, pos.y, pos.z - 1), mainCameraPos / RenderChunk.CHUNK_SIZE) < RenderDistance / 2)
                    {
                        LoadChunk(new Vector3(pos.x, pos.y, pos.z - 1));
                    }


                }
            }
        });

        handleChunkLoading.Name = "Sandbox Chunk Updates";
        handleChunkLoading.Start();
    }

    private void OnDestroy()
    {
        // Stop the thread when this object is destroyed.
        handleChunkLoading.Abort();
    }

    void Update()
    {
        // Update the main camera position which is used in the loading thread.
        mainCameraPos = Camera.main.transform.position;

        foreach(RenderChunk chunk in chunks.Values)
        {
            Graphics.DrawMesh(chunk.GetMesh(), chunk.GetPosition() * RenderChunk.CHUNK_SIZE, Quaternion.identity, mainMaterial, 0);
        }
    }

    /**
     * <summary>Get the instance of the class.</summary>
     * <returns>The current instance of the WorldManager.</returns>
     */
    public static WorldManager GetInstance()
    {
        return instance;
    }

    /**
     * <summary>Get the current active texture atlas.</summary>
     * <returns>The current active texture atlas.</returns>
     */
    public TextureAtlas GetTextureAtlas()
    {
        return textureAtlas;
    }

    /**
     * <summary>Get the dictionary containing the loaded chunks.</summary>
     * <returns>The list of loaded chunks.</returns>
     */
    public ConcurrentDictionary<Vector3, RenderChunk> GetLoadedChunks()
    {
        return chunks;
    }

    /**
     * <summary>
     * Load a chunk via it's position. If the chunk is already loaded, then nothing happens.
     * <para>Note: This method is Async and runs on the <see cref="ThreadManager"/> to not hold up the main thread.</para>
     * </summary>
     * <param name="position">The position to load the chunk at.</param>
     */
    public void LoadChunk(Vector3 position)
    {
        if (chunks.ContainsKey(position)) return;
        Block bl = ItemManager.GetInstance().GetObjectByName<Block>("Block");

        ThreadManager.GetInstance().AddAction(() =>
        {
            RenderChunk chunk = new RenderChunk(new List<RenderBlock>(), textureAtlas);
            for (int x = 0; x < RenderChunk.CHUNK_SIZE; x++)
            {
                for (int y = 0; y < RenderChunk.CHUNK_SIZE; y++)
                {
                    for (int z = 0; z < RenderChunk.CHUNK_SIZE; z++)
                    {
                        RenderBlock rb = new RenderBlock(bl, new Vector3(x, y, z));
                        rb.SetColor(new Color(255, 0, 0));
                        chunk.AddBlock(rb);
                    }
                }
            }
            chunk.SetPosition((int)position.x, (int)position.y, (int)position.z);
            chunk.RegenerateChunkObject(textureAtlas);
            lock (chunks)
            {
                chunks.TryAdd(position, chunk);
            }
        });
    }

    /**
     * <summary>Check to see if a chunk is loaded.</summary>
     * <param name="position">The position to check.</param>
     * <returns>If a chunk is currently loaded.</returns>
     */
    public bool IsChunkLoaded(Vector3 position)
    {
        return chunks.ContainsKey(position);
    }

    /**
     * <summary>Get the current render distance.</summary>
     * <returns>The current render distance.</returns>
     */
    public int GetRenderDistance()
    {
        return RenderDistance;
    }
}
