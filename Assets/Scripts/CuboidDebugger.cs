using System;
using UnityEngine;
using CubivoxCore;
using CubivoxCore.Worlds;
using System.Collections.Generic;

/// <summary>
/// Debug Cuboid regions by showing the Cuboid outline and the chunks that make up the Cuboid.
/// </summary>
public class CuboidDebugger : MonoBehaviour
{

    public Vector3 min; // The minimum corner of the cube
    public Vector3 max; // The maximum corner of the cube

    private Cuboid currentChunkCuboid;
    private List<ChunkLocation> chunks = new List<ChunkLocation>();

    private void Start()
    {
        currentChunkCuboid = new Cuboid(
            new Location(min.x, min.y, min.z),
            new Location(max.x, max.y, max.z)
        );

        ObtainChunkBulkEdits(currentChunkCuboid);
    }

    void OnDrawGizmos()
    {
        if( currentChunkCuboid == null )
        {
            return;
        }
        if( ToVec3(currentChunkCuboid.MinCorner) != min || ToVec3(currentChunkCuboid.MaxCorner) != max)
        {
            currentChunkCuboid = new Cuboid(
                new Location(min.x, min.y, min.z),
                new Location(max.x, max.y, max.z)
            );
            ObtainChunkBulkEdits(currentChunkCuboid);
        }
        DrawCube(min, max, UnityEngine.Color.green);
        foreach (var chunk in chunks)
        {
            var cuboid = chunk.AsCuboid();
            DrawCube(ToVec3(cuboid.MinCorner), ToVec3(cuboid.MaxCorner), UnityEngine.Color.red);
        }
    }

    private Vector3 ToVec3(Location loc)
    {
        return new Vector3((float) loc.x, (float) loc.y, (float) loc.z);
    }

    void DrawCube(Vector3 min, Vector3 max, UnityEngine.Color color)
    {
        max += new Vector3(0.999f, 0.999f, 0.999f);

        // Calculate the 8 corners of the cube
        Vector3[] corners = new Vector3[8];
        corners[0] = new Vector3(min.x, min.y, min.z); // bottom-front-left
        corners[1] = new Vector3(max.x, min.y, min.z); // bottom-front-right
        corners[2] = new Vector3(min.x, min.y, max.z); // bottom-back-left
        corners[3] = new Vector3(max.x, min.y, max.z); // bottom-back-right
        corners[4] = new Vector3(min.x, max.y, min.z); // top-front-left
        corners[5] = new Vector3(max.x, max.y, min.z); // top-front-right
        corners[6] = new Vector3(min.x, max.y, max.z); // top-back-left
        corners[7] = new Vector3(max.x, max.y, max.z); // top-back-right

        // Draw lines to connect the corners and form the edges of the cube
        Debug.DrawLine(corners[0], corners[1], color);
        Debug.DrawLine(corners[1], corners[3], color);
        Debug.DrawLine(corners[3], corners[2], color);
        Debug.DrawLine(corners[2], corners[0], color);

        Debug.DrawLine(corners[4], corners[5], color);
        Debug.DrawLine(corners[5], corners[7], color);
        Debug.DrawLine(corners[7], corners[6], color);
        Debug.DrawLine(corners[6], corners[4], color);

        Debug.DrawLine(corners[0], corners[4], color);
        Debug.DrawLine(corners[1], corners[5], color);
        Debug.DrawLine(corners[2], corners[6], color);
        Debug.DrawLine(corners[3], corners[7], color);
    }

    private void ObtainChunkBulkEdits(Cuboid cuboid)
    {
        chunks.Clear();
        // Convert Min and Max corners of the Cuboid to ChunkLocations
        ChunkLocation chunkMin = cuboid.MinCorner.ToChunkLocation();
        ChunkLocation chunkMax = cuboid.MaxCorner.ToChunkLocation();

        // Loop through the ChunkLocation space
        for (int x = chunkMin.X; x <= chunkMax.X; x++)
        {
            for (int y = chunkMin.Y; y <= chunkMax.Y; y++)
            {
                for (int z = chunkMin.Z; z <= chunkMax.Z; z++)
                {
                    var chunkLocation = new ChunkLocation(null, x, y, z);

                    chunks.Add(chunkLocation);
                }
            }
        }
    }
}
