using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AI_Thwomp : AI_Behavior
{

    private enum PerformingStatus
    {
        Attacking,
        Special,
        None
    }

    private enum AttackStatus
    {
        Start,
        Performing,
        End,
        None
    }

    private enum BiteStatus
    {
        Performing,
        End,
        None
    }

    bool biteFlag;

    PerformingStatus status = PerformingStatus.None;
    AttackStatus attackStatus = AttackStatus.None;
    BiteStatus biteStatus = BiteStatus.None;
    

    private int ticks = 0;

    private bool performingDamage;

    GameObject target;

    protected override void Start()
    {
        base.Start();
        target = null;
    }

    protected override void Update()
    {
        base.Update();
        // testing some animations, was testing the Behaviours, big waste of time


        if(BattleStatus == BattleStatus.Performing)
        {
            switch (status)
            {
                case PerformingStatus.Attacking:
                    switch (attackStatus)
                    {
                        case AttackStatus.None:
                            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_Pout"))
                            {
                                AnimTrigger("trigger_grimmace");
                            }
                            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_Grimmace"))
                            {
                                AnimTrigger("trigger_bite");
                                attackStatus = AttackStatus.Start;
                            }
                            break;

                        case AttackStatus.Start:
                            attackStatus = AttackStatus.Performing;
                            break;

                        case AttackStatus.Performing:


                            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
                            {
                                if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
                                {
                                    AnimTrigger("trigger_wait_00");
                                }
                                
                            }
                            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Wait_00"))
                            {
                                AnimTrigger("trigger_bite_forward");
                            }

                            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Bite_Forward"))
                            {
                                float t = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                                if (t >= 0 && t >= 40f / 59f && t <= 45f / 59f)
                                {
                                    if (!performingDamage)
                                    {
                                        performingDamage = true;

                                        var partyMember = target.GetComponent<PartyMember>();
                                        partyMember.currentHP -= 10;
                                    }

                                    biteFlag = true;
                                }
                                else if(t >= 1)
                                {
                                    AnimTrigger("trigger_wait_01");
                                }


                            }
                            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_Grimmace"))
                            {
                                if(biteStatus == BiteStatus.None)
                                {
                                    biteStatus = BiteStatus.Performing;
                                }
                                else if(biteStatus == BiteStatus.Performing)
                                {
                                    if (biteFlag)
                                    {
                                        biteStatus = BiteStatus.End;
                                    }
                                }
                                else
                                {
                                    attackStatus = AttackStatus.End;
                                }
                                
                                
                            }
                            break;

                        case AttackStatus.End:
                            attackStatus = AttackStatus.None;
                            BattleStatus = BattleStatus.Done;
                            biteStatus = BiteStatus.None;
                            biteFlag = false;
                            performingDamage = false;
                            target = null;
                            break;
                    }



                    break;

                case PerformingStatus.Special:

                    break;
            }

        }
        else
        {
            ticks = 0;

            /*
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_Pout")) 
                animator.SetTrigger("trigger_grimmace");
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_Grimmace")) animator.SetTrigger("trigger_return_to_pout"); */

        }

    }

    protected override void OnTurnSubmit()
    {
        int randomPlayerIndex = Random.Range(0, battleManager.PlayerParty.Count);
        var playerParty = battleManager.PlayerParty.ToList();

        target = playerParty[randomPlayerIndex].Value;

        if(Random.Range(0, 120) <= 80)
        {
            battleManager.SubmitTurn(this, PlayerAction.Melee);
        }
        else
        {
            battleManager.SubmitTurn(this, PlayerAction.Melee);
        }
    }

    public override void Special(SpecialAttack.Attack attack)
    {
        BattleStatus = BattleStatus.Performing;
        status = PerformingStatus.Special;
    }

    public override void Melee()
    {
        BattleStatus = BattleStatus.Performing;
        status = PerformingStatus.Attacking;
    }

    public override void Defend()
    {
        BattleStatus = BattleStatus.Performing;
    }

    public override void Item(string name)
    {
        throw new System.NotImplementedException();
    }

    public override void OnBattleLoopEnd()
    {
        base.OnBattleLoopEnd();

        BattleStatus = BattleStatus.Idle;

        ticks = 0;
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

// Thwomps available actions are 

// LAUGH, QUAKE, THWOMP, BITE, EAT, TANTRUM, HEAVE

/*  if (last player action was an attack that did no damage)
    {
        Laugh();
        pity_the_fool = true;
    }

    randF = GetRandFloat(); //0-1f

    if (HP > enemyStatus.MaxHP*0.5f)
    {
        // 25% LAUGH, 37.5% QUAKE, 37.5% THWOMP
        if (randF < 0.250f)
        {
            if (pity_the_fool == false) Laugh(); // no effect
            else pity_the_fool = false; // doesn't laugh twice in a row, resets flag
        }
    else if (randomInt < 0.625f) Quake(); // inflicts FEAR status
    else Thwomp(); // -50 HP
    }

    else // Thwomp's HP <= enemyStatus.MaxHP*0.5f
    {
        // 5% EAT, 10% BITE, 20% LAUGH, 30% HEAVE/TANTRUM, 35% THWOMP
        if (randF < 0.05f) Eat(); // removes party member, refills Thwomp's MP, instant KO.
        else if (randF < 0.15f) Bite(); // Thwomp gains 10 FP, player loses 10 FP and reduces player's HP to MathF.Max(Mathf.RoundToInt((PartyMember.MaxHP * 0.1f)), 1)
        else if (randF < 0.35f)
        {
            if (pity_the_fool == false) Laugh(); // no effect
            else pity_the_fool = false; // doesn't laugh twice in a row, resets flag
        }
    }
    else if (randF < 0.65f)
    {
        if (player's last turn was a magic attack) Heave() // Thwomp huffs-and-puffs, takes 2 turns for this this turn and the next.
        else Tantrum(); // inficts FEAR status and random damage to all party members
    }
    else Thwomp(); // -50 HP
}
*/
