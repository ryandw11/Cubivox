using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

using CubivoxClient.BaseGame;

namespace CubivoxRender
{
    [BurstCompile(CompileSynchronously = true)]
    public struct RenderChunkJob : IJob
    {
        const int CHUNK_SIZE = 16;
        public struct MeshOutput
        {
            public NativeList<int3> vertices;
            public NativeList<float2> textures;
            public NativeList<int> indicies;
        }

        [WriteOnly]
        public MeshOutput meshOutput;

        [ReadOnly]
        public NativeArray<RenderVoxel> voxelData;

        [ReadOnly]
        public RenderVoxel.VoxelVerticies voxelVerticies;
        [ReadOnly]
        public RenderVoxel.VoxelTextures voxelTextures;
        [ReadOnly]
        public RenderVoxel.VoxelIndicies voxelIndicies;

        public void Execute()
        {
            int index = 0;
            for(int x = 0; x < CHUNK_SIZE; x++)
            {
                for(int y = 0; y < CHUNK_SIZE; y++)
                {
                    for(int z = 0; z < CHUNK_SIZE; z++)
                    {
                        var renderVoxel = voxelData[XYZToI(x, y, z)];
                        if (renderVoxel.empty) continue;
                        var positon = new int3(x, y, z);
                        if (ShouldAddFrontFace(x, y, z))
                        {
                            var faceVerticies = HandleFaceVerticies(voxelVerticies.front, positon);
                            var faceTextures = HandleFaceTextures(voxelTextures.front, renderVoxel);
                            var faceIndicies = HandleFaceIndicies(voxelIndicies.front, index);
                            meshOutput.vertices.AddRange(faceVerticies);
                            meshOutput.textures.AddRange(faceTextures);
                            meshOutput.indicies.AddRange(faceIndicies);

                            index += 4;

                            faceVerticies.Dispose();
                            faceTextures.Dispose();
                            faceIndicies.Dispose();
                        }
                        if (ShouldAddBackFace(x, y, z))
                        {
                            var faceVerticies = HandleFaceVerticies(voxelVerticies.back, positon);
                            var faceTextures = HandleFaceTextures(voxelTextures.back, renderVoxel);
                            var faceIndicies = HandleFaceIndicies(voxelIndicies.back, index);
                            meshOutput.vertices.AddRange(faceVerticies);
                            meshOutput.textures.AddRange(faceTextures);
                            meshOutput.indicies.AddRange(faceIndicies);

                            index += 4;

                            faceVerticies.Dispose();
                            faceTextures.Dispose();
                            faceIndicies.Dispose();
                        }
                        if (ShouldAddTopFace(x, y, z))
                        {
                            var faceVerticies = HandleFaceVerticies(voxelVerticies.top, positon);
                            var faceTextures = HandleFaceTextures(voxelTextures.top, renderVoxel);
                            var faceIndicies = HandleFaceIndicies(voxelIndicies.top, index);
                            meshOutput.vertices.AddRange(faceVerticies);
                            meshOutput.textures.AddRange(faceTextures);
                            meshOutput.indicies.AddRange(faceIndicies);

                            index += 4;

                            faceVerticies.Dispose();
                            faceTextures.Dispose();
                            faceIndicies.Dispose();
                        }
                        if (ShouldAddBottomFace(x, y, z))
                        {
                            var faceVerticies = HandleFaceVerticies(voxelVerticies.bottom, positon);
                            var faceTextures = HandleFaceTextures(voxelTextures.bottom, renderVoxel);
                            var faceIndicies = HandleFaceIndicies(voxelIndicies.bottom, index);
                            meshOutput.vertices.AddRange(faceVerticies);
                            meshOutput.textures.AddRange(faceTextures);
                            meshOutput.indicies.AddRange(faceIndicies);

                            index += 4;

                            faceVerticies.Dispose();
                            faceTextures.Dispose();
                            faceIndicies.Dispose();
                        }
                        if (ShouldAddLeftFace(x, y, z))
                        {
                            var faceVerticies = HandleFaceVerticies(voxelVerticies.left, positon);
                            var faceTextures = HandleFaceTextures(voxelTextures.left, renderVoxel);
                            var faceIndicies = HandleFaceIndicies(voxelIndicies.left, index);
                            meshOutput.vertices.AddRange(faceVerticies);
                            meshOutput.textures.AddRange(faceTextures);
                            meshOutput.indicies.AddRange(faceIndicies);

                            index += 4;

                            faceVerticies.Dispose();
                            faceTextures.Dispose();
                            faceIndicies.Dispose();
                        }
                        if (ShouldAddRightFace(x, y, z))
                        {
                            var faceVerticies = HandleFaceVerticies(voxelVerticies.right, positon);
                            var faceTextures = HandleFaceTextures(voxelTextures.right, renderVoxel);
                            var faceIndicies = HandleFaceIndicies(voxelIndicies.right, index);
                            meshOutput.vertices.AddRange(faceVerticies);
                            meshOutput.textures.AddRange(faceTextures);
                            meshOutput.indicies.AddRange(faceIndicies);

                            index += 4;

                            faceVerticies.Dispose();
                            faceTextures.Dispose();
                            faceIndicies.Dispose();
                        }
                    }
                }
            }
        }

