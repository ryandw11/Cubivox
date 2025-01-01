using CubivoxCore;
using System.Collections;
using UnityEngine;

using Unity.Mathematics;
using CubivoxCore.Worlds;

namespace CubivoxClient
{
    public class LocationUtils
    {
        public static Vector3 LocationToVector(Location location)
        {
            return new Vector3((float)location.x, (float)location.y, (float)location.z);
        }

        public static int3 LocationToInt3(Location location)
        {
            return new int3((int)location.x, (int)location.y, (int)location.z);
        }

        public static Location VectorToLocation(Vector3 vector)
        {
            return new Location(vector.x, vector.y, vector.z);
        }

        public static ChunkLocation ChunkTransformVectorToChunkLocation(World world, Vector3 vector)
        {
            return new ChunkLocation(world, Mathf.FloorToInt(vector.x / 16), Mathf.FloorToInt(vector.y / 16), Mathf.FloorToInt(vector.z / 16));
        }
    }
}