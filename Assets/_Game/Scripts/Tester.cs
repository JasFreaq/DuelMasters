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
            _creatureObject.SetupCard(creatureData);
            _spellObject.SetupCard(spellData);

            _battleCardObject.SetupCard(creatureData);
            _creatureManaObject.SetupCard(creatureData);
            _spellManaObject.SetupCard(spellData);
        }
    }
}
