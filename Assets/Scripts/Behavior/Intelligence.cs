using UnityEngine;
using System.Collections;

public abstract class Intelligence : PartyMemberBattleActions
{
    protected Animator animator;
    protected BattleManager battleManager;

    // Use this for initialization
    protected virtual void Start()
    {
        animator = gameObject.GetComponent<Animator>();

        GameObject[] taggedItems = GameObject.FindGameObjectsWithTag("BattleManager");
        if (taggedItems.Length != 0)
        {
            foreach (GameObject obj in taggedItems)
            {
                if (obj.name == "BattleManager")
                {
                    battleManager = obj.GetComponent<BattleManager>();
                    return;
                }
            }
        }


    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }
}
