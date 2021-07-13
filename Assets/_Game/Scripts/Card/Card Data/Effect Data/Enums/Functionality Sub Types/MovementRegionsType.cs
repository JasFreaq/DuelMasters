using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementRegionsType
{
    Draw,
    ReturnToHand,

    //Deck To
    DeckToShields,
    DeckToGraveyard,
    DeckToMana,
    DeckToBattle,

    //Hand To
    HandToDeck,
    HandToShields,
    HandToGraveyard,
    HandToMana,
    HandToBattle,

    //Shields To
    ShieldsToDeck,
    ShieldsToGraveyard,
    ShieldsToMana,
    ShieldsToBattle,

    //GraveyardTo
    GraveyardToDeck,
    GraveyardToShields,
    GraveyardToMana,
    GraveyardToBattle,

    //Mana To
    ManaToDeck,
    ManaToShields,
    ManaToGraveyard,
    ManaToBattle,

    //Battle To
    BattleToDeck,
    BattleToShields,
    BattleToGraveyard,
    BattleToMana
}
