using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Linq;

public enum PlayerAction { Melee, Special, Item, Defend, None } //@TODO move to self contained enum class //appended `Fireball` 05/21/2020 @ 23:36
public enum BattleState { START, PLAYERTURNSUBMIT, ENEMYTURNSUBMIT, SUBMISSION, BATTLE, WON, LOST }

public class BattleManager : MonoBehaviour
{
    private const int PLAYER_PARTY_MEMBERS_SIZE = 1;
    private const int ENEMY_PARTY_MEMBERS_SIZE = 1;

    private Dictionary<PartyMemberBattleActions, PlayerAction> TurnKeyMap = new Dictionary<PartyMemberBattleActions, PlayerAction>();
    private Dictionary<PartyMemberBattleActions, Stats> StatsKeyMap = new Dictionary<PartyMemberBattleActions, Stats>();
    private List<PartyMemberBattleActions> TurnList = new List<PartyMemberBattleActions>();

    private List<PartyMemberBattleActions> FinishedTurnList = new List<PartyMemberBattleActions>();


    private Dictionary<PartyMemberBattleActions, GameObject> PlayerParty = new Dictionary<PartyMemberBattleActions, GameObject>();
    private Dictionary<PartyMemberBattleActions, GameObject> EnemyParty = new Dictionary<PartyMemberBattleActions, GameObject>();

    private Dictionary<PartyMemberBattleActions, GameObject> GroupParties = new Dictionary<PartyMemberBattleActions, GameObject>();



    private Dictionary<PartyMemberBattleActions, string> TurnItemKeyMap = new Dictionary<PartyMemberBattleActions, string>();
    private Dictionary<PartyMemberBattleActions, SpecialAttack.Attack> SpecialKeyMap = new Dictionary<PartyMemberBattleActions, SpecialAttack.Attack>();

    PartyMemberBattleActions currentPerformingCharacter;

    public void SubmitTurn(PartyMemberBattleActions member, PlayerAction action)
    {
        TurnKeyMap[member] = action;
        TurnList.Add(member);

        StatsKeyMap[member] = GroupParties[member].GetComponent<Stats>();
    }

    public void SubmitTurn(PartyMemberBattleActions member, PlayerAction action, SpecialAttack.Attack attack)
    {
        TurnKeyMap[member] = action;
        TurnList.Add(member);
        SpecialKeyMap[member] = attack;

        StatsKeyMap[member] = GroupParties[member].GetComponent<Stats>();
    }

    public void SubmitTurn(PartyMemberBattleActions member, PlayerAction action, string parameter)
    {
        TurnKeyMap[member] = action;
        TurnList.Add(member);

        TurnItemKeyMap[member] = parameter;

        StatsKeyMap[member] = GroupParties[member].GetComponent<Stats>();
    }
    public KeyValuePair<PartyMemberBattleActions, PlayerAction> PerformTurn()
    {
        var lastCharacter = TurnList.First();
        switch (TurnKeyMap[lastCharacter])
        {
            case PlayerAction.Defend:
                lastCharacter.Defend();
                break;

            case PlayerAction.Item:

                lastCharacter.Item(TurnItemKeyMap[lastCharacter]);

                break;

            case PlayerAction.Melee:
                lastCharacter.Melee();
                break;


            case PlayerAction.Special:
                lastCharacter.Special(SpecialKeyMap[lastCharacter]);
                break;
        }

        currentPerformingCharacter = lastCharacter;

        TurnList.Remove(currentPerformingCharacter);
        FinishedTurnList.Add(currentPerformingCharacter);

        return new KeyValuePair<PartyMemberBattleActions,
                                PlayerAction>(currentPerformingCharacter,
                                              TurnKeyMap[lastCharacter]);
    }

    // BattleMenu
    public GameObject battleMenu;
    public List<Button> magicButtons;
    public Button attackButton;
    public Button defendButton;
    public List<Button> itemButtons;

    // Character prefabs
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerSpawnPoint;
    public Transform enemySpawnPoint;

    Enemy enemyParams;
    PartyMember playerParams;

    //+------------------------+
    //|          HUD           |
    //+------------------------+
    public Text pm0currentHP;
    public Text pm0maxHP;

    public BattleState state;

    void Start()
    {
        PlayerParty.Clear();
        EnemyParty.Clear();
        GroupParties.Clear();

        state = BattleState.START;
        StartCoroutine(SetupBattle());

        // initialize BattleMenu buttons:
        attackButton = battleMenu.transform.GetChild(1).GetChild(2).GetChild(0).GetChild(0).GetComponent<Button>();
        defendButton = battleMenu.transform.GetChild(2).GetChild(2).GetChild(0).GetChild(0).GetComponent<Button>();

        // get all the magic buttons:
        Transform magicContainer = battleMenu.transform.GetChild(0).GetChild(2).GetChild(0);
        int numMagicChildren = magicContainer.transform.childCount;
        for (int i = 0; i < numMagicChildren; i++)
        {
            magicButtons.Add(magicContainer.GetChild(i).GetComponent<Button>());
            //Debug.Log(i + "th magic button is " + magicButtons[i].GetComponentInChildren<TMP_Text>().text);
            //yes, this worked!
        }

        // get all the item buttons:
        Transform itemContainer = battleMenu.transform.GetChild(3).GetChild(2).GetChild(0);
        int numItemChildren = itemContainer.transform.childCount;
        for (int i = 0; i < numItemChildren; i++)
        {
            itemButtons.Add(itemContainer.GetChild(i).GetComponent<Button>());
            //Debug.Log(i + "th item button is " + itemButtons[i].GetComponentInChildren<TMP_Text>().text);
            //yes, this worked!
        }
    }

