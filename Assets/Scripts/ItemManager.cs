using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cubivox.Items;
using Cubivox.Items.Default;

public class ItemManager : MonoBehaviour
{
    private static ItemManager instance;

    private List<Item> items = new List<Item>();
    private List<Block> blocks = new List<Block>();
    private Dictionary<string, ICubivoxObject> objects = new Dictionary<string, ICubivoxObject>();

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        // ======== [Default Items] =========
        AddItem(new StandardCube());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static ItemManager GetInstance()
    {
        return instance;
    }

    public void AddItem(ICubivoxObject sandboxObject)
    {
        this.objects.Add(sandboxObject.GetName(), sandboxObject);
        if (sandboxObject is Block)
            this.blocks.Add((Block)sandboxObject);
        else if (sandboxObject is Item)
            this.items.Add((Item)sandboxObject);
    }

    public List<Block> GetBlocks()
    {
        return this.blocks;
    }

    public List<Item> GetItems()
    {
        return items;
    }

    public Dictionary<string, ICubivoxObject> GetSandboxObjects()
    {
        return objects;
    }

    public  T GetObjectByName<T>(string name) where T : ICubivoxObject
    {
        return (T) objects[name];
    }
}
