using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum EffectConditionType
{
    WhenPutIntoBattle,
    WhenDestroyed,
    OnShieldTrigger,
    AtEndOfTurn,
    WhenBlocks,
    WhenAttacks,
    AfterBattle,
    WhileAll
}
