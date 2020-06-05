using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;
using UnityEditor.Build.Content;
//using UnityEditor.Animations;

//public enum ControlState { PLATFORMING, BATTLE }

public class LuigiAnimEvents : MonoBehaviour
{
    private Animator animator;

    // BattleMenu
    public GameObject battleMenu;
    public Button jumpButton;
    public Button fireballButton;
    public Button attackButton;
    public Button defendButton;
    public List<Button> itemButtons = new List<Button>();

    public Transform playerSpawnPoint;
    public Transform enemySpawnPoint;
    public List<Transform> meleeTargets;
    public List<Transform> rangedTargets;
    public List<Transform> jumpTargets;
    private Transform target;
    

    Luigi_Intelligence intelligence;

    void Start()
    {
        Application.targetFrameRate = 60;
        SetupControllerState();


        if (animator.GetInteger("intCntrlState") == 1)
        {

            InitializePrivates();

            GetBattleMenu();

            ///////////
            /// BEGIN PILE OF CRAP
            /// 

            if (this.gameObject.GetComponent<Luigi_Intelligence>() == null)
            {
                intelligence = this.gameObject.AddComponent<Luigi_Intelligence>();

            }
            else
            {
                intelligence = this.gameObject.GetComponent<Luigi_Intelligence>();
            }

            intelligence.meleeTargets = meleeTargets;
            intelligence.rangedTargets = rangedTargets;
            intelligence.jumpTargets = jumpTargets;
            intelligence.playerSpawnPoint = playerSpawnPoint;
            intelligence.enemySpawnPoint = enemySpawnPoint;
            intelligence.target = target;

            intelligence.audioSource = GetComponent<AudioSource>();

            /// END PILE OF CRAP
            /// 
            ///////////


            if (battleMenu != null) GetButtons();
            else Debug.LogError("ERROR: LuigiAnimEvents::battleMenu is empty. No buttons, no battle menu.");

            // Add listeners to buttons:
            attackButton.onClick.AddListener(call: delegate { intelligence.RegisterWithBattleManager(PlayerAction.Melee); });
            jumpButton.onClick.AddListener(call: delegate { intelligence.RegisterSpecialWithBattleManager(PlayerAction.Special, SpecialAttack.Attack.Jump); });
            fireballButton.onClick.AddListener(call: delegate { intelligence.RegisterSpecialWithBattleManager(PlayerAction.Special, SpecialAttack.Attack.Fire); }); //05/21/2020 @ 23:44
            defendButton.onClick.AddListener(call: delegate { intelligence.RegisterWithBattleManager(PlayerAction.Defend); });

            foreach (var button in itemButtons)
            {
                button.onClick.AddListener(call: delegate { intelligence.RegisterItemWithBattleManager(button.GetComponentInChildren<TMP_Text>().text); });

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
        //luigiPrefab = GameObject.FindGameObjectWithTag("Player"); // there is only one party member right now
        //signal = GameObject.Instantiate(signal, new Vector3(-2f, 1.5f, 7f), Quaternion.identity);
        //signal.SetActive(false);
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

    void Update()
    {
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
                if (obj.name == "MeleeTarget")
                {
                    meleeTargets.Add(obj.transform);
                }
                else if (obj.name == "RangedTarget")
                {
                    rangedTargets.Add(obj.transform);
                }
                else if (obj.name == "JumpTarget")
                {
                    jumpTargets.Add(obj.transform);
                }
                else if (obj.name == "SpawnPlayer")
                {
                    playerSpawnPoint = obj.transform;
                }
                else if (obj.name == "SpawnEnemy")
                {
                    enemySpawnPoint = obj.transform;
                }
                else Debug.LogError("ERROR: Unrecognized target name. Check spelling.");
            }
        }
        //animator = GetComponent<Animator>();
    }



    void GetButtons()
    {
        jumpButton = battleMenu.transform.GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetComponent<Button>();
        fireballButton = battleMenu.transform.GetChild(0).GetChild(2).GetChild(0).GetChild(1).GetComponent<Button>();
        attackButton = battleMenu.transform.GetChild(1).GetChild(2).GetChild(0).GetChild(0).GetComponent<Button>();
        defendButton = battleMenu.transform.GetChild(2).GetChild(2).GetChild(0).GetChild(0).GetComponent<Button>();
        // handle itemButton
        if (battleMenu.transform.GetChild(3).GetChild(2).GetChild(0).childCount != 0)
        {
            var transform = battleMenu.transform.GetChild(3).GetChild(2).GetChild(0).transform;
            List<Transform> children = transform.Cast<Transform>().ToList();

            foreach (var child in children)
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
        //Debug.Log("triggered " + triggerName);
    }
}