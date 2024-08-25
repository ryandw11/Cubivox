using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CubivoxCore.Mods;
using CubivoxCore.Texturing;

namespace CubivoxClient.Texturing
{
    public class ClientAtlasTexture : AtlasTexture
    {
        public ClientAtlasTexture(Mod mod, TextureRoot root, string location) : base(mod, root, location)
        {
            if (root == TextureRoot.CUBIVOX)
            {
                // Unity Resources don't need an extension.
                Location = Location.Replace(".png", "");
            }
        }

        /**
         * <summary>Initalize the data for the texture atlas.</summary>
         * <param name="id">The id of the atlas texture.</param>
         * <param name="xOffset">The x offset of the atlas texture.</param>
         * <param name="yOffset">The y offset of the atlas texture.</param>
         */
        internal void Init(int id, float xOffset, float yOffset)
        {
            Id = id;
            XOffset = xOffset;
            YOffset = yOffset;
        }
    }
}
