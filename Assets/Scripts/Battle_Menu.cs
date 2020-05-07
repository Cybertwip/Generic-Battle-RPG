using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Battle_Menu : MonoBehaviour
{
    public float scaleFactor = 0.75f;
    public float resolution = 100f;
    [SerializeField] GameObject buttonTemplate;
    private TMP_Text tmpText;

    // MAGIC
    // public List<SpecialAttack> magicList = new List<SpecialAttack>();
    public List<GameObject> magicList = new List<GameObject>();
    private int magicList_size;
    [SerializeField] GameObject magicContainer;
    [SerializeField] GameObject magicContainerParent;

    // ITEM
    public List<GameObject> itemList = new List<GameObject>();
    private int itemList_size;
    [SerializeField] GameObject itemContainer;
    [SerializeField] GameObject itemContainerParent;

    // ATTACK
    [SerializeField] GameObject attackContainer;
    [SerializeField] GameObject attackContainerParent;

    //DEFENSE
    [SerializeField] GameObject defenseContainer;
    [SerializeField] GameObject defenseContainerParent;

    public void Awake()
    {
        Setup(); // must query buttons for each character
    }

    public void Setup()
    {

        float multiplier = Mathf.Pow(resolution * scaleFactor,-1);
        float buttonHeight = buttonTemplate.GetComponent<RectTransform>().sizeDelta.y;
        //=================================================================
        //=================================================================

        //+==================================+
        //|$$$$$$$$$$$ MAGIC MENU $$$$$$$$$$$|
        //+==================================+

        //Populate list of all Special Attacks (magic)
        /*
        magicList.Add(new SpecialAttack("Jump", -15, -2,SpecialAttack.Types.PhysDamage));
        magicList.Add(new SpecialAttack("Fireball", -15, -2,SpecialAttack.Types.Fire));
        magicList.Add(new SpecialAttack("Super Jump", -20, -5, SpecialAttack.Types.PhysDamage));
        magicList.Add(new SpecialAttack("Super Flame", -20, -5, SpecialAttack.Types.Fire));
        magicList.Add(new SpecialAttack("Ultra Jump", -25, -8, SpecialAttack.Types.PhysDamage));
        magicList.Add(new SpecialAttack("Ultra Flame", -25, -8, SpecialAttack.Types.Fire));
        */
        foreach (GameObject specialAttack in Inventory.Instance.magicList) magicList.Add(specialAttack);
        magicList_size = magicList.Count;
        
        /* This is where it gets tricky: Need to expand the height of the
         * magic menu "container" before we instantiate more buttons.
         * Those buttons will be offset automatically thanks to Vertical Layout Group component.
         */

        // Resize Container:
        if (magicList_size != 0)
        {
            float magicContainerHeight = magicContainerParent.GetComponent<RectTransform>().sizeDelta.y;
            float magicTargetHeight = magicContainerHeight + buttonHeight * (magicList_size - 1);
            magicContainerParent.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, magicTargetHeight);
        }
        // Instantiate children of Container (Vertical Layout Group component will take care of alignment for me)
        //for (int i = 0; i < magicList_size; i++) // this will push list backwards
        for (int i = magicList_size - 1; i >=0;  i-- )
        {
            GameObject newMagicButton = Instantiate(buttonTemplate, magicContainer.transform, false);
            //newMagicButton.GetComponentInChildren<TMP_Text>().text = magicList[i].menuName;
            newMagicButton.GetComponentInChildren<TMP_Text>().text = magicList[i].GetComponent<SpecialAttack>().menuName;
        }

        //=================================================================
        //=================================================================

        //+==================================+
        //|$$$$$$$$$$ DEFENSE MENU $$$$$$$$$$|
        //+==================================+

        GameObject newDefenseButton = Instantiate(buttonTemplate, defenseContainer.transform, false);
        newDefenseButton.GetComponentInChildren<TMP_Text>().text = "Defend";

        //=================================================================
        //=================================================================

        //+===================================+
        //|$$$$$$$$$$$ ATTACK MENU $$$$$$$$$$$|
        //+===================================+

        GameObject newAttackButton = Instantiate(buttonTemplate, attackContainer.transform, false);
        newAttackButton.GetComponentInChildren<TMP_Text>().text = Inventory.Instance.activeWeapon.GetComponent<Weapon>().menuName; //onlyAttack.menuName;

        /*
        Weapon onlyAttack = new Weapon("Punch Glove", -10);
        GameObject newAttackButton = Instantiate(entry, attackContainer.transform, false);
        newAttackButton.GetComponentInChildren<TMP_Text>().text = onlyAttack.menuName;
        */


        //=================================================================
        //=================================================================

        //+=================================+
        //|$$$$$$$$$$$ ITEM MENU $$$$$$$$$$$|
        //+=================================+

        //Populate list of items (magic)
        //itemList.Add(new Item("HP Shroom","Recover 30hp."));
        //itemList.Add(new Item("Power Star","Strength + 3"));
        //itemList.Add(new Item("Chocolate Mousse","A treat you shouldn't keep to yourself."));

        foreach (GameObject item in Inventory.Instance.itemList) itemList.Add(item);
        
        itemList_size = itemList.Count;

        if (itemList_size != 0)
        {
            // Resize Container:
            float itemContainerHeight = itemContainerParent.GetComponent<RectTransform>().sizeDelta.y;
            float itemTargetHeight = itemContainerHeight + buttonHeight * (itemList_size - 1);
            itemContainerParent.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, itemTargetHeight);
            // Instantiate children of Container (Vertical Layout Group component will take care of alignment for me)
            // for (int i = 0; i < itemList_size; i++) // pushes list backwards
        }
        for (int i = itemList_size - 1; i >= 0; i--)
        {
            GameObject newItemButton = Instantiate(buttonTemplate, itemContainer.transform, false);
            newItemButton.GetComponentInChildren<TMP_Text>().text = itemList[i].GetComponent<Item>().menuName;
        }
    }
}
