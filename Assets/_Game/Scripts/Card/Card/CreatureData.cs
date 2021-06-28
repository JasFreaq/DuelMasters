using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Creature Card", menuName = "Card/Creature", order = 51)]
public class CreatureData : CardData
{
    [SerializeField] private CardParams.Race[] _race = new CardParams.Race[1];
    [SerializeField] private int _power;

    public CardParams.Race[] Race
    {
        get { return _race; }
#if UNITY_EDITOR
        set { _race = value; }
#endif
    }
    
    public int Power
    {
        get { return _power; }
#if UNITY_EDITOR
        set { _power = value; }
#endif
    }
}
