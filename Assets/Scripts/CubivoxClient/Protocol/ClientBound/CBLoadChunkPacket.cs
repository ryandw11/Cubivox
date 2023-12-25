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

namespace CubivoxClient.Protocol.ClientBound
{
    public class CBLoadChunkPacket : ClientBoundPacket
    {
        public bool ProcessPacket(ClientCubivox clientCubivox, NetworkStream stream)
        {
            byte[] locBuffer = new byte[4];
            if (stream.Read(locBuffer, 0, 4) != 4)
                return false;
            int x = BitConverter.ToInt32(locBuffer);
            if (stream.Read(locBuffer, 0, 4) != 4)
                return false;
            int y = BitConverter.ToInt32(locBuffer);
            if (stream.Read(locBuffer, 0, 4) != 4)
                return false;
            int z = BitConverter.ToInt32(locBuffer);

            byte[] voxelMapBuffer = new byte[2];
            if (stream.Read(voxelMapBuffer, 0, 2) != 2)
                return false;

            Dictionary<byte, short> voxelMap = new Dictionary<byte, short>();
            short voxelMapSize = BitConverter.ToInt16(voxelMapBuffer);
            for(int i = 0; i < voxelMapSize; i++)
            {
                if (stream.Read(voxelMapBuffer, 0, 2) != 2)
                    return false;
                voxelMap.Add((byte) i, BitConverter.ToInt16(voxelMapBuffer));
            }

            if (stream.Read(locBuffer, 0, 4) != 4)
                return false;
            int voxelBufferSize = BitConverter.ToInt32(locBuffer);

            byte[] voxelBuffer = new byte[voxelBufferSize];
            if (stream.Read(voxelBuffer, 0, voxelBufferSize) != voxelBufferSize)
                return false;
            voxelBuffer = Decompress(voxelBuffer);
            byte[,,] voxels = MemoryUtils.OneDArrayTo3DArray(ref voxelBuffer, ClientChunk.CHUNK_SIZE);

            CubivoxController.RunOnMainThread(() => {
                ClientWorld world = WorldManager.GetInstance().GetCurrentWorld();

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
    }
}
