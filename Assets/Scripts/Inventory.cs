using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private static Inventory instance = null; // static (class level) variable
    public static Inventory Instance { get { return instance; } } // static getter (only accessing allowed)

    [SerializeField]
    private GameObject[] items = new GameObject[3];

    public List<GameObject> itemList;
    public List<GameObject> magicList;
    public List<GameObject> weaponList;
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