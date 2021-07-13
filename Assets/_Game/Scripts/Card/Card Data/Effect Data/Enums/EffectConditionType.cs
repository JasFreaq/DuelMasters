using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum EffectConditionType
{
    WhenPutIntoBattle,
    While,
    WhenAttacking,
    WhenBlocking,
    OnShieldTrigger,
    AfterBattle,
    WhenDestroyed,
    AtEndOfTurn,
    UntilEndOfTurn
}
