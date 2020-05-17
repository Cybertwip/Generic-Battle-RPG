using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using AssemblyCSharp.Assets.Scripts;
using System.Linq;

public enum PlayerAction { Melee, Special, Item, Defend, None } //@TODO move to self contained enum class
public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleManager : MonoBehaviour
{
    private const int PARTY_MEMBERS_SIZE = 1;

    private Dictionary<IPartyMemberBattleActions, PlayerAction> TurnKeyMap = new Dictionary<IPartyMemberBattleActions, PlayerAction>();
    private List<IPartyMemberBattleActions> TurnList = new List<IPartyMemberBattleActions>();

    private List<string> TurnItemList = new List<string>();

    IPartyMemberBattleActions currentPerformingCharacter;

    public void SubmitTurn(IPartyMemberBattleActions member, PlayerAction action)
    {
        TurnKeyMap[member] = action;
        TurnList.Add(member);
    }

    public void SubmitTurn(IPartyMemberBattleActions member, PlayerAction action, string parameter)
    {
        TurnKeyMap[member] = action;
        TurnList.Add(member);

        switch (action)
        {
            case PlayerAction.Item:
                TurnItemList.Add(parameter);
                break;
        }
    }
    public void PerformTurn()
    {
        var lastCharacter = TurnList.First();
        switch (TurnKeyMap[lastCharacter])
        {
            case PlayerAction.Defend:
                lastCharacter.Defend();
                break;

            case PlayerAction.Item:

                lastCharacter.Item(TurnItemList.First());
                TurnItemList.RemoveAt(0);

                break;

            case PlayerAction.Melee:
                lastCharacter.Melee();
                break;


            case PlayerAction.Special:
                lastCharacter.Special();
                break;
        }

        currentPerformingCharacter = lastCharacter;

        TurnList.Remove(currentPerformingCharacter);
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
        GameObject enemyGo = Instantiate(enemyPrefab, enemySpawnPoint);
        enemyParams = enemyGo.GetComponent<Enemy>();

        // Set the HUD
        SetHudHP();
        pm0maxHP.text = playerParams.maxHP.ToString();

        yield return new WaitForSeconds(1.0f);

        state = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    void InitBattleStatuses()
    {
        var playerGO = playerParams.gameObject;

        // battle states
        //@TODO do this for each party member, also move LuigiAnimEvents to
        // own player's PartyMember inherited script
        var playerBattleState = playerGO.GetComponent<LuigiAnimEvents>() as IPartyMemberBattleActions;

        playerBattleState.BattleStatus = BattleStatus.Idle;
        currentPerformingCharacter = null;

    }

    void PlayerTurn()
    {
        InitBattleStatuses();

        battleMenu.SetActive(true);

        TurnKeyMap.Clear();
        TurnList.Clear();

        // player chooses an action from a button
        //
    }

    void EnemyTurn()
    {
        battleMenu.SetActive(false);
        PlayerTurn();
    }

    public void SetHudHP() => pm0currentHP.text = playerParams.currentHP.ToString();

    private void Update()
    {
        switch (state)
        {
            case BattleState.PLAYERTURN:
                if(TurnList.Count == PARTY_MEMBERS_SIZE)
                {
                    PerformTurn();
                }
                else if(TurnList.Count > 0)
                {
                    if(currentPerformingCharacter.BattleStatus == BattleStatus.Done)
                    {
                        PerformTurn();
                    }
                }
                else
                {
                    if(currentPerformingCharacter != null)
                    {
                        if (currentPerformingCharacter.BattleStatus == BattleStatus.Done)
                        {
                            Debug.Log("Player turn is over");
                            // player turn's is over
                            currentPerformingCharacter = null;
                            EnemyTurn();
                        }

                    }
                }
                break;
        }
    }
}

