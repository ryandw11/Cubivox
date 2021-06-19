using UnityEngine;
using Cubivox.Renderobjects;

namespace Cubivox.Items
{
    public abstract class CubivoxObject : ICubivoxObject
    {
        protected string name;
        protected string texture;
        protected AtlasTexture atlasTexture;

        public CubivoxObject()
        {
            Name name = (Name)GetType().GetCustomAttributes(typeof(Name), true)[0];
            this.name = name.GetValue();
            Texture texture = (Texture)GetType().GetCustomAttributes(typeof(Texture), true)[0];
            this.texture = texture.GetTexture();
        }

        public abstract string GetModel();

        public string GetName()
        {
            return name;
        }

        public string GetTexture()
        {
            return texture;
        }

        internal void SetAtlasTexture(AtlasTexture atlasTexture)
        {
            this.atlasTexture = atlasTexture;
        }

        public AtlasTexture GetAtlasTexture()
        {
            return atlasTexture;
        }
    }
}
