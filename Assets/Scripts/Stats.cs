using UnityEngine;
using System.Collections;

public interface Stats
{
    int level { get; set; }
    int damage { get; set; }

    int maxHP { get; set; }
    int maxFP { get; set; }
    int currentHP { get; set; }
    int currentFP { get; set; }

    int speed { get; set; }
}
