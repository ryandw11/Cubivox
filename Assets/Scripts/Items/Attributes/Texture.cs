using System;

namespace Cubivox.Items
{
    [AttributeUsage(AttributeTargets.Class)]
    public class Texture : Attribute
    {
        private string texture;
        public Texture(string texture)
        {
            this.texture = texture;
        }

        public string GetTexture()
        {
            return texture;
        }
    }
}
