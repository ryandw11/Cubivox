using CubivoxCore.Attributes;
using CubivoxCore.Mods;
using CubivoxCore.Voxels;

namespace CubivoxClient.BaseGame
{
    [Name("TestBlock")]
    [Key("TESTBLOCK")]
    [Texture("Textures/ExampleBlock")]
    public class TestVoxel : ModVoxel
    {
        public TestVoxel(Mod mod) : base(mod)
        { }
    }
}
