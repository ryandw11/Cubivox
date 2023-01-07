using System.Collections;
using System.Linq;
using UnityEngine;
using CubivoxCore.BaseGame;
using CubivoxCore;
using CubivoxCore.Worlds;
using CubivoxRender;

using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace CubivoxClient.BaseGame
{
    public class ClientChunk : MonoBehaviour, Chunk
    {
        public static readonly int CHUNK_SIZE = 16;

        private ClientVoxel[,,] voxels = new ClientVoxel[CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE];
        private JobHandle jobHandle;
        RenderChunkJob.MeshOutput meshOutput;
        NativeArray<RenderVoxel> voxs;
        NativeList<int3>  vertices;
        NativeList<float2> textures;
        NativeList<int> indicies;
        bool hasJob = false;

        public Location GetLocation()
        {
            return LocationUtils.ChunkTransformVectorToChunkLocation(transform.position);
        }

        public Voxel GetVoxel(int x, int y, int z)
        {
            return voxels[Mathf.FloorToInt(x - transform.position.x), Mathf.FloorToInt(y - transform.position.y), Mathf.FloorToInt(z - transform.position.z)];
        }

        public World GetWorld()
        {
            return WorldManager.GetInstance().GetCurrentWorld();
        }

        public bool IsLoaded()
        {
            // Chunks are always loaded on the client.
            return !hasJob;
        }

        public bool Load()
        {
            // The client does not have the ability to load chunks.
            return true;
        }

        public void SetVoxel(int x, int y, int z, VoxelDef voxelDef)
        {
            voxels[Mathf.FloorToInt(x - transform.position.x), Mathf.FloorToInt(y - transform.position.y), Mathf.FloorToInt(z - transform.position.z)].SetVoxelDef(voxelDef);
        }

        // Use this for initialization
        void Start()
        {
            if(Mathf.FloorToInt(transform.position.y / CHUNK_SIZE) != 2)
            {
                if((int)(transform.position.y / CHUNK_SIZE) < 2)
                {
                    voxels = WorldManager.defaultVoxels;
                } else
                {
                    voxels = WorldManager.defaultVoxelsAir;
                }
            } else
            {
                for (int x = 0; x < CHUNK_SIZE; x++)
                {
                    for (int z = 0; z < CHUNK_SIZE; z++)
                    {
                        double height = Mathf.PerlinNoise((transform.position.x + x) / 100, (transform.position.z + z) / 100) * 16;
                        for (int y = 0; y < CHUNK_SIZE; y++)
                        {
                            if (y < height)
                            {
                                voxels[x, y, z] = new ClientVoxel(new Location(x, y, z), (VoxelDef)Cubivox.GetItemRegistry().GetItem(new ControllerKey(Cubivox.GetInstance(), "TESTBLOCK")));
                            }
                            else
                            {
                                voxels[x, y, z] = new ClientVoxel(new Location(x, y, z), (VoxelDef)Cubivox.GetItemRegistry().GetItem(new ControllerKey(Cubivox.GetInstance(), "AIR")));
                            }
                        }
                    }
                }
                    
            }

            WorldManager.GetInstance().GetCurrentWorld().AddLoadedChunk(this);

            UpdateChunk();
        }

        // Update is called once per frame
        void Update()
        {
            if(hasJob)
            {
                if(jobHandle.IsCompleted)
                {
                    jobHandle.Complete();
                    var mesh = new Mesh
                    {
                        vertices = meshOutput.vertices.ToArray().Select(vertex => new Vector3(vertex.x, vertex.y, vertex.z)).ToArray(),
                        triangles = meshOutput.indicies.ToArray(),
                        uv = meshOutput.textures.ToArray().Select(vertex => new Vector2(vertex.x, vertex.y)).ToArray()
                    };
                    vertices.Dispose(jobHandle);
                    indicies.Dispose(jobHandle);
                    textures.Dispose(jobHandle);
                    voxs.Dispose(jobHandle);

                    mesh.RecalculateNormals();
                    mesh.RecalculateBounds();
                    mesh.RecalculateTangents();
                    GetComponent<MeshFilter>().mesh = mesh;
                    GetComponent<MeshCollider>().sharedMesh = mesh;

                    hasJob = false;
                }
            }
        }

        public void UpdateChunk()
        {
            if (hasJob) return;
            voxs = new NativeArray<RenderVoxel>(CHUNK_SIZE * CHUNK_SIZE * CHUNK_SIZE, Allocator.TempJob);
            for (int x = 0; x < CHUNK_SIZE; x++)
            {
                for (int y = 0; y < CHUNK_SIZE; y++)
                {
                    for (int z = 0; z < CHUNK_SIZE; z++)
                    {
                        voxs[XYZToI(x, y, z)] = voxels[x, y, z].GetRenderVoxel();
                    }
                }
            }

            vertices = new NativeList<int3>(Allocator.TempJob);
            textures = new NativeList<float2>(Allocator.TempJob);
            indicies = new NativeList<int>(Allocator.TempJob);

            meshOutput = new RenderChunkJob.MeshOutput
            {
                vertices = vertices,
                textures = textures,
                indicies = indicies
            };

            jobHandle = new RenderChunkJob
            {
                meshOutput = meshOutput,
                voxelData = voxs,
                voxelVerticies = new RenderVoxel.VoxelVerticies
                {
                    front = Verticies.FRONT,
                    back = Verticies.BACK,
                    top = Verticies.TOP,
                    bottom = Verticies.BOTTOM,
                    left = Verticies.LEFT,
                    right = Verticies.RIGHT
                },
                voxelTextures = new RenderVoxel.VoxelTextures
                {
                    front = Textures.FRONT,
                    back = Textures.BACK,
                    top = Textures.TOP,
                    bottom = Textures.BOTTOM,
                    left = Textures.LEFT,
                    right = Textures.RIGHT
                },
                voxelIndicies = new RenderVoxel.VoxelIndicies
                {
                    front = Indices.FRONT,
                    back = Indices.BACK,
                    top = Indices.TOP,
                    bottom = Indices.BOTTOM,
                    left = Indices.LEFT,
                    right = Indices.RIGHT
                }
            }.Schedule();

            hasJob = true;  
        }

        private int XYZToI(int x, int y, int z)
        {
            return x + (z * CHUNK_SIZE) + (y * CHUNK_SIZE * CHUNK_SIZE);
        }

    }
}