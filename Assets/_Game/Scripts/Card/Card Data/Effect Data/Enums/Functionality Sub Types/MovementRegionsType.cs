using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementRegionsType
{
    //Deck To
    Draw,
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
    ShieldsToHand,
    ShieldsToGraveyard,
    ShieldsToMana,
    ShieldsToBattle,

    //GraveyardTo
    GraveyardToDeck,
    GraveyardToHand,
    GraveyardToShields,
    GraveyardToMana,
    GraveyardToBattle,

    //Mana To
    ManaToDeck,
    ManaToHand,
    ManaToShields,
    ManaToGraveyard,
    ManaToBattle,

    //Battle To
    BattleToDeck,
    BattleToHand,
    BattleToShields,
    BattleToGraveyard,
    BattleToMana
}
