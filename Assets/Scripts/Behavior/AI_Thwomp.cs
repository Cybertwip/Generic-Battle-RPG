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

    private enum ThwompStatus
    {
        RisePrepare,
        Rise,
        LocatePrepare,
        Locate,
        ThwompPrepare,
        Thwomp,
        RiseBackPrepare,
        RiseBack,
        ReturnToBasePrepare,
        ReturnToBase,
        RiseDownFinalPrepare,
        RiseDownFinal,
        End,
        None
    }

    private enum BiteStatus
    {
        Start,
        Performing,
        End,
        None
    }

    private enum SpecialAttacks
    {
        Tantrum,
        Laugh
    }

    private enum MeleeAttakcs
    {
        Bite,
        Thwomp,
        None
    }

    private enum BitePerformingStatus
    {
        Performing,
        End,
        None
    }

    bool biteFlag;

    PerformingStatus status = PerformingStatus.None;
    BiteStatus biteStatus = BiteStatus.None;
    BitePerformingStatus bitePerformingStatus = BitePerformingStatus.None;
    ThwompStatus thwompStatus = ThwompStatus.None;

    MeleeAttakcs currentMeleeAttack = MeleeAttakcs.None;

    bool transformFlag;

    bool lerpFlag;

    private int waitTicks = 0;
    private int ticks = 0;

    Vector3 targetLerpPosition;
    Vector3 initialPosition;

    private bool performingDamage;

    GameObject target;

    protected override void Start()
    {
        base.Start();
        target = null;
        initialPosition = transform.position;
    }

    float lerpTime;

    private void LerpOverTime(Vector3 start, Vector3 end, float duration)
    {
        if (lerpTime <= duration)
        {
            lerpTime += Time.deltaTime;
            float percent = Mathf.Clamp01(lerpTime / duration);
            transform.position = Vector3.Lerp(start, end, percent);
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        // testing some animations, was testing the Behaviours, big waste of time


        if(BattleStatus == BattleStatus.Performing)
        {
            switch (status)
            {
                case PerformingStatus.Attacking:

                    if(currentMeleeAttack == MeleeAttakcs.Bite)
                    {
                        switch (biteStatus)
                        {
                            case BiteStatus.None:
                                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_Pout"))
                                {
                                    AnimTrigger("trigger_grimmace");
                                }
                                else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_Grimmace"))
                                {
                                    AnimTrigger("trigger_bite");
                                    biteStatus = BiteStatus.Start;
                                }
                                break;

                            case BiteStatus.Start:
                                biteStatus = BiteStatus.Performing;
                                break;

                            case BiteStatus.Performing:


                                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
                                {
                                    if ((animator.GetCurrentAnimatorStateInfo(0).normalizedTime) < 10f / 48f)
                                    {
                                        lerpFlag = false;
                                        lerpTime = 0;
                                    }
                                    else if ((animator.GetCurrentAnimatorStateInfo(0).normalizedTime) >= 10f / 48f &&
                                        (animator.GetCurrentAnimatorStateInfo(0).normalizedTime) <= 16f / 48f)
                                    {
                                        if (!lerpFlag)
                                        {
                                            lerpFlag = true;
                                            targetLerpPosition = new Vector3(gameObject.transform.position.x,
                                                                      gameObject.transform.position.y,
                                                                      gameObject.transform.position.z - 1.8117333f / 2f);
                                        }


                                        LerpOverTime(gameObject.transform.position, targetLerpPosition, 6f / 60f);

                                    }
                                    else if ((animator.GetCurrentAnimatorStateInfo(0).normalizedTime) < 34f / 48f)
                                    {
                                        lerpFlag = false;
                                        lerpTime = 0;
                                    }
                                    else if ((animator.GetCurrentAnimatorStateInfo(0).normalizedTime) >= 34f / 48f &&
                                            (animator.GetCurrentAnimatorStateInfo(0).normalizedTime) <= 40f / 48f)
                                    {
                                        if (!lerpFlag)
                                        {
                                            lerpFlag = true;
                                            targetLerpPosition = new Vector3(gameObject.transform.position.x,
                                                                      gameObject.transform.position.y,
                                                                      gameObject.transform.position.z - 1.8117333f / 2f);
                                        }

                                        LerpOverTime(gameObject.transform.position, targetLerpPosition, 6f / 60f);
                                    }
                                    else if ((animator.GetCurrentAnimatorStateInfo(0).normalizedTime) >= 1f)
                                    {

                                        if (!transformFlag)
                                        {
                                            AnimTrigger("trigger_wait_00");

                                            transformFlag = true;
                                            /*
                                            var newZ = gameObject.transform.GetChild(1).transform.position.z;
                                            var newPosition = new Vector3(gameObject.transform.position.x,
                                                                          gameObject.transform.position.y,
                                                                          newZ);

                                            gameObject.transform.position = newPosition;*/

                                        }


                                    }
                                }
                                else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Wait_00"))
                                {
                                    AnimTrigger("trigger_bite_forward");
                                    transformFlag = false;
                                }
                                else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Walk_R"))
                                {
                                    if ((animator.GetCurrentAnimatorStateInfo(0).normalizedTime) < 10f / 48f)
                                    {
                                        lerpFlag = false;
                                        lerpTime = 0;
                                    }
                                    else if ((animator.GetCurrentAnimatorStateInfo(0).normalizedTime) >= 10f / 48f &&
                                        (animator.GetCurrentAnimatorStateInfo(0).normalizedTime) <= 16f / 48f)
                                    {
                                        if (!lerpFlag)
                                        {
                                            lerpFlag = true;
                                            targetLerpPosition = new Vector3(gameObject.transform.position.x,
                                                                      gameObject.transform.position.y,
                                                                      gameObject.transform.position.z + 1.8117333f / 2f);
                                        }


                                        LerpOverTime(gameObject.transform.position, targetLerpPosition, 6f / 60f);

                                    }
                                    else if ((animator.GetCurrentAnimatorStateInfo(0).normalizedTime) < 34f / 48f)
                                    {
                                        lerpFlag = false;
                                        lerpTime = 0;
                                    }
                                    else if ((animator.GetCurrentAnimatorStateInfo(0).normalizedTime) >= 34f / 48f &&
                                            (animator.GetCurrentAnimatorStateInfo(0).normalizedTime) <= 40f / 48f)
                                    {
                                        if (!lerpFlag)
                                        {
                                            lerpFlag = true;
                                            targetLerpPosition = new Vector3(gameObject.transform.position.x,
                                                                      gameObject.transform.position.y,
                                                                      gameObject.transform.position.z + 1.8117333f / 2f);
                                        }

                                        LerpOverTime(gameObject.transform.position, targetLerpPosition, 6f / 60f);
                                    }

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
                                    else if (t >= 1)
                                    {
                                        AnimTrigger("trigger_wait_01");
                                    }


                                }
                                else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_Grimmace"))
                                {
                                    if (bitePerformingStatus == BitePerformingStatus.None)
                                    {
                                        bitePerformingStatus = BitePerformingStatus.Performing;
                                    }
                                    else if (bitePerformingStatus == BitePerformingStatus.Performing)
                                    {
                                        if (biteFlag)
                                        {
                                            bitePerformingStatus = BitePerformingStatus.End;
                                        }
                                    }
                                    else
                                    {
                                        biteStatus = BiteStatus.End;
                                    }


                                }
                                break;

                            case BiteStatus.End:
                                biteStatus = BiteStatus.None;
                                BattleStatus = BattleStatus.Done;
                                bitePerformingStatus = BitePerformingStatus.None;
                                biteFlag = false;
                                performingDamage = false;
                                transformFlag = false;
                                target = null;
                                currentMeleeAttack = MeleeAttakcs.None;
                                break;
                        }
                    }
                    else if(currentMeleeAttack == MeleeAttakcs.Thwomp)
                    {
                        switch (thwompStatus)
                        {
                            case ThwompStatus.None:
                               if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_Pout"))
                                {
                                    thwompStatus = ThwompStatus.RisePrepare;
                                }
                                break;

                            case ThwompStatus.RisePrepare:
                                thwompStatus = ThwompStatus.Rise;
                                AnimTrigger("trigger_thwomp");
                                lerpTime = 0;
                                targetLerpPosition = new Vector3(transform.position.x, transform.position.y + 4, transform.position.z);
                                break;

                            case ThwompStatus.Rise:
                                LerpOverTime(transform.position, targetLerpPosition, 16f / 60f);

                                if (transform.position.y == targetLerpPosition.y)
                                {
                                    thwompStatus = ThwompStatus.LocatePrepare;
                                }

                                break;

                            case ThwompStatus.LocatePrepare:
                                
                                if(waitTicks >= 16)
                                {
                                    waitTicks = 0;
                                    lerpTime = 0;
                                    targetLerpPosition = new Vector3(transform.position.x, transform.position.y, target.transform.position.z);
                                    thwompStatus = ThwompStatus.Locate;
                                }
                                else
                                {
                                    waitTicks++;
                                }

                                
                                break;

                            case ThwompStatus.Locate:
                                LerpOverTime(transform.position, targetLerpPosition, 16f / 60f);

                                if (transform.position.z == targetLerpPosition.z)
                                {
                                    thwompStatus = ThwompStatus.ThwompPrepare;
                                }
                                break;

                            case ThwompStatus.ThwompPrepare:

                                if (waitTicks >= 30)
                                {
                                    waitTicks = 0;
                                    lerpTime = 0;
                                    AnimTrigger("trigger_thwomp_grimmace");
                                    targetLerpPosition = new Vector3(transform.position.x, transform.position.y - 4, transform.position.z);
                                    thwompStatus = ThwompStatus.Thwomp;
                                }
                                else
                                {
                                    waitTicks++;
                                }
                                break;

                            case ThwompStatus.Thwomp:
                                LerpOverTime(transform.position, targetLerpPosition, 10f / 60f);

                                if (transform.position.y == targetLerpPosition.y)
                                {
                                    var partyMember = target.GetComponent<PartyMember>();
                                    partyMember.currentHP -= 10;

                                    thwompStatus = ThwompStatus.RiseBackPrepare;
                                }
                                break;

                            case ThwompStatus.RiseBackPrepare:
                                if (waitTicks >= 60)
                                {
                                    waitTicks = 0;
                                    lerpTime = 0;
                                    AnimTrigger("trigger_thwomp_grimmace_r");
                                    targetLerpPosition = new Vector3(transform.position.x, transform.position.y + 4, transform.position.z);
                                    thwompStatus = ThwompStatus.RiseBack;
                                }
                                else
                                {
                                    waitTicks++;
                                }
                                break;

                            case ThwompStatus.RiseBack:

                                LerpOverTime(transform.position, targetLerpPosition, 16f / 60f);

                                if (transform.position.y == targetLerpPosition.y)
                                {
                                    thwompStatus = ThwompStatus.ReturnToBasePrepare;
                                    AnimTrigger("trigger_thwomp_grimmace_r_exit");
                                }
                                break;

                            case ThwompStatus.ReturnToBasePrepare:
                                if (waitTicks >= 30)
                                {
                                    waitTicks = 0;
                                    lerpTime = 0;
                                    targetLerpPosition = new Vector3(transform.position.x, transform.position.y, initialPosition.z);
                                    thwompStatus = ThwompStatus.ReturnToBase;
                                }
                                else
                                {
                                    waitTicks++;
                                }
                                break;
                            case ThwompStatus.ReturnToBase:

                                LerpOverTime(transform.position, targetLerpPosition, 16f / 60f);

                                if (transform.position.z == targetLerpPosition.z)
                                {
                                    thwompStatus = ThwompStatus.RiseDownFinalPrepare;
                                }
                                break;

                            case ThwompStatus.RiseDownFinalPrepare:
                                if (waitTicks >= 30)
                                {
                                    waitTicks = 0;
                                    lerpTime = 0;
                                    targetLerpPosition = new Vector3(transform.position.x, initialPosition.y, initialPosition.z);
                                    thwompStatus = ThwompStatus.RiseDownFinal;
                                }
                                else
                                {
                                    waitTicks++;
                                }

                                break;

                            case ThwompStatus.RiseDownFinal:
                                LerpOverTime(transform.position, targetLerpPosition, 16f / 60f);

                                if (transform.position.y == targetLerpPosition.y)
                                {
                                    AnimTrigger("trigger_thwomp_exit");
                                    thwompStatus = ThwompStatus.None;
                                    BattleStatus = BattleStatus.Done;
                                    target = null;
                                    currentMeleeAttack = MeleeAttakcs.None;
                                }
                                break;
                        }
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

        currentMeleeAttack = MeleeAttakcs.Thwomp;//(MeleeAttakcs)Random.Range(0, 2);
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
