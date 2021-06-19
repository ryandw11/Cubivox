using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cubivox.Renderobjects;

/**
 * <summary>
 * The name of this class is misleading. This is just the mesh GameObject reprsentation of a chunk.
 * <para>See <see cref="RenderChunk"/> instead for the real chunk data.</para>
 * </summary>
 */
public class Chunk : MonoBehaviour
{
    private WorldManager worldManager;
    public RenderChunk renderChunk;

    // Start is called before the first frame update
    void Awake()
    {
        this.worldManager = WorldManager.GetInstance();
    }

    // Update is called once per frame
    void Update()
    {
        // If this chunk was unloaded delete it.
        if (!worldManager.GetLoadedChunks().ContainsKey(renderChunk.GetPosition()))
        {
            Destroy(gameObject);
        }
    }
}
