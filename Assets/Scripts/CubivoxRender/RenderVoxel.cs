using Unity;
using Unity.Collections;
using UnityEngine;

using Unity.Mathematics;

namespace CubivoxRender
{
    public struct RenderVoxel
    {
        [ReadOnly]
        public float xOffset;
        [ReadOnly]
        public float yOffset;
        [ReadOnly]
        public int rows;
        [ReadOnly]
        public bool transparent;
        [ReadOnly]
        public bool empty;

        public struct VoxelVerticies
        {
            [ReadOnly]
            public NativeArray<int3> front;
            [ReadOnly]
            public NativeArray<int3> back;
            [ReadOnly]
            public NativeArray<int3> top;
            [ReadOnly]
            public NativeArray<int3> bottom;
            [ReadOnly]
            public NativeArray<int3> left;
            [ReadOnly]
            public NativeArray<int3> right;
        }

        //[ReadOnly]
        //public VoxelVerticies voxelVerticies;

        public struct VoxelTextures
        {
            [ReadOnly]
            public NativeArray<float2> front;
            [ReadOnly]
            public NativeArray<float2> back;
            [ReadOnly]
            public NativeArray<float2> top;
            [ReadOnly]
            public NativeArray<float2> bottom;
            [ReadOnly]
            public NativeArray<float2> left;
            [ReadOnly]
            public NativeArray<float2> right;
        }

        //[ReadOnly]
        //public VoxelTextures voxelTextures;

        public struct VoxelIndicies
        {
            [ReadOnly]
            public NativeArray<int> front;
            [ReadOnly]
            public NativeArray<int> back;
            [ReadOnly]
            public NativeArray<int> top;
            [ReadOnly]
            public NativeArray<int> bottom;
            [ReadOnly]
            public NativeArray<int> left;
            [ReadOnly]
            public NativeArray<int> right;
        }

        //[ReadOnly]
        //public VoxelIndicies voxelIndicies;
    }
}
