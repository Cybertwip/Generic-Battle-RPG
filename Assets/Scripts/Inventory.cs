using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class Inventory : MonoBehaviour
{
    public class InventoryItem
    {
        public string Name { get; set; }
        public Item.Type Type { get; set; }
        public string MenuName { get; set; }
        public string Description { get; set; }

        public string GetPath { get; set; }
        public string IconPath { get; set; }
    }

    private static Inventory instance = null; // static (class level) variable
    public static Inventory Instance { get { return instance; } } // static getter (only accessing allowed)

    public List<InventoryItem> Items = new List<InventoryItem>();

    public List<GameObject> itemList = new List<GameObject>();
    public List<GameObject> magicList = new List<GameObject>();
    public List<GameObject> weaponList = new List<GameObject>();
    //public List<GameObject> armorList;
    //public List<GameObject> accessoryList;

    public GameObject activeWeapon;
    //public GameObject activeArmor;
    //public GameObject activeAccessory;

    public Item.Type GetItemTypeByName(string name)
    {
        return Items.First(i => i.Name == name).Type;
    }

    private void Awake()
    {
        // if instance is not yet set, set it and make it persistent between scenes
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            Items.Add(new InventoryItem()
            {
                Name = "Mushroom",
                Type = Item.Type.Support,
                MenuName = "Mushroom",
                Description = "Mushroom",
                GetPath = "Items/Prefabs/get_mushroom",
                IconPath = "Items/Prefabs/icon_mushroom"
            });

            Items.Add(new InventoryItem()
            {
                Name = "Power Star",
                Type = Item.Type.Support,
                MenuName = "Power Star",
                Description = "Power Star",
                GetPath = "Items/Prefabs/get_powerStar",
                IconPath = "Items/Prefabs/icon_powerStar"
            });

            Items.Add(new InventoryItem()
            {
                Name = "Turd",
                Type = Item.Type.Offensive,
                MenuName = "Turd",
                Description = "Chocolate Mousse",
                GetPath = "Items/Prefabs/get_turd",
                IconPath = "Items/Prefabs/icon_turd"
            });

            var randomItem = Items.Last();

            GameObject newObject = new GameObject();

            var item = newObject.AddComponent<Item>();
            item.id = -1;
            item.type = randomItem.Type;
            item.itemName = randomItem.Name;
            item.menuName = randomItem.MenuName;
            item.description = randomItem.Description;

            itemList.Add(newObject);

            GameObject newObject2 = new GameObject();

            randomItem = Items.First();


            var item2 = newObject2.AddComponent<Item>();
            item2.id = -1;
            item2.type = randomItem.Type;
            item2.itemName = randomItem.Name;
            item2.menuName = randomItem.MenuName;
            item2.description = randomItem.Description;

            itemList.Add(newObject2);

            DontDestroyOnLoad(newObject2);
        }

        else
        {
            // if instance is already set and this is not the same object, destroy it
            if (this != instance) { Destroy(gameObject); }
        }
    }

    // debug only
    public void Debug_PrintItemInventory()
    {
        foreach(GameObject obj in itemList)
        {
            Debug.Log(obj.GetComponent<Item>().itemName);
        }
    }

    public void AddItem(GameObject item)
    {
        itemList.Add(item);
        // Debug_PrintItemInventory(); // debug only
    }
}