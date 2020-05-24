using System;
namespace AssemblyCSharp.Assets.Scripts
{
    public enum BattleStatus { Performing, Done, Idle }

    public interface IPartyMemberBattleActions
    {
        BattleStatus BattleStatus { get; set; }

        UnityEngine.Events.UnityAction Special(SpecialAttack.Attack attack);
        UnityEngine.Events.UnityAction Melee();
        UnityEngine.Events.UnityAction Defend();
        UnityEngine.Events.UnityAction Item(string name);

        UnityEngine.Events.UnityAction OnBattleLoopEnd();

        //void TakeDamage(int totalDamage);

    }
}
