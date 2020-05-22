using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using AssemblyCSharp.Assets.Scripts;
using System.Linq;
using System;
//using UnityEditor.Animations;

//public enum ControlState { PLATFORMING, BATTLE }

public class LuigiAnimEvents : MonoBehaviour, IPartyMemberBattleActions
{
    private Animator animator;

    // BattleMenu
    public GameObject battleMenu;
    public Button jumpButton;
    public Button fireballButton;
    public Button attackButton;
    public Button defendButton;
    public List<Button> itemButtons = new List<Button>();
    // Anim Controllers
    //public RuntimeAnimatorController platformingController;
    //public RuntimeAnimatorController battleController;
    // Targets
    public Transform playerSpawnPoint;
    public Transform enemySpawnPoint;
    public List<Transform> meleeTargets;
    public List<Transform> rangedTargets;
    public List<Transform> jumpTargets;
    // party
    private GameObject luigiPrefab;
    // flags, targets, etc.
    //public GameObject signal; // make visible when timed hit is active, for debug purposes
    private bool doTimedHit;
    private bool failedTimedHit;
    private bool timerOn; //used to start a timer for animation frames
    private float timer; //this is that timer
    private Transform target;
    float lerpTime;
    // JumpAttack flags
    float fallHeight;
    bool mjaUpA;
    bool mjaDownA;
    bool mjaUpB;
    bool mjaDownB;
    bool mjaStandUp;
    bool mjaJumpBack;

    private BattleManager battleManager;

    public PlayerAction PlayerAction { get; set; }
    public BattleStatus BattleStatus { get; set; }

    private bool performingItem = false;
    private string currentPerformingItem = "";

    void Start()
    {
        Application.targetFrameRate = 60;
        SetupControllerState();

        if (animator.GetInteger("intCntrlState") == 1)
        {
            InitializePrivates();
            GetBattleMenu();
            GetBattleManager();
            if (battleMenu != null) GetButtons();
            else Debug.LogError("ERROR: LuigiAnimEvents::battleMenu is empty. No buttons, no battle menu.");

            // Add listeners to buttons:
            attackButton.onClick.AddListener(call: delegate { RegisterWithBattleManager(PlayerAction.Melee); });
            jumpButton.onClick.AddListener(call: delegate { RegisterWithBattleManager(PlayerAction.Special); });
            fireballButton.onClick.AddListener(call: delegate { RegisterWithBattleManager(PlayerAction.Fireball); }); //05/21/2020 @ 23:44
            defendButton.onClick.AddListener(call: delegate { RegisterWithBattleManager(PlayerAction.Defend); });

            foreach(var button in itemButtons)
            {
                button.onClick.AddListener(call: delegate { RegisterItemWithBattleManager(button.GetComponentInChildren<TMP_Text>().text); });

            }
        }
    }

    void InitializePrivates()
    {
        // set target
        if (meleeTargets.Count != 0) target = meleeTargets[0];
        else if (rangedTargets.Count != 0) target = rangedTargets[0];
        else
        {
            target = enemySpawnPoint;
            Debug.LogError("ERROR: target initialized to \'enemySpawnPoint\'.This should not happen. There is no enemy target in any of the Target lists.");
        }
        lerpTime = 0f;
        timer = 0f;
        doTimedHit = false;
        failedTimedHit = false;
        mjaStandUp = false;
        mjaJumpBack = false;
        luigiPrefab = GameObject.FindGameObjectWithTag("Player"); // there is only one party member right now
        //signal = GameObject.Instantiate(signal, new Vector3(-2f, 1.5f, 7f), Quaternion.identity);
        //signal.SetActive(false);
    }

