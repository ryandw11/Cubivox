using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sandbox.Renderobjects
{
    /**
     * <summary>
     * This represents a chunk in the game.
     * 
     * <para>A chunk is a 16 x 16 x 16 area that can be full of blocks. This class is not put on the actual game objects of the chunk. This
     * is just a representation of the chunk, thus its name: RenderChunk. Instead <see cref="Chunk"/> is placed on the actual chunk.</para>
     * <para> Please note that the Chunk class has little functionality and this is the main class you should use.</para>
     * 
     * <para>Chunks are managed via <see cref="WorldManager"/>. Load a chunk via <see cref="WorldManager.LoadChunk(Vector3)"/>.</para>
     * </summary>
     */
    public class RenderChunk
    {
        private GameObject obj;
        private RenderBlock[,,] octChunk;
        private int blockCount = 0;

        public readonly static int CHUNK_SIZE = 12;

        private Vector3 position;

        /**
         * <summary>Create a new RenderChunk.</summary>
         * <remarks>This is only needed when adding chunks manually. Use <see cref="WorldManager"/> instead to get chunks.</remarks>
         * <param name="blocks">The list of render blocks that make up the chunk.</param>
         * <param name="atlas">The texture atlas to use.</param>
         */
        public RenderChunk(List<RenderBlock> blocks, TextureAtlas atlas)
        {
            this.position = new Vector3(0, 0, 0);
            this.octChunk = new RenderBlock[CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE];
            foreach(RenderBlock blck in blocks){
                blck.SetParentChunk(this);
                octChunk[(int)blck.GetPosition().x, (int)blck.GetPosition().y, (int)blck.GetPosition().z] = blck;
            }
            blockCount = blocks.Count;
        }

        /**
         * <summary>Add a block to the chunk.</summary>
         * <param name="block">The RenderBlock to add.</param>
         */
        public void AddBlock(RenderBlock block)
        {
            if (block.GetParentChunk() != null)
                throw new System.Exception("Error: This block already has a parent!");
            block.SetParentChunk(this);
            if (octChunk[(int)block.GetPosition().x, (int)block.GetPosition().y, (int)block.GetPosition().z] == null)
                blockCount++;

            octChunk[(int)block.GetPosition().x, (int)block.GetPosition().y, (int)block.GetPosition().z] = block;
        }

        /**
         * <summary>Remove a block from the chunk.</summary>
         * <param name="block">The block to remove.</param>
         */
        public void RemoveBlock(RenderBlock block)
        {
            octChunk[(int)block.GetPosition().x, (int)block.GetPosition().y, (int)block.GetPosition().z] = null;
            block.SetParentChunk(null);
            blockCount--;
        }

        /**
         * <summary>Get the OctChunk (3D array) of the chunk.</summary>
         * <returns>The 3D array of the chunk.</returns>
         */
        public RenderBlock[,,] GetOctChunk()
        {
            return octChunk;
        }

        /**
         * <summary>Get the actual Game Object for this chunk.</summary>
         * <remarks>This value can be null if the chunk is being unloaded, regenerated, or loaded in.</remarks>
         * <returns>The game object for this chunk.</returns>
         */
        public GameObject GetGameObject()
        {
            return obj;
        }

        /**
         * <summary>Destroy the game object for this chunk.</summary>
         * <remarks>Internal use only.</remarks>
         */
        public void DestroyGameObject()
        {
            GameObject temp = obj;
            obj = null;
            UnityEngine.Object.DestroyImmediate(temp.gameObject);
        }

        /**
         * <summary>Get the position for the chunk.
         * <para>This is in chunk coords, not world coords. Multiply by 16 to get world coords.</para>
         * </summary>
         * <returns>The position of the chunk.</returns>
         */
        public Vector3 GetPosition()
        {
            return position;
        }

        /**
         * <summary>Set the position of the chunk.</summary>
         * <param name="x">The x value.</param>
         * <param name="y">The y value.</param>
         * <param name="z">The z value.</param>
         */
        public void SetPosition(int x, int y, int z)
        {
            this.position.x = x;
            this.position.y = y;
            this.position.z = z;
        }

        /**
         * <summary>Get the chunk class for this chunk.</summary>
         * <remarks>This can return null for the same conditions of <see cref="GetGameObject"/></remarks>
         * <returns>The chunk class.</returns>
         */
        public Chunk GetChunk()
        {
            if (obj == null)
                return null;
            return obj.GetComponent<Chunk>();
        }

        /**
         * <summary>Calculate the visible blocks in the chunk to optimize mesh creation.</summary>
         * <returns>The list of visible blocks.</returns>
         */
        public List<RenderBlock> CalculateVisibleBlocks()
        {
            List<RenderBlock> output = new List<RenderBlock>();
            for(int x = 0; x < CHUNK_SIZE; x++)
            {
                for(int y = 0; y < CHUNK_SIZE; y++)
                {
                    for(int z = 0; z < CHUNK_SIZE; z++)
                    {
                        RenderBlock block = octChunk[x, y, z];
                        if (block == null) continue;

                        block.ClearFaces();

                        bool found = false;

                        if (z - 1 < 0 || octChunk[x,y,z - 1] == null)
                        {
                            block.AddFace(Face.FRONT);
                            output.Add(block);
                            found = true;
                        }
                        if (z + 1 > CHUNK_SIZE-1 || octChunk[x,y,z + 1] == null)
                        {
                            block.AddFace(Face.BACK);
                            if (!found)
                                output.Add(block);
                            found = true;
                        }
                        if (y + 1 > CHUNK_SIZE-1 || octChunk[x,y + 1,z] == null)
                        {
                            block.AddFace(Face.TOP);
                            if (!found)
                                output.Add(block);
                            found = true;
                        }
                        if (y - 1 < 0 || octChunk[x,y - 1,z] == null)
                        {
                            block.AddFace(Face.BOTTOM);
                            if (!found)
                                output.Add(block);
                            found = true;
                        }
                        if (x + 1 > CHUNK_SIZE-1 || octChunk[x + 1,y,z] == null)
                        {
                            block.AddFace(Face.RIGHT);
                            if (!found)
                                output.Add(block);
                            found = true;
                        }
                        if (x - 1 < 0 || octChunk[x - 1,y,z] == null)
                        {
                            block.AddFace(Face.LEFT);
                            if (!found)
                                output.Add(block);
                        }
                    }
                }
            }
            return output;
        }

        /**
         * <summary>
         * Regenerate the mesh (GameObject) for this chunk.
         * <para>This must be done after an edit to the chunk. viz. <see cref="AddBlock(RenderBlock)"/> and <see cref="RemoveBlock(RenderBlock)"/></para>
         * <para>This is resource intensive and should be done on a seperate thread. See <see cref="ThreadManager"/>.</para>
         * </summary>
         * <param name="atlas">The texture atlas to use.</param>
         */
        public void RegenerateChunkObject(TextureAtlas atlas)
        {
            List<RenderBlock> blocks = CalculateVisibleBlocks();
            List<Vector3> positions = new List<Vector3>();
            List<Vector2> textures = new List<Vector2>();
            List<Vector3> normals = new List<Vector3>();
            List<int> indicies = new List<int>();
            List<Color> colors = new List<Color>();
            int count = 0;
            foreach (RenderBlock rb in blocks)
            {
                int prePos = positions.Count;
                rb.GetVertexFromFaces(positions);
                rb.GetTextureFromFaces(textures, atlas);
                rb.GetNormalsFromFaces(normals);
                rb.GetIndicesFromFaces(indicies, count);
                for (int i = 0; i < (positions.Count - prePos); i++)
                    colors.Add(rb.GetColor());
                count += rb.GetVisibleFaces().Count * 4;
            }
            SandboxManager.GetInstance().AddAction(() =>
            {
                GameObject gameObject = new GameObject("Chunk{" + position.x + ", " + position.y + ", " + position.z + "}");
                MeshFilter filter = gameObject.AddComponent<MeshFilter>();
                MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
                Chunk chunk = gameObject.AddComponent<Chunk>();
                chunk.renderChunk = this;

                meshRenderer.sharedMaterial = WorldManager.GetInstance().mainMaterial;

                Mesh mesh = new Mesh();

                mesh.vertices = positions.ToArray();
                mesh.triangles = indicies.ToArray();
                mesh.normals = normals.ToArray();
                mesh.uv = textures.ToArray();
                mesh.colors = colors.ToArray();

                filter.mesh = mesh;

                if (obj != null)
                    UnityEngine.Object.Destroy(obj);
                obj = gameObject;
                obj.transform.position = position * CHUNK_SIZE;
            });
        }
    }
}
