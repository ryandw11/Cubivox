using CubivoxCore;
using CubivoxCore.BaseGame;
using System.Collections.Generic;
using System.Linq;

namespace CubivoxClient
{
    public class ClientItemRegistry : ItemRegistry
    {
        private Dictionary<ControllerKey, Item> itemDictionary;

        // Global Voxel Id Map
        private Dictionary<short, VoxelDef> voxelMap;
        private Dictionary<VoxelDef, short> reverseVoxelMap;
        private short currentVoxelIndex;

        public ClientItemRegistry()
        {
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
            if(item is VoxelDef)
            {
                voxelMap[currentVoxelIndex] = (VoxelDef) item;
                reverseVoxelMap[(VoxelDef)item] = currentVoxelIndex;
                currentVoxelIndex++;
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
    }
}
