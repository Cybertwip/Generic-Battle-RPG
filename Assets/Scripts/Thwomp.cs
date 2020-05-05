using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOTE: Not using this script right now

public class Thwomp : Enemy
{
    private void Start()
    {
        enemyName = "Thwomp";
        level = 10;

        damage = 30;

        maxHP = 200;
        currentHP = 200;
    }

    
    /*
    protected override void Awake()
    {
        base.Awake();
        // variable initialization
        id = "Thwomp";
        hp = 500;
        fp = 20;
        strength = 20;
        defense = 2;
        magicPower = 1;
        magicDefense = 1;
        
        //enemyDimensions = GetComponent<MeshFilter>().mesh.bounds.size;
        //FXdiameter = Mathf.Sqrt(Mathf.Pow(enemyDimensions.x, 2f) + Mathf.Pow(enemyDimensions.z, 2f));
        //FXcylinderScaleA = new Vector3(FXdiameter, 0, FXdiameter);
        //FXcylinderScaleB = new Vector3(FXdiameter, enemyDimensions.y, FXdiameter);
    }
    */

    /*
    private void Start()
    {
        //SpawnFXcyclinder();
    }
    */


    /*
    void Update()
    {
        //if (FXcylinder)
        //{
        //    AnimateFXcylinder();
        //}

    }
    */

}
