using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Thwomp : MonoBehaviour
{
    public Animator animator;
    public Enemy enemyStatus;
    void Start()
    {
        
    }

    void Update()
    {
        // testing some animations, was testing the Behaviours, big waste of time
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_Pout")) animator.SetTrigger("trigger_grimmace");
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_Grimmace")) animator.SetTrigger("trigger_return_to_pout");
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
