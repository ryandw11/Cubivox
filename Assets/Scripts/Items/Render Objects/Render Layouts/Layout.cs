using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cubivox.Renderobjects
{
    public interface Layout
    {
        Vertex GetVertex(Vector3 pos);
        Texture GetTextureCords();
        Normal GetNormal();
        Indices GetIndices();
    }
}