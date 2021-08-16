using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreatureLayoutHandler : CardLayoutHandler
{
    [SerializeField] private GameObject _creatureText;
    [SerializeField] private TextMeshProUGUI _creatureRaceText;
    [SerializeField] private TextMeshProUGUI _powerText;
    [SerializeField] private TextMeshProUGUI _powerTextShade;
    [SerializeField] private GameObject _evolutionCreatureText;
    [SerializeField] private GameObject _evolutionIconImage;
    [SerializeField] private GameObject _vortexEvolutionIconImage;
    [SerializeField] private GameObject _survivorIconImage;
    [SerializeField] private GameObject _waveStrikerIconImage;

    private bool _addPlusToPower;
    private string _originalPowerString;

    public override void SetupCard(CardObject cardObj, CardFrameData cardFrameData)
    {
        base.SetupCard(cardObj, cardFrameData);

        CreatureData creatureData = (CreatureData) cardObj.CardData;

        string raceStr = CardParams.StringFromRace(creatureData.Race[0]);
        if (creatureData.Race.Length == 2)
            raceStr += $" \" {CardParams.StringFromRace(creatureData.Race[1])}";
        _creatureRaceText.text = raceStr;

        _powerText.text = creatureData.Power.ToString();
        _powerTextShade.text = creatureData.Power.ToString();

        if (_addPlusToPower)
        {
            _powerText.text += "+";
            _powerTextShade.text += "+";
        }
    }

    #region Power Adjustment Methods

    public void AddPlusToPower()
    {
        _addPlusToPower = true;
    }

    public void DisplayPowerAttack(int updatedPower)
    {
        _originalPowerString = _powerText.text;
        _powerText.text = updatedPower.ToString();
        _powerTextShade.text = updatedPower.ToString();
    }

    public void ResetPowerAttack()
    {
        _powerText.text = _originalPowerString;
        _powerTextShade.text = _originalPowerString;
    }

    #endregion
}
