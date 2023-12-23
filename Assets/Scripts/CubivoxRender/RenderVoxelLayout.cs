using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;

namespace CubivoxRender
{
    public struct RenderVoxelLayout
    {
        // TODO, do this in the future.
    }

    public struct Verticies
    {
        [ReadOnly]
        public static readonly NativeArray<int3> FRONT = new NativeArray<int3>(4, Allocator.Persistent)
        {
            [0] = new int3(0, 1, 0),
            [1] = new int3(0, 0, 0),
            [2] = new int3(1, 0, 0),
            [3] = new int3(1, 1, 0)
        };

        [ReadOnly]
        public static readonly NativeArray<int3> BOTTOM = new NativeArray<int3>(4, Allocator.Persistent)
        {
            [0] = new int3(0, 0, 0),
            [1] = new int3(0, 0, 1),
            [2] = new int3(1, 0, 1),
            [3] = new int3(1, 0, 0)
        };

        [ReadOnly]
        public static readonly NativeArray<int3> BACK = new NativeArray<int3>(4, Allocator.Persistent)
        {
            [0] = new int3(0, 1, 1),
            [1] = new int3(0, 0, 1),
            [2] = new int3(1, 0, 1),
            [3] = new int3(1, 1, 1)
        };

        [ReadOnly]
        public static readonly NativeArray<int3> LEFT = new NativeArray<int3>(4, Allocator.Persistent)
        {
            [0] = new int3(0, 1, 0),
            [1] = new int3(0, 0, 0),
            [2] = new int3(0, 0, 1),
            [3] = new int3(0, 1, 1)
        };

        [ReadOnly]
        public static readonly NativeArray<int3> RIGHT = new NativeArray<int3>(4, Allocator.Persistent)
        {
            [0] = new int3(1, 1, 1),
            [1] = new int3(1, 0, 1),
            [2] = new int3(1, 0, 0),
            [3] = new int3(1, 1, 0)
        };

        [ReadOnly]
        public static readonly NativeArray<int3> TOP = new NativeArray<int3>(4, Allocator.Persistent)
        {
            [0] = new int3(0, 1, 0),
            [1] = new int3(0, 1, 1),
            [2] = new int3(1, 1, 1),
            [3] = new int3(1, 1, 0)
        };
    }

    public struct Textures
    {
        public const float ONE_THIRD = 1f / 3f;
        public const float TWO_THIRD = 2f / 3f;

        [ReadOnly]
        public static readonly NativeArray<float2> BACK = new NativeArray<float2>(4, Allocator.Persistent)
        {
            [0] = new float2(1, TWO_THIRD),
            [1] = new float2(1, ONE_THIRD),
            [2] = new float2(0.75f, ONE_THIRD),
            [3] = new float2(0.75f, TWO_THIRD)
        };

        [ReadOnly]
        public static readonly NativeArray<float2> TOP = new NativeArray<float2>(4, Allocator.Persistent)
        {
            [0] = new float2(0.25f, TWO_THIRD),
            [1] = new float2(0.25f, 1f),
            [2] = new float2(0.5f, 1f),
            [3] = new float2(0.5f, TWO_THIRD)
        };

        [ReadOnly]
        public static readonly NativeArray<float2> FRONT = new NativeArray<float2>(4, Allocator.Persistent)
        {
            [0] = new float2(0.25f, TWO_THIRD),
            [1] = new float2(0.25f, ONE_THIRD),
            [2] = new float2(0.5f, ONE_THIRD),
            [3] = new float2(0.5f, TWO_THIRD)
        };

        [ReadOnly]
        public static readonly NativeArray<float2> LEFT = new NativeArray<float2>(4, Allocator.Persistent)
        {
            [0] = new float2(0.25f, TWO_THIRD),
            [1] = new float2(0.25f, ONE_THIRD),
            [2] = new float2(0, ONE_THIRD),
            [3] = new float2(0, TWO_THIRD)
        };

        [ReadOnly]
        public static readonly NativeArray<float2> RIGHT = new NativeArray<float2>(4, Allocator.Persistent)
        {
            [0] = new float2(0.75f, TWO_THIRD),
            [1] = new float2(0.75f, ONE_THIRD),
            [2] = new float2(0.5f, ONE_THIRD),
            [3] = new float2(0.5f, TWO_THIRD)
        };

        [ReadOnly]
        public static readonly NativeArray<float2> BOTTOM = new NativeArray<float2>(4, Allocator.Persistent)
        {
            [0] = new float2(0.25f, ONE_THIRD),
            [1] = new float2(0.25f, 0),
            [2] = new float2(0.5f, 0),
            [3] = new float2(0.5f, ONE_THIRD)
        };
    }

    public struct Indices
    {
        [ReadOnly]
        public static readonly NativeArray<int> FRONT = new NativeArray<int>(6, Allocator.Persistent)
        {
            [0] = 0,
            [1] = 3,
            [2] = 2,
            [3] = 2,
            [4] = 1,
            [5] = 0
        };

        [ReadOnly]
        public static readonly NativeArray<int> BOTTOM = new NativeArray<int>(6, Allocator.Persistent)
        {
            [0] = 0,
            [1] = 3,
            [2] = 2,
            [3] = 2,
            [4] = 1,
            [5] = 0
        };

        [ReadOnly]
        public static readonly NativeArray<int> BACK = new NativeArray<int>(6, Allocator.Persistent)
        {
            [0] = 0,
            [1] = 1,
            [2] = 2,
            [3] = 2,
            [4] = 3,
            [5] = 0
        };

        [ReadOnly]
        public static readonly NativeArray<int> LEFT = new NativeArray<int>(6, Allocator.Persistent)
        {
            [0] = 0,
            [1] = 1,
            [2] = 2,
            [3] = 2,
            [4] = 3,
            [5] = 0
        };

        [ReadOnly]
        public static readonly NativeArray<int> RIGHT = new NativeArray<int>(6, Allocator.Persistent)
        {
            [0] = 0,
            [1] = 1,
            [2] = 2,
            [3] = 2,
            [4] = 3,
            [5] = 0
        };

        [ReadOnly]
        public static readonly NativeArray<int> TOP = new NativeArray<int>(6, Allocator.Persistent)
        {
            [0] = 0,
            [1] = 1,
            [2] = 2,
            [3] = 2,
            [4] = 3,
            [5] = 0
        };
    }
}
