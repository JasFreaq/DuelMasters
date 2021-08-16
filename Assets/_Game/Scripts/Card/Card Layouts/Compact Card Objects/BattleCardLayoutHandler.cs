using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleCardLayoutHandler : CompactCardLayoutHandler
{
    [SerializeField] private TextMeshProUGUI _creatureRaceText;
    [SerializeField] private TextMeshProUGUI _powerText;
    [SerializeField] private TextMeshProUGUI _powerTextShade;

    private bool _addPlusToPower;
    private string _originalPowerString;

    public override void SetupCard(CardData cardData, CompactCardFrameData compactFrameData)
    {
        base.SetupCard(cardData, compactFrameData);

        CreatureData creatureData = (CreatureData) cardData;

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
