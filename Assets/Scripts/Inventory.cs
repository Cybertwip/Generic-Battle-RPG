using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Inventory : MonoBehaviour
{
    public class InventoryItem
    {
        public string Name { get; set; }
        public string Type { get; set; }
        //public string Path { get; set; }
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
                Type = "support",
                GetPath = "Items/Prefabs/get_mushroom",
                IconPath = "Items/Prefabs/icon_mushroom"
            });

            Items.Add(new InventoryItem()
            {
                Name = "Power Star",
                Type = "support",
                GetPath = "Items/Prefabs/get_powerStar",
                IconPath = "Items/Prefabs/icon_powerStar"
            });

            Items.Add(new InventoryItem()
            {
                Name = "Chocolate Mousse",
                Type = "offensive",
                GetPath = "Items/Prefabs/get_turd",
                IconPath = "Items/Prefabs/icon_turd"
            });

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