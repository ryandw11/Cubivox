using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cubvox.Items;

namespace Cubvox.Renderobjects
{
    /**
     * <summary>
     * This represents the block in a render chunk. A render block contains block data for blocks.
     * <para>See <see cref="Block"/> for info on a block.</para>
     * <para>It is important to note that items not apart of the voxel system (entities) are not render blocks or in the render system.</para>
     * </summary>
     */
    public class RenderBlock
    {
        private Layout layout;
        private AtlasTexture atlasTexture;
        private Block block;
        private Color color;

        private Vector3 position;

        private RenderChunk parentChunk;
        private List<Face> visibleFaces;
        public bool selected { get; set; }

        /**
         * <summary>Create a new render block.</summary>
         * <param name="layout">The layout to use.</param>
         * <param name="atlasTexture">The atlas texture.</param>
         * <param name="position">The position of the block. (Must be 0 - 15 for all values)</param>
         */
        public RenderBlock(Layout layout, AtlasTexture atlasTexture, Vector3 position)
        {
            this.layout = layout;
            this.atlasTexture = atlasTexture;
            this.position = position;
            this.visibleFaces = new List<Face>();
            this.selected = false;
            this.color = new Color();
        }

        /**
         * <summary>Create a render block with the default layout.</summary>
         * <param name="atlasTexture">The atlas texture used.</param>
         * <param name="position">The position of the block. (Must be 0 - 15 for all values)</param>
         */
        public RenderBlock(AtlasTexture atlasTexture, Vector3 position) : this(new BlockLayout(), atlasTexture, position)
        {}

        /**
         * <summary>Create a render block from a block item.</summary>
         * <param name="block">The block to base the render block off of.</param>
         * <param name="position">The position of the block. (Must be 0 - 15 for all values)</param>
         */
        public RenderBlock(Block block, Vector3 position) : this(block.GetAtlasTexture(), position)
        {
            this.block = block;
        }

        /**
         * <summary>Get the <see cref="Block"/> assigned to this render block.</summary>
         * <remarks>This can be null if the block was not set during construction.</remarks>
         * <returns>The <see cref="Block"/> assigned to this render block.</returns>
         */
        public Block GetBlock()
        {
            return block;
        }

        /**
         * <summary>Set the color of the render block.</summary>
         * <param name="color">The color to set the the render block to.</param>
         */
        public void SetColor(Color color)
        {
            this.color = color;
        }

        /**
         * <summary>Get the color of the render block.</summary>
         * <returns>The color of the render block.</returns>
         */
        public Color GetColor()
        {
            return color;
        }

        /**
         * <summary>Get the list of visible faces.</summary>
         * <returns>The list of visible faces.</returns>
         */
        public List<Face> GetVisibleFaces()
        {
            return visibleFaces;
        }

        /**
         * <summary>Get the position of the block.</summary>
         * <returns>The position of the block.</returns>
         */
        public Vector3 GetPosition()
        {
            return position;
        }

        /**
         * <summary>Set the position of the block.</summary>
         * <param name="position">The position to set.</param>
         */
        public void SetPosition(Vector3 position)
        {
            this.position = position;
        }

        /**
         * <summary>Get the chunk that this block belongs to.</summary>
         * <remarks>Returns null if this block does not have a parent chunk.</remarks>
         * <returns>The parent chunk.</returns>
         */
        public RenderChunk GetParentChunk()
        {
            return parentChunk;
        }

        /**
         * <summary>Sets the parent chunk.</summary>
         * <param name="chunk">The parent chunk.</param>
         */
        internal void SetParentChunk(RenderChunk chunk)
        {
            this.parentChunk = chunk;
        }

        /**
         * <summary>Get the texture of the block.</summary>
         */
        public AtlasTexture GetTexture()
        {
            return atlasTexture;
        }

        /**
         * <summary>Get the layout for the block.</summary>
         */
        public Layout GetLayout()
        {
            return layout;
        }

        /**
         * <summary>Add a face to the block.</summary>
         * <remarks>Internal use only.</remarks>
         * <param name="f">The face to add.</param>
         */
        public void AddFace(Face f)
        {
            visibleFaces.Add(f);
        }

        /**
         * <summary>Clear the list of visible faces.</summary>
         */
        public void ClearFaces()
        {
            visibleFaces.Clear();
        }

