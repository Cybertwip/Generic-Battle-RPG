using System;
using UnityEngine;

public enum BattleStatus { Performing, Done, Idle }

public abstract class PartyMemberBattleActions: MonoBehaviour
{
    public BattleStatus BattleStatus { get; set; }

    public abstract void Special(SpecialAttack.Attack attack);
    public abstract void Melee();
    public abstract void Defend();
    public abstract void Item(string name);

    public abstract void OnBattleLoopEnd();

    //void TakeDamage(int totalDamage);

}
