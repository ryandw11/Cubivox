using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cubvox.Renderobjects
{
    public interface Texture
    {
        List<Vector2> GetFront(float xOffset, float yOffset, int rows);
        List<Vector2> GetBack(float xOffset, float yOffset, int rows);
        List<Vector2> GetTop(float xOffset, float yOffset, int rows);
        List<Vector2> GetBottom(float xOffset, float yOffset, int rows);
        List<Vector2> GetRight(float xOffset, float yOffset, int rows);
        List<Vector2> GetLeft(float xOffset, float yOffset, int rows);
    }
}
