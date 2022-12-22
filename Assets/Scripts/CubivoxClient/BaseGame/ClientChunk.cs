using System.Collections;
using UnityEngine;
using CubivoxCore.BaseGame;
using CubivoxCore;
using CubivoxCore.Worlds;

namespace CubivoxClient.BaseGame
{
    public class ClientChunk : MonoBehaviour, Chunk
    {
        public static readonly int CHUNK_SIZE = 16;

        private ClientVoxel[,,] voxels = new ClientVoxel[CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE];

        public Location GetLocation()
        {
            return LocationUtils.VectorToLocation(transform.position);
        }

        public Voxel GetVoxel(int x, int y, int z)
        {
            return voxels[Mathf.FloorToInt(x - transform.position.x), Mathf.FloorToInt(y - transform.position.y), Mathf.FloorToInt(z - transform.position.z)];
        }

        public World GetWorld()
        {
            // TODO :: Return the current world that the client is in.
            throw new System.NotImplementedException();
        }

        public bool IsLoaded()
        {
            // Chunks are always loaded on the client.
            return true;
        }

        public bool Load()
        {
            // The client does not have the ability to load chunks.
            return true;
        }

        // Use this for initialization
        void Start()
        {
            if(transform.position.y < 2 * CHUNK_SIZE)
            {
                for (int x = 0; x < CHUNK_SIZE; x++)
                {
                    for (int y = 0; y < CHUNK_SIZE; y++)
                    {
                        for (int z = 0; z < CHUNK_SIZE; z++)
                        {
                            voxels[x, y, z] = new ClientVoxel(new Location(x, y, z), (VoxelDef) Cubivox.GetItemRegistry().GetItem(new ControllerKey(Cubivox.GetInstance(), "TESTBLOCK")));
                        }
                    }
                }
            }
            else
            {
                for (int x = 0; x < CHUNK_SIZE; x++)
                {
                    for (int y = 0; y < CHUNK_SIZE; y++)
                    {
                        for (int z = 0; z < CHUNK_SIZE; z++)
                        {
                            voxels[x, y, z] = new ClientVoxel(new Location(x, y, z), (VoxelDef) Cubivox.GetItemRegistry().GetItem(new ControllerKey(Cubivox.GetInstance(), "AIR")));
                        }
                    }
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}