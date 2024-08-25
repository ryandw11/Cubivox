using CubivoxClient.Texturing;
using CubivoxCore;
using CubivoxCore.Items;
using CubivoxCore.Voxels;
using System.Collections.Generic;
using System.Linq;

namespace CubivoxClient
{
    public class ClientItemRegistry : ItemRegistry
    {
        private ClientTextureAtlas textureAtlas;
        private Dictionary<ControllerKey, Item> itemDictionary;

        // Global Voxel Id Map
        private Dictionary<short, VoxelDef> voxelMap;
        private Dictionary<VoxelDef, short> reverseVoxelMap;
        private short currentVoxelIndex;

        public ClientItemRegistry(ClientTextureAtlas textureAtlas)
        {
            this.textureAtlas = textureAtlas;
            itemDictionary = new Dictionary<ControllerKey, Item>();
            voxelMap = new Dictionary<short, VoxelDef>();
            reverseVoxelMap = new Dictionary<VoxelDef, short>();
            currentVoxelIndex = 0;
        }

        public Item GetItem(ControllerKey key)
        {
            return itemDictionary[key];
        }

        public List<Item> GetItems()
        {
            return itemDictionary.Values.ToList();
        }

        public void RegisterItem(Item item)
        {
            itemDictionary.Add(item.GetControllerKey(), item);

            // Keep track of the voxel map.
            VoxelDef voxelDef = item as VoxelDef;
            if(item != null)
            {
                voxelMap[currentVoxelIndex] = voxelDef;
                reverseVoxelMap[voxelDef] = currentVoxelIndex;
                currentVoxelIndex++;

                if (voxelDef.GetAtlasTexture() != null)
                {
                    textureAtlas.RegisterTexture(voxelDef.GetAtlasTexture(), false);
                }
            }
        }

        public void UnregisterItem(Item item)
        {
            itemDictionary.Remove(item.GetControllerKey());
        }

        public VoxelDef GetVoxelDef(short id)
        {
            return voxelMap[id];
        }

        public short GetVoxelDefId(VoxelDef voxelDef)
        {
            return reverseVoxelMap[voxelDef];
        }

        public VoxelDef GetVoxelDefinition(ControllerKey key)
        {
            Item item = itemDictionary[key];
            if (item is VoxelDef)
            {
                return (VoxelDef) item;
            }

            return null;
        }
    }
}
