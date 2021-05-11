using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    [SerializeField] private CreatureObject _creatureObject;
    [SerializeField] private SpellObject _spellObject;
    [SerializeField] private BattleCardObject _battleCardObject;
    [SerializeField] private CreatureManaCardObject _creatureManaObject;
    [SerializeField] private SpellManaCardObject _spellManaObject;
    [SerializeField] private CreatureData creatureData;
    [SerializeField] private SpellData spellData;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_creatureObject)
                _creatureObject.SetupCard(creatureData);
            if (_spellObject)
                _spellObject.SetupCard(spellData);

            if (_battleCardObject)
                _battleCardObject.SetupCard(creatureData);
            if (_creatureManaObject)
                _creatureManaObject.SetupCard(creatureData);
            if (_spellManaObject)
                _spellManaObject.SetupCard(spellData);
        }
    }
}
