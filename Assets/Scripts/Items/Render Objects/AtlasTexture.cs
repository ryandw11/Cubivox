using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cubvox.Renderobjects
{
    /**
     * <summary>This represents a texture on the texture atlas.</summary>
     */
    public class AtlasTexture
    {
        /**
         * <summary>Get the string location of the texture. (Must be in resource folder.)</summary>
         */
        public string location { private set; get; }

        /**
         * <summary>Get the id of the texture.</summary>
         * <remarks>Will be 0 until the texture atlas is calculated.</remarks>
         */
        public int id { private set;  get; }

        /**
         * <summary>Get the x offset of the texture on the atlas.</summary>
         * <remarks>WIll be 0 until the texture atlas is calculated.</remarks>
         */
        public float xOffset { private set; get; }

        /**
         * <summary>Get the y offset of the texture on the atlas.</summary>
         * <remarks>WIll be 0 until the texture atlas is calculated.</remarks>
         */
        public float yOffset { private set; get; }

        /**
         * <summary>Create a new texture atlas.</summary>
         * <param name="location">The location of the texture. (Must be in the Resources folder).</param>
         */
        public AtlasTexture(string location)
        {
            this.location = location.Replace(".png", "");
        }

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
