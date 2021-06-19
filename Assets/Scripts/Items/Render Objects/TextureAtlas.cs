using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cubivox.Renderobjects
{
    /**
     * <summary>This represents the TextureAtlas.
     * 
     * <para>The current texture atlas can be obtained via <see cref="WorldManager"/></para>
     * </summary>
     */
    public class TextureAtlas
    {
        private string output;
        private List<AtlasTexture> textures;
        private int textureWidth;
        private int textureHeight;
        private Texture2D texture;
        private int numberOfRows;

        /**
         * <summary>Create a new texture atlas.</summary>
         * <param name="textures">The list of textures.</param>
         * <param name="output">The output value.</param>
         * <param name="textureWidth">The width of the textures.</param>
         * <param name="textureHeight">The height of the textures.</param>
         */
        public TextureAtlas(List<AtlasTexture> textures, string output, int textureWidth, int textureHeight)
        {
            this.textures = textures;
            this.output = output;
            this.textureWidth = textureWidth;
            this.textureHeight = textureHeight;

            calculateTextureAtlas(textures);
        }

        /**
         * <summary>Create a new texture atlas with the default size.</summary>
         * <param name="textures">The list of textures.</param>
         * <param name="output">The output.</param>
         */
        public TextureAtlas(List<AtlasTexture> textures, string output) : this(textures, output, 400, 300) { }

        /**
         * <summary>Get the list of textures.</summary>
         * <returns>The list of textures.</returns>
         */
        public List<AtlasTexture> GetTextures()
        {
            return textures;
        }

        /**
         * <summary>Get the number of rows in the texture atlas.</summary>
         * <returns>The number of rows in the texture atlas. (This number is also the number of columns.)</returns>
         */
        public int GetNumberOfRows()
        {
            return numberOfRows;
        }

        /**
         * <summary>Get the x offset (using texture coords) using the id.</summary>
         * <param name="id">The id to get the x offset for.</param>
         * <returns>The x offset.</returns>
         */
        public float GetXOffset(int id)
        {
            return ((float)id % numberOfRows) / numberOfRows;
        }

        /**
         * <summary>Get the y offset (using texture coords) using the id.</summary>
         * <param name="id">The id to get the y offset for.</param>
         * <returns>The y offset.</returns>
         */
        public float GetYOffset(int id)
        {
            return Mathf.Floor(id / numberOfRows) / numberOfRows;
        }

        /**
         * <summary>Calculate required data for the texture atlas.</summary>
         * <param name="textures">The textures to compile into the atlas.</param>
         */
        private void calculateTextureAtlas(List<AtlasTexture> textures)
        {
            int numOfRows = (int)Mathf.Ceil(Mathf.Sqrt(textures.Count));
            this.numberOfRows = numOfRows;

            int w = textureWidth * numOfRows;
            int h = textureHeight * numOfRows;

            List<Texture2D> texData = new List<Texture2D>();
            textures.ForEach(text => {
                Texture2D realText = Resources.Load<Texture2D>(text.location);
                Texture2D cloneText = new Texture2D(realText.width, realText.height);
                cloneText.SetPixels(realText.GetPixels());
                cloneText.Apply();
                texData.Add(cloneText);
            });

            Texture2D atlas = new Texture2D(w, h);

            int i = 0;
            for(int y = 0; y < h; y += textureHeight)
            {
                for(int x = 0; x < w; x += textureWidth)
                {
                    if (i >= texData.Count) break;

                    atlas.SetPixels(x, y, textureWidth, textureHeight, Resize(texData[i], textureWidth, textureHeight).GetPixels());
                    textures[i].Init(i, GetXOffset(i), GetYOffset(i));
                    i++;
                }
            }
            atlas.Apply();
            this.texture = atlas;
        }

        /**
         * <summary>Resize a Texture.</summary>
         * <param name="texture">The texture to resize.</param>
         * <param name="targetX">The target x size.</param>
         * <param name="targetY">The target y size.</param>
         * <returns>The resized image.</returns>
         */
        private Texture2D Resize(Texture2D texture, int targetX, int targetY)
        {
            RenderTexture rt = new RenderTexture(targetX, targetY, 24);
            RenderTexture.active = rt;
            Graphics.Blit(texture, rt);
            Texture2D result = new Texture2D(targetX, targetY);
            result.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
            result.Apply();
            return result;
        }

        /**
         * <summary>Get the texture for the atlas itself.</summary>
         * <returns>The texture for the atlas itself.</returns>
         */
        public Texture2D GetTexture()
        {
            return texture;
        }

        /**
         * <summary>Reclaculate the texture atlas.</summary>
         */
        public void RecalculateTextureAtlas()
        {
            calculateTextureAtlas(textures);
        }

        /**
         * <summary>Get the texture width.</summary>
         * <returns>The texture width.</returns>
         */
        public int GetTextureWidth()
        {
            return textureWidth;
        }

        /**
         * <summary>Get the texture height.</summary>
         * <returns>The texture height.</returns>
         */
        public int GetTextureHeight()
        {
            return textureHeight;
        }

        /**
         * <summary>Set the texture size.</summary>
         * <param name="width">The width.</param>
         * <param name="height">The height.</param>
         */
        public void SetTextureSize(int width, int height)
        {
            this.textureWidth = width;
            this.textureHeight = height;
            RecalculateTextureAtlas();
        }
    }
}
