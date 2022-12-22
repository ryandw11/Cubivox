using CubivoxCore;
using CubivoxCore.BaseGame;
using System.Collections.Generic;
using System.Linq;

namespace CubivoxClient
{
    public class ClientItemRegistry : ItemRegistry
    {
        private Dictionary<ControllerKey, Item> itemDictionary;

        public ClientItemRegistry()
        {
            itemDictionary = new Dictionary<ControllerKey, Item>();
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
        }

        public void UnregisterItem(Item item)
        {
            itemDictionary.Remove(item.GetControllerKey());
        }
    }
}
