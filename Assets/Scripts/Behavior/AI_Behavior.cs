using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public interface IsEnemy
{

}

public abstract class AI_Behavior : Intelligence, IsEnemy
{
    protected Enemy enemyStatus;

    private bool submittedTurn = false;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        enemyStatus = gameObject.GetComponent<Enemy>();
    }

    /*
    public virtual void OnPlayerTurnPerformed(IPartyMemberBattleActions sender,
                                              PlayerAction action)
    {
        playerActionsHistory.Add(new KeyValuePair<IPartyMemberBattleActions,
                                                  PlayerAction>(sender,
                                                                action));
    } */

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        if(battleManager.state == BattleState.ENEMYTURNSUBMIT)
        {
            if (!submittedTurn && Alive)
            {
                OnTurnSubmit();

                submittedTurn = true;

            }
        }

    }

    public abstract void OnDamageReceived(int amount);
    protected abstract void OnTurnSubmit();

    public override void OnBattleLoopEnd()
    {
        submittedTurn = false;
    }
}
