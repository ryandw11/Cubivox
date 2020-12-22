using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sandbox.Items;
using Sandbox.Items.Default;

public class ItemManager : MonoBehaviour
{
    private static ItemManager instance;

    private List<Item> items = new List<Item>();
    private List<Block> blocks = new List<Block>();
    private Dictionary<string, ISandboxObject> objects = new Dictionary<string, ISandboxObject>();

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

    public void AddItem(ISandboxObject sandboxObject)
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

    public Dictionary<string, ISandboxObject> GetSandboxObjects()
    {
        return objects;
    }

    public  T GetObjectByName<T>(string name) where T : ISandboxObject
    {
        return (T) objects[name];
    }
}
