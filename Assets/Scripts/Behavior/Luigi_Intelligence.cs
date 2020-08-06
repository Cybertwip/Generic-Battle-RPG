using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public interface IsPlayer
{

}

public class Luigi_Intelligence : PlayerIntelligence, IsPlayer
{

    public PlayerAction PlayerAction { get; set; }

    // jump flags
    bool mjaUpA;
    bool mjaDownA;
    bool mjaUpB;
    bool mjaDownB;
    bool mjaWait;
    bool mjaStandUp;
    bool mjaJumpBack;

    // JumpAttack flags
    float fallHeight;


    // fireball flags
    bool fba01;

    // melee attack flags

    private bool doTimedHit;
    private bool failedTimedHit;

    // timer
    private bool timerOn; //used to start a timer for animation frames

    // defending
    private bool damageReduction;

    public Transform playerSpawnPoint;
    public Transform enemySpawnPoint;
    public List<Transform> meleeTargets;
    public List<Transform> rangedTargets;
    public List<Transform> jumpTargets;


    private bool performingItem = false;
    private GameObject performingItemGO = null;
    private GameObject offensiveItem = null;

    private string currentPerformingItem = "";

    float lerpTime;

    float fireballAttackTicks;

    bool playerCastedFireball;

    public GameObject battleMenu;

    private AudioClip sfxClip;
    public AudioSource audioSource;

    public GameObject fireballPrefab;
    public Transform fireballEmitter;

    // party
    //private GameObject luigiPrefab;
    // flags, targets, etc.
    //public GameObject signal; // make visible when timed hit is active, for debug purposes
    private float timer; //this is that timer
    public Transform target;

    bool punchDamageFlag;

    PartyMember stats;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        var partyMember = GetComponent<PartyMember>();
        partyMember.maxHP = 200;
        partyMember.currentHP = 200;

        stats = partyMember;

        GetBattleMenu();
        GetBattleManager();

