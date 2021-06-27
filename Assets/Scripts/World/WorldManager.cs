using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Cubivox.Renderobjects;
using Cubivox.Items;

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
            for(int cy = 0; cy < 5; cy++)
            {
                for (int cx = -10; cx < 10; cx++)
                {
                    for (int cz = -10; cz < 10; cz++)
                   {
                        LoadChunk(new Vector3(cx, cy, cz));
                    }
                }
            }
        });

        // Setup the main thread for loading and unloading chunks.

        handleChunkLoading = new Thread(() => {
            while (true)
            {
                foreach (RenderChunk chunk in chunks.Values)
                {
                    if (!chunk.IsLoaded()) continue;

                    if (Vector3.Distance(mainCameraPos / RenderChunk.CHUNK_SIZE, chunk.GetPosition()) > RenderDistance / 2 + 1)
                    {
                        bool res = chunks.TryRemove(chunk.GetPosition(), out _);
                        CubivoxManager.GetInstance().AddAction(() => chunk.DestroyGameObject());
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

                // Prevent heavy use of the CPU.
                Thread.Sleep(50);
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
        RenderChunk chunk = new RenderChunk(new List<RenderBlock>(), textureAtlas);
        lock (chunks)
        {
            chunks.TryAdd(position, chunk);
        }
        ThreadManager.GetInstance().AddAction(() =>
        {
            if(position.y < 2)
            {
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
            }

            chunk.SetPosition((int)position.x, (int)position.y, (int)position.z);
            chunk.RegenerateChunkObject(textureAtlas);
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

    /// <summary>
    /// Get a loaded chunk.
    /// </summary>
    /// <param name="position">The position of the chunk to get.</param>
    /// <returns>The loaded render chunk.</returns>
    public RenderChunk GetLoadedChunk(Vector3 position)
    {
        RenderChunk output;
        bool result = chunks.TryGetValue(position, out output);

        if (!result) return null;

        return output;
    }

    /// <summary>
    /// Get the number of loaded chunks.
    /// </summary>
    /// <returns>The number of chunks that are loaded.</returns>
    public int NumberOfLoadedChunks()
    {
        return chunks.Count;
    }

    /**
     * <summary>Get the current render distance.</summary>
     * <returns>The current render distance.</returns>
     */
    public int GetRenderDistance()
    {
        return RenderDistance;
    }

    /// <summary>
    /// Check to make sure a block exists within the <b>LOADED</b> world.
    /// </summary>
    /// <param name="x">The global x position.</param>
    /// <param name="y">The global y position.</param>
    /// <param name="z">The global z position.</param>
    /// <returns>If the loaded block exists.</returns>
    public bool LoadedBlockExists(int x, int y, int z)
    {
        int chunkX = Mathf.FloorToInt((float) x / RenderChunk.CHUNK_SIZE);
        int chunkY = Mathf.FloorToInt((float) y / RenderChunk.CHUNK_SIZE);
        int chunkZ = Mathf.FloorToInt((float) z / RenderChunk.CHUNK_SIZE);
        RenderChunk chunk = GetLoadedChunk(new Vector3(chunkX, chunkY, chunkZ));
        if (chunk == null) return false;

        return chunk.HasBlock(x - (int) chunk.GetGlobalPosition().x, y - (int) chunk.GetGlobalPosition().y, z - (int) chunk.GetGlobalPosition().z);
    }

    /// <summary>
    /// Get a <b>LOADED</b> RenderBlock in the world.
    /// </summary>
    /// <param name="x">The global x position.</param>
    /// <param name="y">The global y position.</param>
    /// <param name="z">The global z position.</param>
    /// <returns>The RenderBlock. (Null if not found)</returns>
    public RenderBlock GetLoadedBlock(int x, int y, int z)
    {
        int chunkX = Mathf.FloorToInt((float)x / RenderChunk.CHUNK_SIZE);
        int chunkY = Mathf.FloorToInt((float)y / RenderChunk.CHUNK_SIZE);
        int chunkZ = Mathf.FloorToInt((float)z / RenderChunk.CHUNK_SIZE);
        RenderChunk chunk = GetLoadedChunk(new Vector3(chunkX, chunkY, chunkZ));
        if (chunk == null) return null;

        return chunk.GetOctChunk()[x - (int)chunk.GetGlobalPosition().x, y - (int)chunk.GetGlobalPosition().y, z - (int)chunk.GetGlobalPosition().z];
    }

    /// <summary>
    /// Remove a <b>LOADED</b> block if it exists.
    /// </summary>
    /// <param name="x">The global x position.</param>
    /// <param name="y">The global y position.</param>
    /// <param name="z">The global z position.</param>
    public void RemoveLoadedBlock(int x, int y, int z)
    {
        int chunkX = Mathf.FloorToInt((float)x / RenderChunk.CHUNK_SIZE);
        int chunkY = Mathf.FloorToInt((float)y / RenderChunk.CHUNK_SIZE);
        int chunkZ = Mathf.FloorToInt((float)z / RenderChunk.CHUNK_SIZE);
        RenderChunk chunk = GetLoadedChunk(new Vector3(chunkX, chunkY, chunkZ));
        if (chunk == null) return;

        chunk.RemoveBlock(x - (int)chunk.GetGlobalPosition().x, y - (int)chunk.GetGlobalPosition().y, z - (int)chunk.GetGlobalPosition().z);
        chunk.RegenerateChunkObject(textureAtlas);
    }
}
