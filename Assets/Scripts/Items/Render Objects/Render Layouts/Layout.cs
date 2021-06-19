using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cubvox.Renderobjects
{
    public interface Layout
    {
        Vertex GetVertex(Vector3 pos);
        Texture GetTextureCords();
        Normal GetNormal();
        Indices GetIndices();
    }
}