        Alive = true;

    }


    void GetBattleMenu()
    {
        GameObject[] taggedItems = GameObject.FindGameObjectsWithTag("BattleMenu");
        if (taggedItems.Length != 0)
        {
            foreach (GameObject obj in taggedItems)
            {
                if (obj.name == "BattleMenu")
                {
                    battleMenu = obj;
                    return;
                }
            }
        }

        //Debug.LogError("ERROR: BattleMenu not found!");
    }

    void GetBattleManager()
    {
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

        //Debug.LogError("ERROR: BattleManager not found!");
    }

    private bool dead;

    private bool submitDeathFlag;

    private float deathTicks;
    
    public override void OnDeath()
    {
        dead = true;
    }

    public override void OnRevived()
    {
        throw new System.NotImplementedException();
    }

    //+------------------------------------------------------------------------------+
    //|                          Turn based actions                                  |
    //+------------------------------------------------------------------------------+


    public void RegisterSpecialWithBattleManager(PlayerAction action, SpecialAttack.Attack attack)
    {

        battleManager.SubmitTurn(this, action, attack);
    }


    public void RegisterWithBattleManager(PlayerAction action)
    {

        battleManager.SubmitTurn(this, action);
    }

    public void RegisterItemWithBattleManager(string itemName, string itemType = "")
    {

        battleManager.SubmitTurn(this, PlayerAction.Item, itemName);
    }


    private void LerpOverTime(Vector3 start, Vector3 end, float duration)
    {
        if (lerpTime <= duration)
        {
            lerpTime += Time.deltaTime;
            float percent = Mathf.Clamp01(lerpTime / duration);
            transform.position = Vector3.Lerp(start, end, percent);
        }
    }


    //+------------------------------------------------------------------------------+
    //|                          BUTTON EVENTS / "ACTIONS"                           |
    //+------------------------------------------------------------------------------+

    // JUMP ATTACK
    public override void Special(SpecialAttack.Attack attack)
    {
        BattleStatus = BattleStatus.Performing;
        PlayerAction = PlayerAction.Special;

        battleMenu.SetActive(false);

        switch (attack)
        {
            case SpecialAttack.Attack.Jump:
                if (jumpTargets.Count == 1) target = jumpTargets[0]; // temporary, no selection function yet, just one enemy
                else target.position = Vector3.zero; // weird placement... 05/20/2020 @23:54
                AnimTrigger("triggerJump");
                mjaUpA = true;
                break;

            case SpecialAttack.Attack.Fire:
                sfxClip = Resources.Load<AudioClip>("SFX/smrpg_mario_fireball");
                if (rangedTargets.Count != 0)
                {
                    target = rangedTargets[0]; // temporary, no selection function yet, just one enemy
                    AnimTrigger("triggerMagic"); // all of Luigi's Magic attacks use the same "windup" animation //05/21/2020 @23:59
                }
                else { Debug.LogError("Hey, it's Spencer...there is supposed to be a rangedTarget on the enemy, but there isn't. Make one in the prefab and call it \"rangedTarget\"!"); }

                break;
        }

    }

    // PHYSICAL ATTACK
    public override void Melee()
    {
        BattleStatus = BattleStatus.Performing;
        PlayerAction = PlayerAction.Melee;

        battleMenu.SetActive(false);
        if (meleeTargets.Count == 1) target = meleeTargets[0];
        else target.position = Vector3.zero;
        AnimTrigger("triggerPunch");
        animator.SetBool("boolRunToTarget", true);
    }

    // DEFEND
    public override void Defend()
    {
        PlayerAction = PlayerAction.Defend;
        BattleStatus = BattleStatus.Performing;

        battleMenu.SetActive(false);
        AnimTrigger("triggerDefend");
    }

    // ITEM
    public override void Item(string name)
    {
        BattleStatus = BattleStatus.Performing;
        PlayerAction = PlayerAction.Item;

        currentPerformingItem = name;
        performingItem = false;

        var performingItemType = Inventory.Instance.GetItemTypeByName(name);
        battleMenu.SetActive(false);

        switch (performingItemType)
        {
            case global::Item.Type.Support:
                AnimTrigger("triggerConsume");
                break;
            case global::Item.Type.Offensive:
                AnimTrigger("triggerThrow");
                break;
        }
    }

    public override void OnBattleLoopEnd()
    {
        switch (PlayerAction)
        {
            case PlayerAction.Defend:
                AnimTrigger("triggerDefendEnd");

                /*
                StartCoroutine(ExecuteAfterTime(2.0f, () =>
                {
                    PlayerAction = PlayerAction.None;
                    AnimationDefendEnd();
                }));*/
                break;
        }

    }

    //+------------------------------------------------------------------------------+
    //|                          PHYSICS                                             |
    //+------------------------------------------------------------------------------+


    /*
     * This function will trace out the path of a projectile given ANY starting point,
     * ANY ending point, the max height of the projectile, and the duration of time it should
     * take the projectile to traverse the arc.
    */
    private Vector3 ParabolicTrajectory(Vector3 start, Vector3 end, float height, float lerp, float duration)
    {
        Vector3 direction = new Vector3((end - start).x, 0f, (end - start).z);

        float Ys = start.y;
        float Ye = end.y;
        float Ym = height + 1f;
        float sqrtYs = Mathf.Sqrt((Ym - Ys) / (Ym - Ye)); //useful expression
        float Xval = start.x + direction.x * lerp / duration;
        float Zval = start.z + direction.z * lerp / duration;
        float Yval = 0;

        //Debug.Log("xDir is " + Mathf.Abs(direction.x));
        //Debug.Log("zDir is " + Mathf.Abs(direction.z));
        if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.z))
        {
            float Xs = start.x;
            float Xe = end.x;
            float Xm1, Xm2;
            float Cx;

            Xm1 = (Xe * sqrtYs - Xs) / (sqrtYs - 1f);
            Xm2 = (Xe * sqrtYs + Xs) / (sqrtYs + 1f);
            if (Xm1 >= start.x && Xm1 <= end.x)
            {
                Cx = (Ym - Ye) / Mathf.Pow(Xe - Xm1, 2f);
                Yval += Ye + Ym - Cx * Mathf.Pow(Xval - Xm1, 2f);
            }
            else if (Xm2 >= end.x && Xm2 <= start.x)
            {
                Cx = (Ym - Ye) / Mathf.Pow(Xe - Xm2, 2f);
                Yval += Ye + Ym - Cx * Mathf.Pow(Xval - Xm2, 2f);
            }
            else
                Debug.LogError("ERROR! X_m is not between start.x and end.x!");
        }
        else
        {
            float Zs = start.z;
            float Ze = end.z;
            float Zm1, Zm2;
            float Cz;

            Zm1 = (Ze * sqrtYs - Zs) / (sqrtYs - 1);
            Zm2 = (Ze * sqrtYs + Zs) / (sqrtYs + 1);
            if (Zm1 >= end.z && Zm1 <= start.z)
            {
                Cz = (Ym - Ye) / Mathf.Pow(Ze - Zm1, 2f);
                Yval += Ye + Ym - Cz * Mathf.Pow(Zval - Zm1, 2f);
            }
            else if (Zm2 >= end.z && Zm2 <= start.z)
            {
                Cz = (Ym - Ye) / Mathf.Pow(Ze - Zm2, 2f);
                Yval += Ye + Ym - Cz * Mathf.Pow(Zval - Zm2, 2f);
            }
            else
                Debug.LogError("ERROR! Z_m is not between start.z and end.z!");
        }
        return new Vector3(Xval, Yval, Zval);
    }



    bool SimulateProjectile(Transform projectile, Vector3 origin, Vector3 target)
    {
        const float gravity = 9;
        const float firingAngle = 45;

        // Move projectile to the position of throwing object + add some offset if needed.
        projectile.position = origin + new Vector3(0, 0.0f, 0);

        // Calculate distance to target
        float target_Distance = Vector3.Distance(projectile.position, target);

        // Calculate the velocity needed to throw the object to the target at specified angle.
        float projectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);

        // Extract the X  Y componenent of the velocity
        float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

        // Calculate flight time.
        float flightDuration = target_Distance / Vx;

        Quaternion oldRotation = new Quaternion(projectile.rotation.x,
                                                projectile.rotation.y,
                                                projectile.rotation.z,
                                                projectile.rotation.w);

        // Rotate projectile to face the target.
        projectile.rotation = Quaternion.LookRotation(target - projectile.position);

        float elapse_time = 0;

        if (elapse_time < flightDuration)
        {
            float deltaTime = Time.deltaTime;

            projectile.Translate(0, (Vy - (gravity * elapse_time)) * deltaTime, Vx * deltaTime);

            projectile.rotation = oldRotation;

            elapse_time += deltaTime;

            return true;
        }
        else
        {
            return false;
        }
    }



    //+----------------------------------------------------------------------------+
    //|                         ANIMATIONDEFENDSTART                               |
    //+----------------------------------------------------------------------------+
    /* Puts Luigi in Defend state. Causes the Defend animation to pause on the 15th
     * frame (normalizedTime = 0.5), when the character is crouched, until the next
     * turn.
    */
    void AnimationDefendStart()
    {
        float t = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        if (t >= 0.5f) animator.speed = 0f;
    }

    //+----------------------------------------------------------------------------+
    //|                          ANIMATIONDEFENDEND                                |
    //+----------------------------------------------------------------------------+
    /* Makes Luigi finish the Defend animation (he exits his crouch). Does this by
     * changing play speed back to 1.
     * The check for running this should be if it is the player's turn and the
     * current animation state is Defend.
    */
    void AnimationDefendEnd()
    {
        animator.speed = 1f;
    }



    //+----------------------------------------------------------------------------+
    //|                          ANIMATIONDEFENDTIMED                              |
    //+----------------------------------------------------------------------------+
    /* This is for timed-Defend. Causes the Defend animation to pause for 0.25s on
     * the 15th frame (normalizedTime = 0.5), when the character is crouched.
    */
    void AnimationDefendTimed()
    {
        float t = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        if (timer == 0f && t >= 0.5f) animator.speed = 0f;
        if (t >= 0.5f && timer <= 1f) timer += 1f / 60f;
        if (timer >= 0.25f) animator.speed = 1f;
    }

    public override void TakeDamage(int amount)
    {
        var partyMember = GetComponent<PartyMember>();

        if (damageReduction)
        {
            AnimTrigger("triggerDefend");
            partyMember.currentHP -= amount / 2;
        }
        else
        {
            AnimTrigger("triggerTakeDamage");
            partyMember.currentHP -= amount;
        }
        
        if(partyMember.currentHP <= 0)
        {
            Alive = false;

            partyMember.currentHP = 0;
            animator.SetBool("boolIsDead", true); 
        }


    }

    // Update is called once per frame
    void Update()
    {

        if (dead)
        {
            deathTicks += Time.deltaTime;

            if(deathTicks >= 3)
            {
                if (!submitDeathFlag)
                {
                    submitDeathFlag = true;
                    battleManager.SubmitDeath(this);
                }
                
            }

            return;
        }

        if (DefendWindow)
        {
            if (Input.GetButtonDown("Jump"))
            {
                damageReduction = true;
            }
        }
        else
        {
            damageReduction = false;
        }


        if (animator.GetInteger("intCntrlState") == 1)
        {
            //+----------------------------------------------------------------------------+
            //|                                     JUMP                                   |
            //+----------------------------------------------------------------------------+
            if (animator.GetBool("sm_Magic_JumpAttack") && PlayerAction != PlayerAction.None) // note to self: this is a bool defined in the Editor using `SubStateMonitor.cs`. It is true as long as we are inside the substate machine. I learned a few months ago that you cannot "nest" substate machines, so don't try!
            {
                //01 jump straight up
                if (mjaUpA == true)
                {
                    LerpOverTime(playerSpawnPoint.position, playerSpawnPoint.position + Vector3.up * 10f, 0.5f);
                    if (lerpTime >= 0.5f) // move directly over target (in the plane)
                    {
                        lerpTime = 0f;
                        mjaUpA = false;
                        mjaDownA = true;
                        AnimTrigger("MJA01"); // turns this on, all other triggers off
                        transform.position = target.position + Vector3.up * 10f;
                    }
                }
                //02 land on target
                if (mjaDownA == true)
                {
                    LerpOverTime(target.position + Vector3.up * 10f, target.position, 0.5f); // falling 10 units onto enemy
                    if (lerpTime >= 0.5f)
                    {
                        animator.speed = 0.5f;
                        AnimTrigger("MJA02");
                        lerpTime = 0f;
                        mjaDownA = false;
                        mjaWait = true;

                        target.parent.GetComponent<AI_Behavior>().OnDamageReceived(25);

                    }
                }
                //02.5 wait (better would be to stretch landing animation to 0.25 seconds)
                if (mjaWait == true)
                {
                    lerpTime += Time.deltaTime;
                    if (lerpTime >= 0.25f)
                    {
                        mjaWait = false;
                        mjaUpB = true; // not used yet, only exists in two lines in this script, intention was for use with timed jumps
                        animator.speed = 1f;
                        AnimTrigger("triggerTimedHitFailed"); // TEMPORARY, puts us in sm_Jump_Finish
                    }
                }
                //02 land on target
                /*
                if (mjaDownA == true)
                {
                    LerpOverTime(target.position + Vector3.up * 10f, target.position, 0.5f); // falling 10 units onto enemy
                    // (play falling sound)
                    if (lerpTime >= 0.366f) { AnimTrigger("MJA02"); }
                    if (lerpTime >= 0.5f)
                    {
                        lerpTime = 0f;
                        mjaDownA = false;
                        mjaUpB = true; // not used yet, only exists in two lines in this script, intention was for use with timed jumps
                        animator.SetBool("boolTimedHit", false); // TEMPORARY, puts us in sm_Jump_Finish
                    }
                }
                */
                //03 jump straight up again, not implemented yet
                if (mjaUpB == true)
                {
                    if (animator.GetBool("boolTimedHit") == true) // in the tree, not implemented
                    {
                        Debug.LogError("It's Spencer: We should not be here, this is not implemented yet!");
                        //jump straight up again, state variables DNE in state machine yet
                    }
                    else
                    {
                        mjaUpB = false;
                        mjaStandUp = true;
                    }
                }

                if (mjaStandUp == true)
                {
                    if (lerpTime < 12f / 30f) lerpTime += Time.deltaTime; // duration of Luigi's "Jump_start" animation
                    else
                    {
                        mjaStandUp = false;
                        mjaJumpBack = true;
                        lerpTime = 0f;
                    }
                }

                if (mjaJumpBack == true && lerpTime <= 24f / 60f)
                {
                    //LerpOverTime(target.position, playerSpawnPoint.position, 24f / 60f);
                    transform.position = ParabolicTrajectory(target.position, playerSpawnPoint.position, target.position.y, lerpTime, 24f / 60f);
                    lerpTime += Time.deltaTime;
                    if (lerpTime > 24f / 60f)
                    {
                        transform.position = playerSpawnPoint.position; // to ensure alignment
                        lerpTime = 0f;
                        mjaJumpBack = false;
                        punchDamageFlag = false;
                        BattleStatus = BattleStatus.Done;
                        PlayerAction = PlayerAction.None;
                    }
                }
            }

            //+----------------------------------------------------------------------------+
            //|                                   FIREBALL                                 |
            //+----------------------------------------------------------------------------+

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Magic") && PlayerAction != PlayerAction.None)
            {
                float t = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

                if ((t >= 84f / 120f && t <= 99f / 120f) && fba01 == false)
                {
                    fba01 = true;
                    animator.speed = 0f; // pause "magic" animation
                    lerpTime = 0f; //resetting, just in case
                    fireballAttackTicks = 0;
                }

                if (animator.speed == 0f)
                {
                    fireballAttackTicks += Time.deltaTime;

                    if (lerpTime < 6f)
                    {

                        if (Input.GetButtonDown("Jump"))
                        {
                            playerCastedFireball = true;
                        }


                        // TODO: launch fireball
                        if ((fireballAttackTicks > (1 / 60f) * 10) && playerCastedFireball)
                        {
                            playerCastedFireball = false;
                            fireballAttackTicks = 0;
                            GameObject fireballGO = Instantiate(fireballPrefab, this.transform);

                            var playerPosition = this.transform.position;

                            fireballGO.transform.position = new Vector3(playerPosition.x, playerPosition.y + 1, playerPosition.z + 1);
                            fireballGO.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 5);

                            audioSource.PlayOneShot(sfxClip); // this is only for demo purposes
                        }
                    }
                    else if (lerpTime >= 7f)
                    {
                        animator.speed = 1f; // continue "magic" animation where it left off
                    }
                    lerpTime += Time.deltaTime;
                }

                if (t >= 0.95f) // this is skipping a lot when t >= 0.99, etc.
                {
                    target.parent.GetComponent<AI_Behavior>().OnDamageReceived(25);

                    punchDamageFlag = false;
                    BattleStatus = BattleStatus.Done;
                    PlayerAction = PlayerAction.None;
                    lerpTime = 0f;
                    // reset action vars:
                    fba01 = false;
                }
                //do stuff
            }

            //+----------------------------------------------------------------------------+
            //|                                     LERP                                   |
            //+----------------------------------------------------------------------------+
            if (animator.GetBool("boolRunToTarget") == true)
            {
                LerpOverTime(playerSpawnPoint.position, target.position, 0.375f);
                if (lerpTime >= 0.375f)
                {
                    animator.SetBool("boolRunToTarget", false);
                    if (PlayerAction.ToString() == "Melee") AnimTrigger("triggerFirstPunch");
                    lerpTime = 0f;
                }
            }

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Run_Back_Home")) //yes, I want him to run backwards, its funny
            {
                if (BattleStatus == BattleStatus.Performing)
                {
                    animator.SetBool("boolRunBackHome", true);

                    if (animator.GetBool("boolRunBackHome") == true)
                    {
                        LerpOverTime(target.position, playerSpawnPoint.position, 0.375f);
                        if (lerpTime >= 0.375f)
                        {
                            animator.SetBool("boolRunBackHome", false);
                            lerpTime = 0f;
                            BattleStatus = BattleStatus.Done;
                            PlayerAction = PlayerAction.None;

                            punchDamageFlag = false;

                            // reset timed-hit anim parameter (there is a bug in the melee action where timed-hit always succeeds if we don't):
                            animator.SetBool("boolTimedHit", false);
                        }

                    }

                }

            }

            // reset flags when action sequence is over
            if ((failedTimedHit == true || doTimedHit == true) && animator.GetCurrentAnimatorStateInfo(0).IsName("Battle_Idle"))
            {
                failedTimedHit = false;
                doTimedHit = false;
            }

            /*
			if (animator.GetCurrentAnimatorStateInfo(0).IsName("Defend"))
			{
				// DONT FORGET TO PUT THE EXIT ANIMATION IN LATER!
                if(BattleStatus  == BattleStatus.Performing)
                {
                    AnimationDefendStart();
                }
            }*/

            //+----------------------------------------------------------------------------+
            //|                               PHYSICAL ATTACK                              |
            //+----------------------------------------------------------------------------+
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("First_Punch"))
            {
                float t = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                // handle player's timed hit input (if there is any)

                // prevents player from succeeding by spamming buttons:
                if (t < 29f / 83f && Input.GetButtonDown("Jump"))
                    failedTimedHit = true;

                if((t >= 40f / 83f && t < 57f / 83f))
                {
                    if (!punchDamageFlag)
                    {
                        punchDamageFlag = true;
                        target.parent.GetComponent<AI_Behavior>().OnDamageReceived(10);

                    }
                }

                if ((t >= 29f / 83f && t < 57f / 83f) && Input.GetButtonDown("Jump") && failedTimedHit == false)
                {
                    Debug.Log("triggerTimedHit");
                    AnimTrigger("triggerTimedHit");
                    target.parent.GetComponent<AI_Behavior>().OnDamageReceived(15);

                    //doTimedHit = true;
                }

                /*
                if (doTimedHit == true)
                {
                    animator.SetBool("boolTimedHit", true);
                    doTimedHit = false;
                }
                */

                // deal with timed-hit signal: (all signal debug lines work)
                //if (t >= 29f / 83f && t < 57f / 83f && signal.activeInHierarchy == false) signal.SetActive(true);
                //if (t >= 57f / 83f && signal.activeInHierarchy == true) signal.SetActive(false);
            }

            //+----------------------------------------------------------------------------+
            //|                                CONSUME ITEM                                |
            //+----------------------------------------------------------------------------+
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("ConsumeItem"))
            {
                float t = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

                if (t >= 140f / 216f) // t >= 155f/ 215f
                {
                    if (!performingItem)
                    {
                        performingItem = true;

                        var inventory = Inventory.Instance;

                        var item = inventory.Items.Where(i => i.Name == currentPerformingItem).First().GetPath;

                        GameObject prefab = Resources.Load(item) as GameObject;

                        GameObject newObj =
                            Instantiate(prefab,
                                        playerSpawnPoint.position + new Vector3(0, 2.75f, 0.5f),
                                        Camera.main.transform.rotation);

                        GameObject toRemove = null;

                        foreach (var listItem in inventory.itemList)
                        {
                            var itemComponent = listItem.GetComponent<Item>();
                            if (itemComponent.itemName == currentPerformingItem)
                            {
                                toRemove = listItem;
                                break;
                            }
                        }

                        inventory.itemList.Remove(toRemove);

                        var itemStats = toRemove.GetComponent<Item>();

                        stats.currentHP += itemStats.dHP;
                        stats.currentFP += itemStats.dFP;
                        stats.strenght += itemStats.dStrength;
                        stats.defense += itemStats.dDefense;
                        stats.magicPower += itemStats.dMagicPower;
                        stats.magicDefense += itemStats.dMagicDefense;
                        stats.speed += itemStats.dSpeed;
                        
                        if(stats.currentHP > stats.maxHP)
                        {
                            stats.currentHP = stats.maxHP;
                        }

                        if(stats.currentFP > stats.maxFP)
                        {
                            stats.currentFP = stats.maxFP;
                        }

                        newObj.transform.SetParent(playerSpawnPoint);

                        Destroy(newObj, 1.6f);

                    }
                }
            }

            //+----------------------------------------------------------------------------+
            //|                                THROW ITEM                                  |
            //+----------------------------------------------------------------------------+
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("ThrowItem"))
            {
                float t = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

                if (t >= 54f / 90f)
                {
                    if (!performingItem)
                    {
                        performingItem = true;

                        var inventory = Inventory.Instance;

                        var item = inventory.Items.Where(i => i.Name == currentPerformingItem).First().IconPath;

                        GameObject prefab = Resources.Load(item) as GameObject;

                        GameObject newObj =
                            Instantiate(prefab,
                                        playerSpawnPoint.position + new Vector3(0, 1.0f, 1.5f),
                                        Camera.main.transform.rotation);

                        GameObject toRemove = null;

                        foreach (var listItem in inventory.itemList)
                        {
                            var itemComponent = listItem.GetComponent<Item>();
                            if (itemComponent.itemName == currentPerformingItem)
                            {
                                toRemove = listItem;
                                break;
                            }
                        }

                        inventory.itemList.Remove(toRemove);

                        //newObj.transform.SetParent(playerSpawnPoint);

                        lerpTime = 0;

                        performingItemGO = newObj;
                        offensiveItem = toRemove;
                    }
                }
            }

            //+----------------------------------------------------------------------------+
            //|                                 THROW ITEM                                 |
            //+----------------------------------------------------------------------------+

            if (performingItem)
            {
                var itemType = Inventory.Instance.GetItemTypeByName(currentPerformingItem);

                if (itemType == global::Item.Type.Offensive)
                {

                    if (performingItemGO != null && !ReferenceEquals(performingItemGO, null))
                    {

                        target = rangedTargets[0]; // temporary, no selection function yet, just one enemy


                        var newTarget = new Vector3();
                        newTarget.x = target.position.x;
                        newTarget.y = target.position.y;
                        newTarget.z = target.position.z;

                        SimulateProjectile(performingItemGO.transform,
                                           performingItemGO.transform.position,
                                           newTarget);

                        //performingItemGO.transform.position = ParabolicTrajectory(playerSpawnPoint.position, target.position, target.position.y + 4, lerpTime, 2f);


                    }
                    else
                    {

                        var itemStats = offensiveItem.GetComponent<Item>();

                        var enemyStats = target.parent.GetComponent<Enemy>();

                        enemyStats.currentHP += itemStats.dHP;
                        enemyStats.currentFP += itemStats.dFP;
                        enemyStats.strenght += itemStats.dStrength;
                        enemyStats.defense += itemStats.dDefense;
                        enemyStats.magicPower += itemStats.dMagicPower;
                        enemyStats.magicDefense += itemStats.dMagicDefense;
                        enemyStats.speed += itemStats.dSpeed;

                        performingItemGO = null;
                        lerpTime = 0;

                        performingItem = false;

                        punchDamageFlag = false;

                        BattleStatus = BattleStatus.Done;
                        PlayerAction = PlayerAction.None;

                    }

                }
            }

            //+----------------------------------------------------------------------------+
            //|                                 BATTLE IDLE                                |
            //+----------------------------------------------------------------------------+
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Battle_Idle"))
            {
                float t = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

                switch (PlayerAction)
                {
                    case PlayerAction.Item:
                        {
                            if (performingItem && t > 2 && (performingItemGO == null || ReferenceEquals(performingItemGO, null)))
                            {
                                var itemType = Inventory.Instance.GetItemTypeByName(currentPerformingItem);

                                if (itemType == global::Item.Type.Support)
                                {
                                    punchDamageFlag = false; 
                                    BattleStatus = BattleStatus.Done;
                                    PlayerAction = PlayerAction.None;
                                    performingItem = false;
                                    currentPerformingItem = "";
                                }

                            }
                        }
                        break;

                }

            }
            //+----------------------------------------------------------------------------+
            //|                                   DEFEND                                   |
            //+----------------------------------------------------------------------------+
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Defend"))
            {
                float t = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

                if (t >= 1f) //(t >= 30f / 30f) changed 05/21/2020 @ 21:58
                {
                    AnimTrigger("triggerDefendEnd");
                    punchDamageFlag = false;
                    BattleStatus = BattleStatus.Done;
                }
            }

            //else if (signal.activeInHierarchy == true) signal.SetActive(false); // in case we are already out of punch animation	
        }
    }

    void AnimTrigger(string triggerName)
    {
        foreach (AnimatorControllerParameter p in animator.parameters)
            if (p.type == AnimatorControllerParameterType.Trigger)
                animator.ResetTrigger(p.name);
        animator.SetTrigger(triggerName);
        //Debug.Log("triggered " + triggerName);
    }
}
