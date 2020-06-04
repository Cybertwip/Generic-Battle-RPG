using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Item : MonoBehaviour
{
    public enum Type
    {
        Support = 0,
        Offensive
    }

    public int id;
    public Type type; // either "support" or "offensive", for now
    public string itemName;
    public string menuName; // how name is displayed in menus
    public string description;

    public Sprite avatar;
    public GameObject iconPrefab;
    public GameObject getPrefab; // prefab associated with "ItemGet" animation
    // dHP, dFP, dStrength, dDefense, dMagicAttack, dMagicDefense
    // public int[] attributes = new int[6];

    public int dHP;
    public int dFP;
    public int dStrength;
    public float dDefense;
    public float dMagicPower;
    public float dMagicDefense;

    public bool enraged = false;


    //============================

    public Item() { }

    public Item(int id, string itemName, string menuName,
        string description, Type type = Type.Support)
    {
        this.id = id;
        this.type = type;
        this.itemName = itemName;
        this.menuName = menuName;
        this.description = description;

        //this.iconPrefab = Resources.Load<GameObject>("Items/Sprites/" + this.id + ".png");
        //this.getPrefab = Resources.Load<GameObject>("Items/" + this.id + "Get.png");
    }

    public Item(Item item)
    {
        this.id = item.id;
        this.type = item.type;
        this.itemName = item.itemName;
        this.menuName = item.menuName;
        this.description = item.description;

        //this.iconPrefab = Resources.Load<GameObject>("Items/Sprites/" + item.id + ".png");
        //this.getPrefab = Resources.Load<GameObject>("Items/" + item.id + "Get.png");
        //this.icon = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Items/Sprites/" + item.id + ".png", typeof(Sprite));
        //this.graphicsPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Items/Prefabs/" + item.id + "Get.png", typeof(GameObject));
    }
}


/*
public class Item
{
    public int id;
    public string itemName;
    public string menuName; // how name is displayed in menus
    public string description;
    public Sprite icon;
    public GameObject graphicsPrefab;
    public Dictionary<string, int> stats = new Dictionary<string, int>();

    public Item() { }

    public Item(int id, string itemName, string menuName,
        string description)
    {
        this.id = id;
        this.itemName = itemName;
        this.menuName = menuName;
        this.description = description;

        this.icon = Resources.Load<Sprite>("Items/Sprites/" + this.id + ".png");
        this.graphicsPrefab = Resources.Load<GameObject>("Items/" + this.id + "Get.png");
        //this.icon = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Items/Sprites/" + this.id + ".png", typeof(Sprite));
        //this.graphicsPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Items/Prefabs/" + this.id + "Get.png", typeof(GameObject));
    }

    public Item(Item item)
    {
        this.id = item.id;
        this.itemName = item.itemName;
        this.menuName = item.menuName;
        this.description = item.description;

        this.icon = Resources.Load<Sprite>("Items/Sprites/" + item.id + ".png");
        this.graphicsPrefab = Resources.Load<GameObject>("Items/" + item.id + "Get.png");
        //this.icon = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Items/Sprites/" + item.id + ".png", typeof(Sprite));
        //this.graphicsPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Items/Prefabs/" + item.id + "Get.png", typeof(GameObject));
    }

*/

    //public string[] attributes = new string[3];


    /*
    public Dictionary<string, int> attributes =
        new Dictionary<string, int>()
        {
            { "dHP", 0 },
            {"dFP", 0 },
            {"dStrength", 0 },
            {"dMagic", 0 },
            {"dLuck", 0 }
        };
    */
//}
//Original script
/*
public class Item
{
    public string _name;
    public string _menuName;
    public string _description;
    //public GameObject _itemPrefab;
   // Animation _anim;

    public Item(string name, string description)//, GameObject itemPrefab
    {
        this._name = name;
        this._menuName = name;
        this._description = description;
        //this._itemPrefab = itemPrefab;
        //this._anim = itemPrefab.GetComponent<Animation>();
    }
}
*/