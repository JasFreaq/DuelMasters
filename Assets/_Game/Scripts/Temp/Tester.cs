using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    [SerializeField] private CreatureLayoutHandler creatureLayoutHandler;
    [SerializeField] private SpellLayoutHandler spellLayoutHandler;
    [SerializeField] private BattleCardLayoutHandler battleCardLayoutHandler;
    [SerializeField] private CreatureManaLayoutHandler creatureManaLayoutHandler;
    [SerializeField] private SpellManaLayoutHandler spellManaSpellManaObject;
    [SerializeField] private CreatureData creatureData;
    [SerializeField] private SpellData spellData;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (creatureLayoutHandler)
                creatureLayoutHandler.SetupCard(creatureData);
            if (spellLayoutHandler)
                spellLayoutHandler.SetupCard(spellData);

            if (battleCardLayoutHandler)
                battleCardLayoutHandler.SetupCard(creatureData);
            if (creatureManaLayoutHandler)
                creatureManaLayoutHandler.SetupCard(creatureData);
            if (spellManaSpellManaObject)
                spellManaSpellManaObject.SetupCard(spellData);
        }
    }
}
