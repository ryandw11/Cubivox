using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cubvox.Renderobjects
{
    public interface Normal
    {
        List<Vector3> GetFront();
        List<Vector3> GetBack();
        List<Vector3> GetTop();
        List<Vector3> GetBottom();
        List<Vector3> GetRight();
        List<Vector3> GetLeft();
    }
}