    void FixedUpdate()
    {
		if (animator.GetInteger("intCntrlState") == 1)
		{
			//+----------------------------------------------------------------------------+
			//|                                     JUMP                                   |
			//+----------------------------------------------------------------------------+
			if (animator.GetBool("sm_Magic_JumpAttack")) // note to self: this is a bool defined in the Editor using `SubStateMonitor.cs`. It is true as long as we are inside the substate machine. I learned a few months ago that you cannot "nest" substate machines, so don't try!
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
					LerpOverTime(target.position + Vector3.up * 10f, target.position, 0.5f);
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
				//03 jump straight up again
                if (mjaUpB == true)
                {
                    if (animator.GetBool("boolTimedHit") == true)
                    {
                        Debug.LogError("It's Spencer: We should not be here, this is not implemented yet!");
                        //jump straight up again, implementation DNE in state machine yet
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
					transform.position = ParabolicTrajectory(target.position, playerSpawnPoint.position, target.position.y, 24f / 60f);
					lerpTime += Time.deltaTime;
					if (lerpTime > 24f / 60f)
					{
						transform.position = playerSpawnPoint.position; // to ensure alignment
						lerpTime = 0f;
						mjaJumpBack = false;
                        BattleStatus = BattleStatus.Done;
                        PlayerAction = PlayerAction.None;
					}
				}
			}

            //+----------------------------------------------------------------------------+
            //|                                   FIREBALL                                 |
            //+----------------------------------------------------------------------------+

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Magic"))
            {
                float t = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                if (t >= 0.95f) // this is skipping a lot when t >= 0.99, etc.
                {
                    BattleStatus = BattleStatus.Done;
                    PlayerAction = PlayerAction.None;
                }
                //do stuff
            }

            //+----------------------------------------------------------------------------+
            //|                                     LERP                                   |
            //+----------------------------------------------------------------------------+
            if (animator.GetBool("boolRunToTarget") == true)
			{
				LerpOverTime(playerSpawnPoint.position, target.position, 0.5f);
				if (lerpTime >= 0.5f)
				{
					animator.SetBool("boolRunToTarget", false);
					lerpTime = 0f;
				}
			}

			if (animator.GetCurrentAnimatorStateInfo(0).IsName("Run_Back_Home")) //yes, I want him to run backwards, its funny
			{
                if(BattleStatus == BattleStatus.Performing)
                {
                    animator.SetBool("boolRunBackHome", true);

                    if (animator.GetBool("boolRunBackHome") == true)
                    {
                        LerpOverTime(target.position, playerSpawnPoint.position, 0.5f);
                        if (lerpTime >= 0.5f)
                        {
                            animator.SetBool("boolRunBackHome", false);
                            lerpTime = 0f;
                            BattleStatus = BattleStatus.Done;
                            PlayerAction = PlayerAction.None;
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
		}
    }

    void Update()
    {
		if (animator.GetInteger("intCntrlState") == 1)
		{
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

				if ((t >= 29f / 83f && t < 57f / 83f) && Input.GetButtonDown("Jump") && failedTimedHit == false)
				{
					Debug.Log("triggerTimedHit");
					doTimedHit = true;
				}

				if (doTimedHit == true)
				{
					animator.SetBool("boolTimedHit", true);
					doTimedHit = false;
				}

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

                if (t >= 155f / 215f)
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

                        foreach(var listItem in inventory.itemList)
                        {
                            var itemComponent = listItem.GetComponent<Item>();
                            if (itemComponent.itemName == item)
                            {
                                toRemove = listItem;
                                break;
                            }
                        }

                        inventory.itemList.Remove(toRemove);

                        newObj.transform.SetParent(playerSpawnPoint);

                        Destroy(newObj, 1.6f);

                    }
                }
            }

            //+----------------------------------------------------------------------------+
            //|                                 BATTLE IDLE                                |
            //+----------------------------------------------------------------------------+
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Battle_Idle"))
            {
                float t = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

                switch (PlayerAction)
                {
                        case PlayerAction.Item:
                        {
                            if (t > 2f && performingItem)
                            {
                                BattleStatus = BattleStatus.Done;
                                PlayerAction = PlayerAction.None;
                                performingItem = false;
                                currentPerformingItem = "";
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
                    BattleStatus = BattleStatus.Done;
                }
            }

            //else if (signal.activeInHierarchy == true) signal.SetActive(false); // in case we are already out of punch animation	
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

    private void LerpOverTime(Vector3 start, Vector3 end, float duration)
    {
        if (lerpTime <= duration)
        {
            lerpTime += Time.deltaTime;
            float percent = Mathf.Clamp01(lerpTime / duration);
            transform.position = Vector3.Lerp(start, end, percent);
        }
    }

    /*
     * This function will trace out the path of a projectile given ANY starting point,
     * ANY ending point, the max height of the projectile, and the duration of time it should
     * take the projectile to traverse the arc.
    */
    private Vector3 ParabolicTrajectory(Vector3 start, Vector3 end, float height, float duration)
    {
        Vector3 direction = new Vector3((end - start).x, 0f, (end - start).z);

        float Ys = start.y;
        float Ye = end.y;
        float Ym = height + 1f;
        float sqrtYs = Mathf.Sqrt((Ym - Ys) / (Ym - Ye)); //useful expression
        float Xval = start.x + direction.x * lerpTime / duration;
        float Zval = start.z + direction.z * lerpTime / duration;
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

    //+------------------------------------------------------------------------------+
    //|                          Turn based actions                                  |
    //+------------------------------------------------------------------------------+



    public UnityEngine.Events.UnityAction RegisterWithBattleManager(PlayerAction action)
    {

        battleManager.SubmitTurn(this, action);
        return null;
    }

    public UnityEngine.Events.UnityAction RegisterItemWithBattleManager(string itemName, string itemType = "")
    {

        battleManager.SubmitTurn(this, PlayerAction.Item, itemName);
        return null;
    }



    //+------------------------------------------------------------------------------+
    //|                          BUTTON EVENTS / "ACTIONS"                           |
    //+------------------------------------------------------------------------------+

    // JUMP ATTACK
    public UnityEngine.Events.UnityAction Special()
    {
        BattleStatus = BattleStatus.Performing;
        PlayerAction = PlayerAction.Special;

        battleMenu.SetActive(false);
        if (jumpTargets.Count == 1) target = jumpTargets[0]; // temporary, no selection function yet, just one enemy
        else target.position = Vector3.zero; // weird placement... 05/20/2020 @23:54
        AnimTrigger("triggerJump");
        mjaUpA = true;
        return null;
    }

    public UnityEngine.Events.UnityAction Fireball()
    {
        BattleStatus = BattleStatus.Performing;
        PlayerAction = PlayerAction.Fireball;

        battleMenu.SetActive(false);
        if (rangedTargets.Count != 0)
        {
            target = rangedTargets[0]; // temporary, no selection function yet, just one enemy
            AnimTrigger("triggerMagic"); // all of Luigi's Magic attacks use the same "windup" animation //05/21/2020 @23:59
        }
        else { Debug.LogError("Hey, it's Spencer...there is supposed to be a rangedTarget on the enemy, but there isn't. Make one in the prefab and call it \"rangedTarget\"!"); }
        return null;
    }

    // PHYSICAL ATTACK
    public UnityEngine.Events.UnityAction Melee()
    {
        BattleStatus = BattleStatus.Performing;
        PlayerAction = PlayerAction.Melee;

        battleMenu.SetActive(false);
        if (meleeTargets.Count == 1) target = meleeTargets[0];
        else target.position = Vector3.zero;
        AnimTrigger("triggerPunch");
        animator.SetBool("boolRunToTarget", true);
        return null;
    }

    // DEFEND
    public UnityEngine.Events.UnityAction Defend()
    {
        PlayerAction = PlayerAction.Defend;
        BattleStatus = BattleStatus.Performing;

        battleMenu.SetActive(false);
        AnimTrigger("triggerDefend");
        return null;
    }

    // ITEM
    public UnityEngine.Events.UnityAction Item(string name)
    {
        BattleStatus = BattleStatus.Performing;
        PlayerAction = PlayerAction.Item;

        currentPerformingItem = name;
        performingItem = false;

        battleMenu.SetActive(false);
        AnimTrigger("triggerConsume");
        return null;
    }

    public UnityEngine.Events.UnityAction OnBattleLoopEnd()
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

        return null;
    }

    /*
    IEnumerator ExecuteAfterTime(float time, Action task)
    {
        yield return new WaitForSeconds(time);
        task();
    }*/
    //+------------------------------------------------------------------------------+
    //|                                    SETUP                                     |
    //+------------------------------------------------------------------------------+

    void SetupControllerState()
    {
        animator = GetComponent<Animator>();
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            animator.SetInteger("intCntrlState", 0); // Platforming state
            //this.GetComponent<Animator>().runtimeAnimatorController = platformingController as RuntimeAnimatorController;
            this.GetComponent<CharacterController>().enabled = true;
            this.GetComponent<PlayerMovement>().enabled = true;
        }
        else if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            animator.SetInteger("intCntrlState", 1); // Battle state
            //this.GetComponent<Animator>().runtimeAnimatorController = battleController as RuntimeAnimatorController;
            this.GetComponent<CharacterController>().enabled = false;
            this.GetComponent<PlayerMovement>().enabled = false;
            // initialize targets:
            GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");
            foreach (GameObject obj in targets)
            {
                if (obj.name == "MeleeTarget") meleeTargets.Add(obj.transform);
                else if (obj.name == "RangedTarget") rangedTargets.Add(obj.transform);
                else if (obj.name == "JumpTarget") jumpTargets.Add(obj.transform);
                else if (obj.name == "SpawnPlayer") playerSpawnPoint = obj.transform;
                else if (obj.name == "SpawnEnemy") enemySpawnPoint = obj.transform;
                else Debug.LogError("ERROR: Unrecognized target name. Check spelling.");
            }
        }
        //animator = GetComponent<Animator>();
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

        Debug.LogError("ERROR: BattleMenu not found!");
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

        Debug.LogError("ERROR: BattleManager not found!");
    }

    void GetButtons()
    {
        jumpButton = battleMenu.transform.GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetComponent<Button>();
        fireballButton = battleMenu.transform.GetChild(0).GetChild(2).GetChild(0).GetChild(1).GetComponent<Button>();
        attackButton = battleMenu.transform.GetChild(1).GetChild(2).GetChild(0).GetChild(0).GetComponent<Button>();
        defendButton = battleMenu.transform.GetChild(2).GetChild(2).GetChild(0).GetChild(0).GetComponent<Button>();
        // handle itemButton
        if (battleMenu.transform.GetChild(3).GetChild(2).GetChild(0).childCount == 0)
        {
        }
        else
        {
            var transform = battleMenu.transform.GetChild(3).GetChild(2).GetChild(0).transform;
            List<Transform> children = transform.Cast<Transform>().ToList();

            foreach(var child in children)
            {
                var button = child.GetComponent<Button>();
                itemButtons.Add(button); 

            }

        }

    }

    // debug only
    void TestButtons()
    {
        Debug.Log(jumpButton.GetComponentInChildren<TMP_Text>().text);
        Debug.Log(fireballButton.GetComponentInChildren<TMP_Text>().text);
        Debug.Log(attackButton.GetComponentInChildren<TMP_Text>().text);
        Debug.Log(defendButton.GetComponentInChildren<TMP_Text>().text);
        if (itemButtons.Count == 0) Debug.Log("Item button");
        else Debug.LogError("ERROR: LuigiAnimationEvents::itemButton = null. No item means no item button.");
    }

    // ensures that all other triggers are off before this trigger is activated
    void AnimTrigger(string triggerName)
    {
        foreach (AnimatorControllerParameter p in animator.parameters)
            if (p.type == AnimatorControllerParameterType.Trigger)
                animator.ResetTrigger(p.name);
        animator.SetTrigger(triggerName);
    }
}