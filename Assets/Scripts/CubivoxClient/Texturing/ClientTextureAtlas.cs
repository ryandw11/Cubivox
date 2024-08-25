using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CubivoxCore;
using CubivoxCore.Mods;
using CubivoxCore.Texturing;

using UnityEngine;

namespace CubivoxClient.Texturing
{
    public class ClientTextureAtlas : TextureAtlas
    {
        private List<ClientAtlasTexture> mTextures;
        private int mTextureWidth;
        private int mTextureHeight;
        private Texture2D mTexture;
        private int mNumberOfRows;
        private Material mMaterial;

        public event AtlasRecalculatedEventHandler AtlasRecalculatedEvent;

        /**
         * <summary>Create a new texture atlas.</summary>
         * <param name="textures">The list of textures.</param>
         * <param name="textureWidth">The width of the textures.</param>
         * <param name="textureHeight">The height of the textures.</param>
         */
        public ClientTextureAtlas(List<ClientAtlasTexture> textures, int textureWidth, int textureHeight)
        {
            mTextures = textures;
            mTextureWidth = textureWidth;
            mTextureHeight = textureHeight;

            mMaterial = new Material(Shader.Find("Standard"));

            CalculateTextureAtlas(textures);
        }

        /**
         * <summary>Create a new texture atlas with the default size.</summary>
         * <param name="textures">The list of textures.</param>
         */
        public ClientTextureAtlas(List<ClientAtlasTexture> textures) : this(textures, 400, 300) { }

        public int GetNumberOfRows()
        {
            return mNumberOfRows;
        }

        public int GetTextureHeight()
        {
            return mTextureHeight;
        }

        public List<AtlasTexture> GetTextures()
        {
            return mTextures.Select(texture => (AtlasTexture) texture).ToList();
        }

        public int GetTextureWidth()
        {
            return mTextureWidth;
        }

        public float GetXOffset(int id)
        {
            return ((float)id % mNumberOfRows) / mNumberOfRows;
        }

        public float GetYOffset(int id)
        {
            return Mathf.Floor(id / mNumberOfRows) / mNumberOfRows;
        }

        public void RecalculateTextureAtlas()
        {
            CalculateTextureAtlas(mTextures);
            mMaterial.mainTexture = mTexture;

            AtlasRecalculatedEvent?.Invoke();
        }

        public void RegisterTexture(AtlasTexture texture, bool recalculateAtlas)
        {
            if(!(texture is ClientAtlasTexture))
            {
                throw new ArgumentException("Provided texture must be a client texture and non-null!", "texture");
            }

            mTextures.Add((ClientAtlasTexture) texture);

            if (recalculateAtlas)
                RecalculateTextureAtlas();
        }

        public void SetTextureSize(int width, int height)
        {
            mTextureWidth = width;
            mTextureHeight = height;
            RecalculateTextureAtlas();
        }

        public Texture2D GetTexture()
        {
            return mTexture;
        }

        public Material GetMaterial()
        {
            return mMaterial;
        }

        /**
         * <summary>Calculate required data for the texture atlas.</summary>
         * <param name="textures">The textures to compile into the atlas.</param>
         */
        private void CalculateTextureAtlas(List<ClientAtlasTexture> atlasTextures)
        {
            int numOfRows = (int)Mathf.Ceil(Mathf.Sqrt(atlasTextures.Count));
            mNumberOfRows = numOfRows;

            int atlasWidth = mTextureWidth * numOfRows;
            int atlasHeight = mTextureHeight * numOfRows;

            // If there are no textures in the texture atlas.
            if(atlasWidth == 0 || atlasHeight == 0)
            {
                mTexture = new Texture2D(1, 1);
                return;
            }

            List<Texture2D> textures = new List<Texture2D>();
            atlasTextures.ForEach(atlasTexture => {
                Texture2D texture = GetTexture(atlasTexture);
                if (texture == null)
                {
                    Cubivox.GetInstance().GetLogger().Error($"Failed to load atlas texture {atlasTexture.Location}, resource not found!");
                    return;
                }
                textures.Add(texture);
            });

            Texture2D atlas = new Texture2D(atlasWidth, atlasHeight);

            int index = 0;
            for (int y = 0; y < atlasHeight; y += mTextureHeight)
            {
                for (int x = 0; x < atlasWidth; x += mTextureWidth)
                {
                    if (index >= textures.Count) break;

                    atlas.SetPixels(x, y, mTextureWidth, mTextureHeight, Resize(textures[index], mTextureWidth, mTextureHeight).GetPixels());
                    atlasTextures[index].Init(index, GetXOffset(index), GetYOffset(index));
                    index++;
                }
            }
            atlas.Apply();
            mTexture = atlas;
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

        private Texture2D GetTexture(AtlasTexture atlasTexture)
        {
            switch(atlasTexture.Root)
            {
                case TextureRoot.CUBIVOX:
                    Texture2D realText = Resources.Load<Texture2D>(atlasTexture.Location);
                    if( realText == null )
                    {
                        return null;
                    }
                    Texture2D cloneText = new Texture2D(realText.width, realText.height);
                    cloneText.SetPixels(realText.GetPixels());
                    cloneText.Apply();
                    return cloneText;
                case TextureRoot.EMBEDDED:
                    Texture2D outputTexture = new Texture2D(2, 2);
                    using (Stream textureStream = atlasTexture.GetEmbeddedResourceStream())
                    {
                        if( textureStream == null )
                        {
                            return null;
                        }
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            textureStream.CopyTo(memoryStream);
                            if( !ImageConversion.LoadImage(outputTexture, memoryStream.ToArray()) )
                            {
                                return null;
                            }
                        }
                    }

                    return outputTexture;
            }

            throw new Exception("Not all texture locations are handled!");
        }

        public AtlasTexture CreateAtlasTexture(Mod mod, TextureRoot root, string location)
        {
            return new ClientAtlasTexture(mod, root, location);
        }
    }
}
