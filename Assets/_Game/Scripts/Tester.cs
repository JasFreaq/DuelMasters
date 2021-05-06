using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
    [SerializeField] private CreatureObject _creatureObject;
    [SerializeField] private SpellObject _spellObject;
    [SerializeField] private Creature _creature;
    [SerializeField] private Spell _spell;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _creatureObject.SetupCard(_creature);
            _spellObject.SetupCard(_spell);
        }
    }
}
