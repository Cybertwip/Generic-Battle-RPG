using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyMember : MonoBehaviour, Stats
{
    public string partyMemberName;

    public int level { get; set; }
    public int damage { get; set; }
    public int maxHP { get; set; }
    public int maxFP { get; set; }
    public int currentHP { get; set; }
    public int currentFP { get; set; }
    public int speed { get; set; }
}