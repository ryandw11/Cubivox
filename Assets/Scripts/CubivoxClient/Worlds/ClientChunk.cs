using System.Collections;
using System.Collections.Generic;
using System.Linq;

using CubivoxCore;
using CubivoxCore.BaseGame.VoxelDefs;
using CubivoxCore.Exceptions;
using CubivoxCore.Scheduler;
using CubivoxCore.Utils;
using CubivoxCore.Voxels;
using CubivoxCore.Worlds;

using CubivoxClient.Voxels;

using CubivoxRender;

using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using CubivoxClient.Texturing;

namespace CubivoxClient.Worlds
{
    public class ClientChunk : MonoBehaviour, Chunk
    {
        public static readonly int CHUNK_SIZE = 16;

        private byte[,,] voxels = new byte[CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE];
        private Dictionary<byte, short> voxelMap = new Dictionary<byte, short>();
        private byte currentVoxelIndex = 0;

        private object mLock = new object();

        private JobHandle jobHandle;
        RenderChunkJob.MeshOutput meshOutput;
        NativeArray<RenderVoxel> voxs;
        NativeList<int3>  vertices;
        NativeList<float2> textures;
        NativeList<int> indicies;
        bool hasJob = false;

        private ClientTextureAtlas clientTextureAtlas;

        public Location GetLocation()
        {
            return LocationUtils.ChunkTransformVectorToChunkLocation(transform.position);
        }

        public Voxel GetVoxel(int x, int y, int z)
        {
            byte id = 0;

            lock (mLock)
            {
                id = voxels[CMath.mod(x, CHUNK_SIZE), CMath.mod(y, CHUNK_SIZE), CMath.mod(z, CHUNK_SIZE)];
            }
            
            return ByteToVoxel(id, new Location(GetWorld(), x, y, z));
        }

        public World GetWorld()
        {
            return WorldManager.GetInstance().GetCurrentWorld();
        }

        public void SetVoxel(int x, int y, int z, VoxelDef voxelDef)
        {
            SetLocalVoxel(CMath.mod(x, CHUNK_SIZE), CMath.mod(y, CHUNK_SIZE), CMath.mod(z, CHUNK_SIZE), voxelDef);
            UpdateChunk();
        }

        public void SetLocalVoxel(int x, int y, int z, VoxelDef voxelDef)
        {
            lock (mLock)
            {
                short voxelId = ClientCubivox.GetClientInstance().GetClientItemRegistry().GetVoxelDefId(voxelDef);
                if (voxelMap.ContainsValue(voxelId))
                {
                    byte key = voxelMap.First(pair => pair.Value == voxelId).Key;
                    voxels[x, y, z] = key;
                }
                else
                {
                    voxelMap[currentVoxelIndex] = voxelId;
                    currentVoxelIndex++;
                }
            }
        }

        public void Regenerate()
        {
            throw new InvalidEnvironmentException("Regenerating a Chunk can only be done on the server!");
        }

        public void PopulateChunk(byte[,,] voxels, Dictionary<byte, short> voxelMap, byte currentVoxelIndex)
        {
            lock( mLock )
            {
                this.voxels = voxels;
                this.voxelMap = voxelMap;
                this.currentVoxelIndex = currentVoxelIndex;
            }
        }

        // Use this for initialization
        void Start()
        {
            clientTextureAtlas = (ClientTextureAtlas) ClientCubivox.GetTextureAtlas();
            clientTextureAtlas.AtlasRecalculatedEvent += HandleAtlasUpdate;
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
                    vertices.Dispose();
                    indicies.Dispose();
                    textures.Dispose();
                    voxs.Dispose();

                    mesh.RecalculateNormals();
                    mesh.RecalculateBounds();
                    mesh.RecalculateTangents();
                    GetComponent<MeshFilter>().mesh = mesh;
                    GetComponent<MeshCollider>().sharedMesh = mesh;

                    hasJob = false;
                }
            }
        }

        private void HandleAtlasUpdate()
        {
            // Atlas coordinates may have changed, update the chunk.
            UpdateChunk();
        }

        public void UpdateChunk()
        {
            CubivoxScheduler.RunOnMainThreadSynchronized(() =>
            {
                lock (mLock)
                {
                    UpdateChunkLocked();
                }
            });
        }

        private void UpdateChunkLocked()
        {
            if (hasJob) return;
            voxs = new NativeArray<RenderVoxel>(CHUNK_SIZE * CHUNK_SIZE * CHUNK_SIZE, Allocator.TempJob);
            for (int x = 0; x < CHUNK_SIZE; x++)
            {
                for (int y = 0; y < CHUNK_SIZE; y++)
                {
                    for (int z = 0; z < CHUNK_SIZE; z++)
                    {
                        voxs[XYZToI(x, y, z)] = CreateRenderVoxel(GetVoxelDef(voxels[x, y, z]));
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

        private VoxelDef GetVoxelDef(byte b)
        {
            return ClientCubivox.GetClientInstance().GetClientItemRegistry().GetVoxelDef(voxelMap[b]);
        }

        private Voxel ByteToVoxel(byte b, Location location)
        {
            VoxelDef definition = ClientCubivox.GetClientInstance().GetClientItemRegistry().GetVoxelDef(voxelMap[b]);
            ClientVoxel clientVoxel = new ClientVoxel(location, definition);

            return clientVoxel;
        }

        // TODO :: REMOVE THIS
        private RenderVoxel CreateRenderVoxel(VoxelDef def)
        {
            RenderVoxel renderVoxel = new RenderVoxel
            {
                xOffset = def.GetAtlasTexture()?.XOffset ?? 0,
                yOffset = def.GetAtlasTexture()?.YOffset ?? 0,
                rows = Cubivox.GetTextureAtlas().GetNumberOfRows(),
                transparent = def.IsTransparent(),
                empty = def is AirVoxel
            };
            return renderVoxel;
        }

    }
}