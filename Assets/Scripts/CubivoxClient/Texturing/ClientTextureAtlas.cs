using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using CubivoxCore.BaseGame.Texturing;

namespace CubivoxClient.Texturing
{
    public class ClientTextureAtlas : TextureAtlas
    {
        private string output;
        private List<ClientAtlasTexture> textures;
        private int textureWidth;
        private int textureHeight;
        private Texture2D texture;
        private int numberOfRows;
        private Material material;

        /**
         * <summary>Create a new texture atlas.</summary>
         * <param name="textures">The list of textures.</param>
         * <param name="output">The output value.</param>
         * <param name="textureWidth">The width of the textures.</param>
         * <param name="textureHeight">The height of the textures.</param>
         */
        public ClientTextureAtlas(List<ClientAtlasTexture> textures, string output, int textureWidth, int textureHeight)
        {
            this.textures = textures;
            this.output = output;
            this.textureWidth = textureWidth;
            this.textureHeight = textureHeight;

            material = new Material(Shader.Find("Standard"));

            CalculateTextureAtlas(textures);
        }

        /**
         * <summary>Create a new texture atlas with the default size.</summary>
         * <param name="textures">The list of textures.</param>
         * <param name="output">The output.</param>
         */
        public ClientTextureAtlas(List<ClientAtlasTexture> textures, string output) : this(textures, output, 400, 300) { }

        public int GetNumberOfRows()
        {
            return numberOfRows;
        }

        public int GetTextureHeight()
        {
            return textureHeight;
        }

        public List<AtlasTexture> GetTextures()
        {
            return textures.Select(texture => (AtlasTexture) texture).ToList();
        }

        public int GetTextureWidth()
        {
            return textureWidth;
        }

        public float GetXOffset(int id)
        {
            return ((float)id % numberOfRows) / numberOfRows;
        }

        public float GetYOffset(int id)
        {
            return Mathf.Floor(id / numberOfRows) / numberOfRows;
        }

        public void RecalculateTextureAtlas()
        {
            CalculateTextureAtlas(textures);
            material.mainTexture = texture;
        }

        public void RegisterTexture(AtlasTexture texture, bool recalculateAtlas)
        {
            if(!(texture is ClientAtlasTexture))
            {
                throw new ArgumentException("Provided texture must be a client texture!", "texture");
            }

            textures.Add((ClientAtlasTexture) texture);

            if (recalculateAtlas)
                RecalculateTextureAtlas();
        }

        public void SetTextureSize(int width, int height)
        {
            textureWidth = width;
            textureHeight = height;
            RecalculateTextureAtlas();
        }

        public Texture2D GetTexture()
        {
            return texture;
        }

        public Material GetMaterial()
        {
            return material;
        }

        /**
         * <summary>Calculate required data for the texture atlas.</summary>
         * <param name="textures">The textures to compile into the atlas.</param>
         */
        private void CalculateTextureAtlas(List<ClientAtlasTexture> textures)
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
            for (int y = 0; y < h; y += textureHeight)
            {
                for (int x = 0; x < w; x += textureWidth)
                {
                    if (i >= texData.Count) break;

                    atlas.SetPixels(x, y, textureWidth, textureHeight, Resize(texData[i], textureWidth, textureHeight).GetPixels());
                    textures[i].Init(i, GetXOffset(i), GetYOffset(i));
                    i++;
                }
            }
            atlas.Apply();
            texture = atlas;
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

        public AtlasTexture CreateAtlasTexture(string location)
        {
            return new ClientAtlasTexture(location);
        }
    }
}
