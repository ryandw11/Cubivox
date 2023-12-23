using CubivoxCore.Mods;
using CubivoxCore.Attributes;

namespace CubivoxClient.Items.Voxels
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
