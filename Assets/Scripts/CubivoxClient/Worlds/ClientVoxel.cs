using CubivoxCore;
using CubivoxCore.BaseGame.VoxelDefs;
using CubivoxCore.Voxels;
using CubivoxCore.Worlds;

using CubivoxRender;

namespace CubivoxClient.Worlds
{
    public class ClientVoxel : Voxel
    {
        private Location location;
        private VoxelDef voxelDef;
        private ClientChunk clientChunk;

        public ClientVoxel(Location location, VoxelDef voxelDef)
        {
            this.location = location;
            this.voxelDef = voxelDef;
        }

        public Chunk GetChunk()
        {
            return clientChunk;
        }

        public Location GetLocation()
        {
            return location;
        }

        public VoxelDef GetVoxelDef()
        {
            return voxelDef;
        }

        public void SetVoxelDef(VoxelDef voxelDef)
        {
            this.voxelDef = voxelDef;
        }

        public RenderVoxel GetRenderVoxel()
        {
            RenderVoxel renderVoxel = new RenderVoxel
            {
                xOffset = voxelDef.GetAtlasTexture()?.XOffset ?? 0,
                yOffset = voxelDef.GetAtlasTexture()?.YOffset ?? 0,
                rows = Cubivox.GetTextureAtlas().GetNumberOfRows(),
                transparent = voxelDef.IsTransparent(),
                empty = voxelDef is AirVoxel
            };
            return renderVoxel;
        }
    }
}