        private bool ShouldAddFrontFace(int x, int y, int z)
        {
            if(z - 1 < 0 || voxelData[XYZToI(x, y, z - 1)].transparent)
            {
                return true;
            }
            return false;
        }

        private bool ShouldAddBackFace(int x, int y, int z)
        {
            if (z + 1 > CHUNK_SIZE - 1 || voxelData[XYZToI(x, y, z + 1)].transparent)
            {
                return true;
            }
            return false;
        }

        private bool ShouldAddTopFace(int x, int y, int z)
        {
            if (y + 1 > CHUNK_SIZE - 1 || voxelData[XYZToI(x, y + 1, z)].transparent)
            {
                return true;
            }
            return false;
        }

        private bool ShouldAddBottomFace(int x, int y, int z)
        {
            if (y - 1 < 0 || voxelData[XYZToI(x, y - 1, z)].transparent)
            {
                return true;
            }
            return false;
        }

        private bool ShouldAddLeftFace(int x, int y, int z)
        {
            if (x - 1 < 0 || voxelData[XYZToI(x - 1, y, z)].transparent)
            {
                return true;
            }
            return false;
        }

        private bool ShouldAddRightFace(int x, int y, int z)
        {
            if (x + 1 > CHUNK_SIZE - 1 || voxelData[XYZToI(x + 1, y, z)].transparent)
            {
                return true;
            }
            return false;
        }

        private NativeArray<int3> HandleFaceVerticies(NativeArray<int3> input, int3 position)
        {
            var verticies = new NativeArray<int3>(input.Length, Allocator.Temp);
            for (int i = 0; i < input.Length; i++)
            {
                verticies[i] = input[i] + position;
            }

            return verticies;
        }

        private NativeArray<float2> HandleFaceTextures(NativeArray<float2> input, RenderVoxel renderVoxel)
        {
            var textures = new NativeArray<float2>(input.Length, Allocator.Temp);
            for (int i = 0; i < input.Length; i++)
            {
                textures[i] = new float2(input[i].x / renderVoxel.rows + renderVoxel.xOffset, input[i].y / renderVoxel.rows + renderVoxel.yOffset);
            }

            return textures;
        }

        private NativeArray<int> HandleFaceIndicies(NativeArray<int> input, int index)
        {
            var indicies = new NativeArray<int>(input.Length, Allocator.Temp);
            for (int i = 0; i < input.Length; i++)
            {
                indicies[i] = input[i] + index;
            }

            return indicies;
        }

        private int XYZToI(int x, int y, int z)
        {
            return x + (z * CHUNK_SIZE) + (y * CHUNK_SIZE * CHUNK_SIZE);
        }
    }
}
