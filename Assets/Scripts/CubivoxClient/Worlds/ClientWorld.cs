using System.Collections.Concurrent;

using CubivoxCore;
using CubivoxCore.Exceptions;
using CubivoxCore.Voxels;
using CubivoxCore.Worlds;
using CubivoxCore.Worlds.Generation;

using UnityEngine;

namespace CubivoxClient.Worlds
{
    public class ClientWorld : World
    {
        private Location spawnLocation;
        private ConcurrentDictionary<ChunkLocation, ClientChunk> loadedChunks;
        private int renderDistance = 16;

        public ClientWorld()
        {
            this.spawnLocation = new Location(0, 0, 0);
            loadedChunks = new ConcurrentDictionary<ChunkLocation, ClientChunk>();
        }

        public void AddLoadedChunk(ClientChunk chunk)
        {
            loadedChunks.TryAdd(chunk.GetLocation(), chunk);
        }

        public void RemoveLoadedChunk(ClientChunk chunk)
        {
            loadedChunks.TryRemove(chunk.GetLocation(), out _);
        }

        public Location GetSpawnLocation()
        {
            return spawnLocation;
        }

        public Voxel GetVoxel(int x, int y, int z)
        {
            int chunkX = Mathf.FloorToInt((float) x / ClientChunk.CHUNK_SIZE);
            int chunkY = Mathf.FloorToInt((float) y / ClientChunk.CHUNK_SIZE);
            int chunkZ = Mathf.FloorToInt((float) z / ClientChunk.CHUNK_SIZE);

            Chunk chunk = GetChunk(chunkX, chunkY, chunkZ);

            if (chunk == null) return null;

            return chunk.GetVoxel(x, y, z);
        }

        public bool IsChunkLoaded(ChunkLocation location)
        {
            ClientChunk output;
            if (loadedChunks.TryGetValue(location, out output))
            {
                return true;
            }

            return false;
        }

        public bool IsChunkLoaded(int x, int y, int z)
        {
            return IsChunkLoaded(new ChunkLocation(this, x, y, z));
        }

        public bool IsChunkLoaded(Chunk chunk)
        {
            return IsChunkLoaded(chunk.GetLocation());
        }

        public Chunk GetChunk(ChunkLocation location)
        {
            ClientChunk output;
            if (loadedChunks.TryGetValue(location, out output))
            {
                return output;
            }

            return null;
        }

        public Chunk GetChunk(int x, int y, int z)
        {
            return GetChunk(new ChunkLocation(this, x, y, z));
        }

        public void SetVoxel(int x, int y, int z, VoxelDef voxel)
        {
            int chunkX = Mathf.FloorToInt((float) x / ClientChunk.CHUNK_SIZE);
            int chunkY = Mathf.FloorToInt((float) y / ClientChunk.CHUNK_SIZE);
            int chunkZ = Mathf.FloorToInt((float) z / ClientChunk.CHUNK_SIZE);

            ClientChunk chunk = (ClientChunk) GetChunk(chunkX, chunkY, chunkZ);

            if (chunk == null)
            {
                ClientCubivox.GetClientInstance().GetLogger().Warn("Tried to set a Voxel when the chunk was not loaded!");
                return;
            }

            chunk.SetVoxel(x, y, z, voxel);
        }

        public ConcurrentDictionary<ChunkLocation, ClientChunk> GetLoadedChunks()
        {
            return loadedChunks;
        }

        public WorldGenerator GetGenerator()
        {
            throw new InvalidEnvironmentException("Generators only exist on the server.");
        }

        public WorldBulkEditor StartBulkEdit(Cuboid editCuboid)
        {
            throw new InvalidEnvironmentException("Bulk edits are only available on the server.");
        }
    }
}
