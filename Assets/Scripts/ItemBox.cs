using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    bool disabled = false;
    private Animator anim;
    [SerializeField]
    private GameObject[] items = new GameObject[3];
    [SerializeField] private GameObject gm;

    void Start()
    {
        anim = GetComponent<Animator>();
        if (!disabled)
        {
            anim.Play("ItemBoxIdleState");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!disabled && other.gameObject.tag == "Player")
        {
            anim.Play("ItemBoxGetItemState");
            int randInt = Random.Range(0, 3);
            randInt = 1;
            var inventory = Inventory.Instance;

            var randomItem = inventory.Items[randInt].GetPath;

            GameObject prefab = Resources.Load(randomItem) as GameObject;
           
            GameObject itemObj =
                Instantiate(prefab,
                            transform.position + new Vector3(0, 0.375f, 0),
                            Camera.main.transform.rotation);

            GameObject newObj = Instantiate(items[randInt]);
            /*GameObject itemObj = Instantiate(items[randInt].GetComponent<Item>().getPrefab,
                transform.position + new Vector3(0, 0.375f, 0), Camera.main.transform.rotation);
            GameObject newObj = Instantiate(items[randInt]);*/
            gm.GetComponent<Inventory>().itemList.Add(newObj);//AddItem(newObj);
            newObj.transform.SetParent(gm.transform.GetChild(0));
            Destroy(itemObj, 1.6f);
            disabled = true;
        }
    }
}
