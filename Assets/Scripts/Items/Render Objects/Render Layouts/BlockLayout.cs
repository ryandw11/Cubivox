using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cubivox.Renderobjects
{
    /**
     * <summary>The basic block implementation of the block layout.</summary>
     */
    public class BlockLayout : Layout
    {
        private const float ONE_THIRD = 1f / 3f;
        private const float TWO_THIRD = 2f / 3f;

        public Indices GetIndices()
        {
            return new IndicesImpl();
        }

        public Normal GetNormal()
        {
            return new NormalImpl();
        }

        public Texture GetTextureCords()
        {
            return new TextureImpl();
        }

        public Vertex GetVertex(Vector3 pos)
        {
            return new VertImpl(pos);
        }

        private class VertImpl : Vertex
        {
            private Vector3 pos;
            public VertImpl(Vector3 pos)
            {
                this.pos = pos;
            }

            public List<Vector3> GetFront()
            {
                return new List<Vector3>() {
                        new Vector3(-0.5f + pos.x, 0.5f + pos.y, -0.5f + pos.z),
                        new Vector3(-0.5f + pos.x, -0.5f + pos.y, -0.5f + pos.z),
                        new Vector3(0.5f + pos.x, -0.5f + pos.y, -0.5f + pos.z),
                        new Vector3(0.5f + pos.x, 0.5f + pos.y, -0.5f + pos.z)
                };
            }

            public List<Vector3> GetBottom()
            {
                return new List<Vector3>() {
                        new Vector3(-0.5f + pos.x, -0.5f + pos.y, -0.5f + pos.z),
                        new Vector3(-0.5f + pos.x, -0.5f + pos.y, 0.5f + pos.z),
                        new Vector3(0.5f + pos.x, -0.5f + pos.y, 0.5f + pos.z),
                        new Vector3(0.5f + pos.x, -0.5f + pos.y, -0.5f + pos.z)
                };
            }

            public List<Vector3> GetBack()
            {
                return new List<Vector3>() {
                        new Vector3(-0.5f + pos.x, 0.5f + pos.y, 0.5f + pos.z),
                        new Vector3(-0.5f + pos.x, -0.5f + pos.y, 0.5f + pos.z),
                        new Vector3(0.5f + pos.x, -0.5f + pos.y, 0.5f + pos.z),
                        new Vector3(0.5f + pos.x, 0.5f + pos.y, 0.5f + pos.z)
                };
            }

            public List<Vector3> GetLeft()
            {
                return new List<Vector3>() {
                        new Vector3(-0.5f + pos.x, 0.5f + pos.y, -0.5f + pos.z),
                        new Vector3(-0.5f + pos.x, -0.5f + pos.y, -0.5f + pos.z),
                        new Vector3(-0.5f + pos.x, -0.5f + pos.y, 0.5f + pos.z),
                        new Vector3(-0.5f + pos.x, 0.5f + pos.y, 0.5f + pos.z)
                };
            }

            public List<Vector3> GetRight()
            {
                return new List<Vector3>() {
                        new Vector3(0.5f + pos.x, 0.5f + pos.y, 0.5f + pos.z),
                        new Vector3(0.5f + pos.x, -0.5f + pos.y, 0.5f + pos.z),
                        new Vector3(0.5f + pos.x, -0.5f + pos.y, -0.5f + pos.z),
                        new Vector3(0.5f + pos.x, 0.5f + pos.y, -0.5f + pos.z)
                };
            }

            public List<Vector3> GetTop()
            {
                return new List<Vector3>() {
                        new Vector3(-0.5f + pos.x, 0.5f + pos.y, -0.5f + pos.z),
                        new Vector3(-0.5f + pos.x, 0.5f + pos.y, 0.5f + pos.z),
                        new Vector3(0.5f + pos.x, 0.5f + pos.y, 0.5f + pos.z),
                        new Vector3(0.5f + pos.x, 0.5f + pos.y, -0.5f + pos.z)
                };
            }
        }
      
        private class TextureImpl : Texture
        {
            public List<Vector2> GetBack(float xOffset, float yOffset, int rows)
            {
                return new List<Vector2>() {
                        new Vector2(1f / rows + xOffset, TWO_THIRD / rows + yOffset),
                        new Vector2(1f / rows + xOffset, ONE_THIRD / rows + yOffset),
                        new Vector2(0.75f / rows + xOffset, ONE_THIRD / rows + yOffset),
                        new Vector2(0.75f / rows + xOffset, TWO_THIRD / rows + yOffset),
                };
            }

            public List<Vector2> GetTop(float xOffset, float yOffset, int rows)
            {
                return new List<Vector2>() {
                        new Vector2(0.25f / rows + xOffset, TWO_THIRD / rows + yOffset),
                        new Vector2(0.25f / rows + xOffset, 1f / rows + yOffset),
                        new Vector2(0.5f / rows + xOffset, 1f / rows + yOffset),
                        new Vector2(0.5f / rows + xOffset, TWO_THIRD / rows + yOffset)
                };
            }

            public List<Vector2> GetFront(float xOffset, float yOffset, int rows)
            {
                return new List<Vector2>() {
                        new Vector2(0.25f / rows + xOffset, TWO_THIRD / rows + yOffset),
                        new Vector2(0.25f / rows + xOffset, ONE_THIRD / rows + yOffset),
                        new Vector2(0.5f / rows + xOffset, ONE_THIRD / rows + yOffset),
                        new Vector2(0.5f / rows + xOffset, TWO_THIRD / rows + yOffset),
                };
            }

            public List<Vector2> GetLeft(float xOffset, float yOffset, int rows)
            {
                return new List<Vector2>() {
                        new Vector2(0.25f / rows + xOffset, TWO_THIRD / rows + yOffset),
                        new Vector2(0.25f / rows + xOffset, ONE_THIRD / rows + yOffset),
                        new Vector2(0 / rows + xOffset, ONE_THIRD / rows + yOffset),
                        new Vector2(0 / rows + xOffset, TWO_THIRD / rows + yOffset),
                        
                };
            }

            public List<Vector2> GetRight(float xOffset, float yOffset, int rows)
            {
                return new List<Vector2>() {
                    new Vector2(0.75f / rows + xOffset, TWO_THIRD / rows + yOffset),
                        new Vector2(0.75f / rows + xOffset, ONE_THIRD / rows + yOffset),
                        new Vector2(0.5f / rows + xOffset, ONE_THIRD / rows + yOffset),
                        new Vector2(0.5f / rows + xOffset, TWO_THIRD / rows + yOffset),
                        
                };
            }

            public List<Vector2> GetBottom(float xOffset, float yOffset, int rows)
            {
                return new List<Vector2>() {
                        new Vector2(0.25f / rows + xOffset, ONE_THIRD / rows + yOffset),
                        new Vector2(0.25f / rows + xOffset, 0 + yOffset),
                        new Vector2(0.5f / rows + xOffset, 0 + yOffset),
                        new Vector2(0.5f / rows + xOffset, ONE_THIRD / rows + yOffset),
                };
            }
        }

        private class NormalImpl : Normal
        {
            public List<Vector3> GetFront()
            {
                return new List<Vector3>() {
                        new Vector3(0f, 0f, -1f),
                        new Vector3(0f, 0f, -1f),
                        new Vector3(0f, 0f, -1f),
                        new Vector3(0f, 0f, -1f)
                };
            }

            public List<Vector3> GetBottom()
            {
                return new List<Vector3>() {
                        new Vector3(0f, -1f, 0f),
                        new Vector3(0f, -1f, 0f),
                        new Vector3(0f, -1f, 0f),
                        new Vector3(0f, -1f, 0f)
                };
            }

            public List<Vector3> GetBack()
            {
                return new List<Vector3>() {
                        new Vector3(0f, 0f, 1f),
                        new Vector3(0f, 0f, 1f),
                        new Vector3(0f, 0f, 1f),
                        new Vector3(0f, 0f, 1f)
                };
            }

            public List<Vector3> GetLeft()
            {
                return new List<Vector3>() {
                        new Vector3(-1f, 0f, 0f),
                        new Vector3(-1f, 0f, 0f),
                        new Vector3(-1f, 0f, 0f),
                        new Vector3(-1f, 0f, 0f)
                };
            }

            public List<Vector3> GetRight()
            {
                return new List<Vector3>() {
                        new Vector3(1f, 0f, 0f),
                        new Vector3(1f, 0f, 0f),
                        new Vector3(1f, 0f, 0f),
                        new Vector3(1f, 0f, 0f)
                };
            }

            public List<Vector3> GetTop()
            {
                return new List<Vector3>() {
                        new Vector3(0f, 1f, 0f),
                        new Vector3(0f, 1f, 0f),
                        new Vector3(0f, 1f, 0f),
                        new Vector3(0f, 1f, 0f)
                };
            }
        }

        private class IndicesImpl : Indices
        {
            public List<int> GetFront(int i)
            {
                return new List<int>() { i, i + 3, i + 2, i + 2, i + 1, i  };
            }

            public List<int> GetBottom(int i)
            {
                return new List<int>() { i, i + 3, i + 2, i + 2, i + 1, i };
            }

            public List<int> GetBack(int i)
            {
                return new List<int>() { i, i + 1, i + 2, i + 2, i + 3, i };
            }

            public List<int> GetLeft(int i)
            {
                return new List<int>() { i, i + 1, i + 2, i + 2, i + 3, i };
            }

            public List<int> GetRight(int i)
            {
                return new List<int>() { i, i + 1, i + 2, i + 2, i + 3, i };
            }

            public List<int> GetTop(int i)
            {
                return new List<int>() { i, i + 1, i + 2, i + 2, i + 3, i };
            }
        }

    }
}
