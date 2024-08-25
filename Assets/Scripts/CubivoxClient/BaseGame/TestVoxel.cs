using System.Runtime;

using CubivoxCore.Attributes;
using CubivoxCore.Mods;
using CubivoxCore.Texturing;
using CubivoxCore.Voxels;

namespace CubivoxClient.BaseGame
{
    [Name("TestBlock")]
    [Key("TESTBLOCK")]
    [Texture(TextureRoot.CUBIVOX, "Textures/ExampleBlock")]
    public class TestVoxel : ModVoxel
    {
        public TestVoxel(Mod mod) : base(mod)
        { }
    }
}
