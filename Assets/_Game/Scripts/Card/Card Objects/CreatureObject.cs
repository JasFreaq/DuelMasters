using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreatureObject : CardObject
{
    [SerializeField] private GameObject _creatureText;
    [SerializeField] private TextMeshProUGUI _creatureRaceText;
    [SerializeField] private TextMeshProUGUI _powerText;
    [SerializeField] private GameObject _evolutionCreatureText;
    [SerializeField] private GameObject _evolutionIconImage;
    [SerializeField] private GameObject _vortexEvolutionIconImage;
    [SerializeField] private GameObject _survivorIconImage;
    [SerializeField] private GameObject _waveStrikerIconImage;

    public override void SetupCard(CardData cardData)
    {
        base.SetupCard(cardData);

        CreatureData creatureData = (CreatureData) cardData;

        string raceStr = CardParams.StringFromRace(creatureData.Race[0]);
        if (creatureData.Race.Length == 2)
            raceStr += $" \" {CardParams.StringFromRace(creatureData.Race[1])}";
        _creatureRaceText.text = raceStr;

        _powerText.text = creatureData.Power.ToString();
    }

    protected override void SetupRules(string rulesText)
    {
        base.SetupRules(rulesText);
    }
}