    IEnumerator SetupBattle()
    {
        // Set the character models:
        playerPrefab.transform.position = Vector3.zero;
        GameObject playerGO = Instantiate(playerPrefab, playerSpawnPoint);
        playerParams = playerGO.GetComponent<PartyMember>();

        enemyPrefab.transform.rotation = Quaternion.AngleAxis(180,Vector3.up);
        GameObject enemyGO = Instantiate(enemyPrefab, enemySpawnPoint);
        enemyParams = enemyGO.GetComponent<Enemy>();


        PlayerParty.Add(playerGO.GetComponent<Intelligence>(), playerGO);
        EnemyParty.Add(enemyGO.GetComponent<Intelligence>(), enemyGO);

        GroupParties.Add(playerGO.GetComponent<Intelligence>(), playerGO);
        GroupParties.Add(enemyGO.GetComponent<Intelligence>(), enemyGO);

        // Set the HUD
        SetHudHP();
        pm0maxHP.text = playerParams.maxHP.ToString();

        yield return new WaitForSeconds(1.0f);

        PlayerSubmitTurn();

    }

    void InitPlayerBattleStatuses()
    {
        var playerGO = playerParams.gameObject;

        var playerBattleState = playerGO.GetComponent<Intelligence>() as PartyMemberBattleActions;

        playerBattleState.BattleStatus = BattleStatus.Idle;
        currentPerformingCharacter = null;

    }

    void InitEnemyBattleStatuses()
    {
        var enemyGo = enemyParams.gameObject;

        var enemyBattleState = enemyGo.GetComponent<Intelligence>() as PartyMemberBattleActions;

        enemyBattleState.BattleStatus = BattleStatus.Idle;
        currentPerformingCharacter = null;

    }

    void PlayerSubmitTurn()
    {
        InitPlayerBattleStatuses();

        battleMenu.SetActive(true);

        TurnKeyMap.Clear();
        StatsKeyMap.Clear();
        TurnList.Clear();
        FinishedTurnList.Clear();
        TurnItemKeyMap.Clear();
        SpecialKeyMap.Clear();

        state = BattleState.PLAYERTURNSUBMIT;

    }

    void EnemySubmitTurn()
    {
        InitEnemyBattleStatuses();

        battleMenu.SetActive(false);

        state = BattleState.ENEMYTURNSUBMIT;

    }

    public void SetHudHP() => pm0currentHP.text = playerParams.currentHP.ToString();

    private void Update()
    {
       
        switch (state)
        {
            case BattleState.PLAYERTURNSUBMIT:
                if (TurnList.Count == PLAYER_PARTY_MEMBERS_SIZE)
                {
                    state = BattleState.ENEMYTURNSUBMIT;
                    EnemySubmitTurn();

                }
                break;

            case BattleState.ENEMYTURNSUBMIT:
                if (TurnList.Count == PLAYER_PARTY_MEMBERS_SIZE + ENEMY_PARTY_MEMBERS_SIZE)
                {
                    state = BattleState.SUBMISSION;

                }
                break;

            case BattleState.SUBMISSION:
                {
                    TurnList.OrderBy(t => { return StatsKeyMap[t].speed; });
                    

                    state = BattleState.BATTLE;
                }
                break;

            case BattleState.BATTLE:

               if(TurnList.Count == PLAYER_PARTY_MEMBERS_SIZE + ENEMY_PARTY_MEMBERS_SIZE)
                {
                    var turnAction = PerformTurn();

                    var enemyGO = enemyParams.gameObject;

                    var enemyAI = enemyGO.GetComponent<AI_Behavior>();

                }
                else if(TurnList.Count > 0)
                {
                    if(currentPerformingCharacter.BattleStatus == BattleStatus.Done)
                    {
                        var turnAction = PerformTurn();

                        var enemyGO = enemyParams.gameObject;

                        var enemyAI = enemyGO.GetComponent<AI_Behavior>();

                    }
                }
                else
                {
                    if(currentPerformingCharacter != null)
                    {
                        if (currentPerformingCharacter.BattleStatus == BattleStatus.Done)
                        {
                            Debug.Log("Both player and enemy turns are over");
                            // player turn's is over
                            currentPerformingCharacter = null;

                            foreach (var partyMember in FinishedTurnList)
                            {
                                partyMember.OnBattleLoopEnd();
                            }


                            TurnKeyMap.Clear();
                            StatsKeyMap.Clear();
                            TurnList.Clear();
                            FinishedTurnList.Clear();
                            TurnItemKeyMap.Clear();
                            SpecialKeyMap.Clear();

                            PlayerSubmitTurn();
        
                        }

                    }
                }
                break;

        }
    }
}

