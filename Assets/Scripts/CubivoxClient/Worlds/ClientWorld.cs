﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using CubivoxCore;
using CubivoxCore.BaseGame;
using CubivoxCore.Exceptions;
using CubivoxCore.Worlds;
using CubivoxClient.BaseGame;
using CubivoxClient;
using UnityEngine;
using System.Runtime.CompilerServices;
using CubivoxClient.Texturing;

namespace CubivoxClient.Worlds
{
    public class ClientWorld : World
    {
        private Location spawnLocation;
        private ConcurrentDictionary<Location, ClientChunk> loadedChunks;
        private int renderDistance = 16;

        public ClientWorld()
        {
            this.spawnLocation = new Location(0, 0, 0);
            loadedChunks = new ConcurrentDictionary<Location, ClientChunk>();
        }

        public void AddLoadedChunk(ClientChunk chunk)
        {
            lock (loadedChunks)
            {
                loadedChunks.TryAdd(chunk.GetLocation(), chunk);
            }
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
            int chunkX = Mathf.FloorToInt(x / ClientChunk.CHUNK_SIZE);
            int chunkY = Mathf.FloorToInt(y / ClientChunk.CHUNK_SIZE);
            int chunkZ = Mathf.FloorToInt(z / ClientChunk.CHUNK_SIZE);

            Chunk chunk = GetChunk(chunkX, chunkY, chunkZ);

            if (chunk == null) return null;

            return chunk.GetVoxel(x, y, z);
        }

        public bool IsChunkLoaded(Location location)
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
            return IsChunkLoaded(new Location(x, y, z));
        }

        public bool IsChunkLoaded(Chunk chunk)
        {
            return IsChunkLoaded(chunk.GetLocation());
        }

        public void LoadChunk(Location location)
        {
            GameObject gameObject = new GameObject($"Chunk{{{location.x}, {location.y}, {location.z}}}");
            gameObject.transform.position = new Vector3((float)location.x * 16, (float)location.y * 16, (float)location.z * 16);
            MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = ((ClientTextureAtlas)Cubivox.GetTextureAtlas()).GetMaterial();
            ClientChunk clientChunk = gameObject.AddComponent<ClientChunk>();
        }

        public void LoadChunk(int x, int y, int z)
        {
            throw new InvalidEnvironmentException("The client cannot load chunks!");
        }

        public void LoadChunk(Chunk chunk)
        {
            throw new InvalidEnvironmentException("The client cannot load chunks!");
        }

        public Chunk GetChunk(Location location)
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
            return GetChunk(new Location(x, y, z));
        }

        public void Save()
        {
            throw new InvalidEnvironmentException("The client cannot initiate a save of the world!");
        }

        public void SetVoxel(int x, int y, int z, VoxelDef voxel)
        {
            GetVoxel(x, y, z).SetVoxelDef(voxel);
        }

        public void UnloadChunk(int x, int y, int z)
        {
            throw new InvalidEnvironmentException("The client cannot unload chunks!");
        }

        public void UnloadChunk(Chunk chunk)
        {
            throw new InvalidEnvironmentException("The client cannot unload chunks!");
        }

        public ConcurrentDictionary<Location, ClientChunk> GetLoadedChunks()
        {
            return loadedChunks;
        }
    }
}