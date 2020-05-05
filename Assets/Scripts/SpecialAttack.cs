using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAttack : MonoBehaviour
{
    public string specialAttackName;
    public string menuName;
    public string description;
    public int hpDamage;
    public int fpCost;
    public Types spclType;

    public enum Types
    {
        PhysDamage,
        Fire,
        Ice,
        Lightning,
        Poison,
        HpRecover,
        FpRecover,
        HpDrain,
        FpDrain,
        AttackUp,
        DefenseUp,
        MagicUp
    };

    //Not working, had to hard-code it
    /*
    private void OnAwake()
    {
        menuName = specialAttackName + " - " + Mathf.Abs(fpCost).ToString() + "FP";
    }
    */

    /*
    public SpecialAttack(string specialAttackName, int hpDamage, int fpCost, Types type)
    {
        this.specialAttackName = specialAttackName;
        this.hpDamage = hpDamage;
        this.fpCost = fpCost;
        this.spclType = type;
        this.menuName = specialAttackName + " - " + Mathf.Abs(fpCost).ToString() + "FP";
    }
    */
}

/*
public class SpecialAttack
{
    public string name;
    public int hpDamage;
    public int fpCost;
    public Types spclType;

    public enum Types
    {
        PhysDamage,
        Fire,
        Ice,
        Lightning,
        Poison,
        HpRecover,
        FpRecover,
        HpDrain,
        FpDrain,
        AttackUp,
        DefenseUp,
        MagicUp
    };

    public string menuName;

    public SpecialAttack(string name, int hpDamage, int fpCost, Types type)
    {
        this.name = name;
        this.hpDamage = hpDamage;
        this.fpCost = fpCost;
        this.spclType = type;
        this.menuName = name + " - " + Mathf.Abs(fpCost).ToString() + "FP";
    }

    
}
*/
