using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CubivoxCore.BaseGame.Texturing;

namespace CubivoxClient.Texturing
{
    public class ClientAtlasTexture : AtlasTexture
    {
        public ClientAtlasTexture(string location) : base(location.Replace(".png", ""))
        {}

        /**
         * <summary>Initalize the data for the texture atlas.</summary>
         * <param name="id">The id of the atlas texture.</param>
         * <param name="xOffset">The x offset of the atlas texture.</param>
         * <param name="yOffset">The y offset of the atlas texture.</param>
         */
        internal void Init(int id, float xOffset, float yOffset)
        {
            this.id = id;
            this.xOffset = xOffset;
            this.yOffset = yOffset;
        }
    }
}
