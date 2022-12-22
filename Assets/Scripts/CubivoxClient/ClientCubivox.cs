using CubivoxClient.Items.Voxels;
using CubivoxCore;
using CubivoxCore.BaseGame.VoxelDefs;
using UnityEngine;

namespace CubivoxClient
{
    public class ClientCubivox : Cubivox
    {
        public ClientCubivox()
        {
            instance = this;
            itemRegistry = new ClientItemRegistry();
        }

        public override EnvType GetEnvType()
        {
            return EnvType.CLIENT;
        }

        public override void OnEnable()
        {
            itemRegistry.RegisterItem(new AirVoxel());
            itemRegistry.RegisterItem(new TestVoxel(this));
        }
    }
}
