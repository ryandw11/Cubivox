using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cubvox.Renderobjects
{
    public interface Indices
    {
        List<int> GetFront(int startingIndex);
        List<int> GetBack(int startingIndex);
        List<int> GetTop(int startingIndex);
        List<int> GetBottom(int startingIndex);
        List<int> GetRight(int startingIndex);
        List<int> GetLeft(int startingIndex);
    }
}