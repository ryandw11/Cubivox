using CubivoxClient.BaseGame;
using CubivoxClient.Texturing;
using CubivoxClient.Worlds;
using CubivoxCore;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Net.Sockets;

using UnityEngine;
using CubivoxCore.Worlds;
using CubivoxCore.Scheduler;

namespace CubivoxClient.Protocol.ClientBound
{
    public class CBLoadChunkPacket : ClientBoundPacket
    {
        public bool ProcessPacket(ClientCubivox clientCubivox, NetworkStream stream)
        {
            byte[] locBuffer = new byte[4];
            NetworkingUtils.FillBufferFromNetwork(locBuffer, stream);
            int x = BitConverter.ToInt32(locBuffer);
            NetworkingUtils.FillBufferFromNetwork(locBuffer, stream);
            int y = BitConverter.ToInt32(locBuffer);
            NetworkingUtils.FillBufferFromNetwork(locBuffer, stream);
            int z = BitConverter.ToInt32(locBuffer);

            byte[] voxelMapBuffer = new byte[2];
            NetworkingUtils.FillBufferFromNetwork(voxelMapBuffer, stream);

            Dictionary<byte, short> voxelMap = new Dictionary<byte, short>();
            short voxelMapSize = BitConverter.ToInt16(voxelMapBuffer);
            for(int i = 0; i < voxelMapSize; i++)
            {
                NetworkingUtils.FillBufferFromNetwork(voxelMapBuffer, stream);
                voxelMap.Add((byte) i, BitConverter.ToInt16(voxelMapBuffer));
            }

            NetworkingUtils.FillBufferFromNetwork(locBuffer, stream);
            int voxelBufferSize = BitConverter.ToInt32(locBuffer);

            byte[] voxelBuffer = new byte[voxelBufferSize];
            NetworkingUtils.FillBufferFromNetwork(voxelBuffer, stream);
            voxelBuffer = Decompress(voxelBuffer);
            byte[,,] voxels = MemoryUtils.OneDArrayTo3DArray(ref voxelBuffer, ClientChunk.CHUNK_SIZE);

            CubivoxScheduler.RunOnMainThread(() => {
                ClientWorld world = WorldManager.GetInstance().GetCurrentWorld();

                // Check if the chunk already exists, if so replace it.
                Chunk existingChunk = world.GetChunk(x, y, z);
                if (existingChunk != null)
                {
                    Cubivox.GetInstance().GetLogger().Debug($"Recieved regenerate request for chunk at {x}, {y}, {z}");
                    ClientChunk clientChunk = (ClientChunk) existingChunk;
                    clientChunk.PopulateChunk(voxels, voxelMap, (byte)voxelMapSize);
                    clientChunk.UpdateChunk();
                }
                else
                {
                    GameObject gameObject = new GameObject($"Chunk{{{x}, {y}, {z}}}");
                    gameObject.tag = "Ground";
                    gameObject.transform.position = new Vector3(x * ClientChunk.CHUNK_SIZE, y * ClientChunk.CHUNK_SIZE, z * ClientChunk.CHUNK_SIZE);
                    MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
                    MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
                    MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
                    meshRenderer.sharedMaterial = ((ClientTextureAtlas)Cubivox.GetTextureAtlas()).GetMaterial();
                    ClientChunk clientChunk = gameObject.AddComponent<ClientChunk>();

                    clientChunk.PopulateChunk(voxels, voxelMap, (byte)voxelMapSize);
                    world.AddLoadedChunk(clientChunk);
                    clientChunk.UpdateChunk();
                }
            });

            return true;
        }

        private byte[] Decompress(byte[] data)
        {
            MemoryStream input = new MemoryStream(data);
            MemoryStream output = new MemoryStream();
            using (DeflateStream dstream = new DeflateStream(input, CompressionMode.Decompress))
            {
                dstream.CopyTo(output);
            }
            return output.ToArray();
        }

        byte Packet.GetType()
        {
            return 0x08;
        }

        bool AssertProblem()
        {
            Debug.Assert(false);
            return false;
        }
    }
}
