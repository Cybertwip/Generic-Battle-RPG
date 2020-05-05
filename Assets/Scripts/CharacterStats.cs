using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    // current stats
    [System.NonSerialized] public string id = "Luigi";
    [System.NonSerialized] public int hp;
    [System.NonSerialized] public int fp;
    [System.NonSerialized] public int strength;
    [System.NonSerialized] public float defense;
    [System.NonSerialized] public float magicPower;
    [System.NonSerialized] public float magicDefense;

    // base stats
    readonly public int baseHp = 100;
    readonly public int baseFp = 30;
    readonly public int baseStrength = 10;
    readonly public float baseDefense = 1;
    readonly public float baseMagicPower = 1;
    readonly public float baseMagicDefense = 1;

    private static CharacterStats instance = null; // static (class level) variable
    public static CharacterStats Instance { get { return instance; } } // static getter (only accessing allowed)

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

        // initialize current stats:
        hp = baseHp;
        fp = baseFp;
        strength = baseStrength;
        defense = baseDefense;
        magicPower = baseMagicPower;
        magicDefense = baseMagicDefense;
    }
}
