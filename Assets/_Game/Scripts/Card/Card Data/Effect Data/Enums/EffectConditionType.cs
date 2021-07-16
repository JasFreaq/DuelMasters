using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum EffectConditionType
{
    WhenPutIntoBattle,
    WhenCast,
    WhileCondition,
    WhenAttacking,
    WhenBlocking,
    WhenYourCreatureIsBlocked,
    OnShieldTrigger,
    AfterBattle,
    WhenWouldBeDestroyed,
    WhenDestroyed,
    AtEndOfTurn,
    UntilEndOfTurn,
    WhileTapState,
    CheckFunction
}
