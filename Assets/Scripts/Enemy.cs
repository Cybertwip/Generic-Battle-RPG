using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//public abstract class Enemy : Monobehaviour
public class Enemy : MonoBehaviour
{

    public string enemyName;
    public int level;

    public int damage;

    public int maxHP;
    public int maxFP;
    public int currentHP;
    public int currentFP;

    /*
    [System.NonSerialized] public string id = "Enemy";
    [System.NonSerialized] public int hp = 100;
    [System.NonSerialized] public int fp = 100;
    [System.NonSerialized] public int strength = 10;
    [System.NonSerialized] public float defense = 1;
    [System.NonSerialized] public float magicPower = 1;
    [System.NonSerialized] public float magicDefense = 1;

    //--FX

    [System.NonSerialized] public Vector3 enemyDimensions;
    [System.NonSerialized] public float FXdiameter = 0f;
    public GameObject FXcylinderPrefab;
    [System.NonSerialized] public GameObject FXcylinder;
    [System.NonSerialized] public Vector3 FXcylinderScaleA; // for animation
    [System.NonSerialized] public Vector3 FXcylinderScaleB; // for animation
    static float t = 0f;
    [System.NonSerialized] public bool lerpFlag = false;

    public Enemy() { }
    public Enemy(string id) { }
    public Enemy(string id, int hp, int mp) { }
    public Enemy(string id, int hp, int mp, int speed) { }

    protected virtual void Awake()
    {
        // variable initialization
        enemyDimensions = GetComponent<MeshFilter>().mesh.bounds.size;
        FXdiameter = Mathf.Sqrt(Mathf.Pow(enemyDimensions.x, 2f) + Mathf.Pow(enemyDimensions.z, 2f));
        FXcylinderScaleA = new Vector3(FXdiameter, 0, FXdiameter);
        FXcylinderScaleB = new Vector3(FXdiameter, enemyDimensions.y, FXdiameter);
    }

    // AI method is run inside Update()
    public virtual void AI()
    {
        if (hp <= 0)
        {
            //play death animation
            Destroy(gameObject);
            Debug.Log(this.id + "has been killed.");
        }
    }

    public void SpawnFXcyclinder()
    {
        FXcylinder = Instantiate(FXcylinderPrefab, gameObject.transform.position, Quaternion.identity);
    }

    public void AnimateFXcylinder()
    {
        FXcylinder.transform.localScale = Vector3.Lerp(FXcylinderScaleA, FXcylinderScaleB, t);
        t += Time.deltaTime;

        if (t >= 1f && lerpFlag == true)
        {
            // termination conditions met
            Destroy(FXcylinder);
            return;
        }

        if (t >= 1f)
        {
            // swaps A and B targets so animation can reverse
            Vector3 scaleTemp = FXcylinderScaleB;
            FXcylinderScaleB = FXcylinderScaleA;
            FXcylinderScaleA = scaleTemp;

            t = 0f;
            lerpFlag = true;
        }
    }
    */
}

