using System;
using UnityEngine;

public enum BattleStatus { Performing, Done, Idle }
public enum BattleActions { Special, Melee, Defend, Item }

public abstract class PartyMemberBattleActions: MonoBehaviour
{
    public BattleStatus BattleStatus { get; set; }

    public BattleActions LastPerformedAction { get; set; }

    public bool Alive { get; set; }

    public abstract void Special(SpecialAttack.Attack attack);
    public abstract void Melee();
    public abstract void Defend();
    public abstract void Item(string name);

    public abstract void OnRevived();
    public abstract void OnDeath();
    public abstract void OnBattleLoopEnd();

    //void TakeDamage(int totalDamage);

}