        /**
         * <summary>Mutate the list of vertices with the visible faces from this block.</summary>
         * <param name="vertex">The list of vertices. (this value is mutated)</param>
         */
        public void GetVertexFromFaces(List<Vector3> vertex)
        {
            foreach(Face f in this.visibleFaces)
            {
                switch (f)
                {
                    case Face.FRONT:
                       vertex.AddRange(layout.GetVertex(GetPosition()).GetFront());
                       break;
                    case Face.BACK:
                        vertex.AddRange(layout.GetVertex(GetPosition()).GetBack());
                        break;
                    case Face.TOP:
                        vertex.AddRange(layout.GetVertex(GetPosition()).GetTop());
                        break;
                    case Face.BOTTOM:
                        vertex.AddRange(layout.GetVertex(GetPosition()).GetBottom());
                        break;
                    case Face.LEFT:
                        vertex.AddRange(layout.GetVertex(GetPosition()).GetLeft());
                        break;
                    case Face.RIGHT:
                        vertex.AddRange(layout.GetVertex(GetPosition()).GetRight());
                        break;
                }
            }
        }

        /**
         * <summary>Mutate the list of texture points with the visible faces from this block.</summary>
         * <param name="texture">The list of texture points. (this value is mutated)</param>
         * <param name="atlas">The texture atlas to use.</param>
         */
        public void GetTextureFromFaces(List<Vector2> texture, TextureAtlas atlas)
        {
            foreach(Face f in visibleFaces)
            {
                switch (f)
                {
                    case Face.FRONT:
                        texture.AddRange(layout.GetTextureCords().GetFront(GetTexture().xOffset, GetTexture().yOffset, atlas.GetNumberOfRows()));
                        break;
                    case Face.BACK:
                        texture.AddRange(layout.GetTextureCords().GetBack(GetTexture().xOffset, GetTexture().yOffset, atlas.GetNumberOfRows()));
                        break;
                    case Face.TOP:
                        texture.AddRange(layout.GetTextureCords().GetTop(GetTexture().xOffset, GetTexture().yOffset, atlas.GetNumberOfRows()));
                        break;
                    case Face.BOTTOM:
                        texture.AddRange(layout.GetTextureCords().GetBottom(GetTexture().xOffset, GetTexture().yOffset, atlas.GetNumberOfRows()));
                        break;
                    case Face.LEFT:
                        texture.AddRange(layout.GetTextureCords().GetLeft(GetTexture().xOffset, GetTexture().yOffset, atlas.GetNumberOfRows()));
                        break;
                    case Face.RIGHT:
                        texture.AddRange(layout.GetTextureCords().GetRight(GetTexture().xOffset, GetTexture().yOffset, atlas.GetNumberOfRows()));
                        break;
                }
            }
        }

        /**
         * <summary>Mutate the list of normals with the visible faces from this block.</summary>
         * <param name="vertex">The list of normals. (this value is mutated)</param>
         */
        public void GetNormalsFromFaces(List<Vector3> vertex)
        {
            foreach(Face f in visibleFaces)
            {
                switch (f)
                {
                    case Face.FRONT:
                        vertex.AddRange(layout.GetNormal().GetFront());
                        break;
                    case Face.BACK:
                        vertex.AddRange(layout.GetNormal().GetBack());
                        break;
                    case Face.TOP:
                        vertex.AddRange(layout.GetNormal().GetTop());
                        break;
                    case Face.BOTTOM:
                        vertex.AddRange(layout.GetNormal().GetBottom());
                        break;
                    case Face.LEFT:
                        vertex.AddRange(layout.GetNormal().GetLeft());
                        break;
                    case Face.RIGHT:
                        vertex.AddRange(layout.GetNormal().GetRight());
                        break;
                }
            }
        }

        /**
         * <summary>Mutate the list of indices with the visible faces from this block.</summary>
         * <param name="vertex">The list of indices. (this value is mutated)</param>
         * <param name="currentIndex">The current index of the indicies.</param>
         */
        public void GetIndicesFromFaces(List<int> vertex, int currentIndex)
        {
            int index = currentIndex;
            foreach (Face f in this.visibleFaces)
            {
                switch (f)
                {
                    case Face.FRONT:
                        vertex.AddRange(layout.GetIndices().GetFront(index));
                        break;
                    case Face.BACK:
                        vertex.AddRange(layout.GetIndices().GetBack(index));
                        break;
                    case Face.TOP:
                        vertex.AddRange(layout.GetIndices().GetTop(index));
                        break;
                    case Face.BOTTOM:
                        vertex.AddRange(layout.GetIndices().GetBottom(index));
                        break;
                    case Face.LEFT:
                        vertex.AddRange(layout.GetIndices().GetLeft(index));
                        break;
                    case Face.RIGHT:
                        vertex.AddRange(layout.GetIndices().GetRight(index));
                        break;
                }
                index += 4;
            }
        }

    }
}
