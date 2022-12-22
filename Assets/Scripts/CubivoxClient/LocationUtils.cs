using CubivoxCore;
using System.Collections;
using UnityEngine;

namespace CubivoxClient
{
    public class LocationUtils
    {
        public static Vector3 LocationToVector(Location location)
        {
            return new Vector3((float)location.x, (float)location.y, (float)location.z);
        }

        public static Location VectorToLocation(Vector3 vector)
        {
            return new Location(vector.x, vector.y, vector.z);
        }
    }
}