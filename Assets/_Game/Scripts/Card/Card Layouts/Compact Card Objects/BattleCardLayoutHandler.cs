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

    public void UpdatePower(int updatedPower)
    {
        string powerString = updatedPower.ToString();
        _powerText.text = powerString;
        _powerTextShade.text = powerString;
    }

    public void ResetPower(int resetPower)
    {
        string powerString = resetPower.ToString();
        _powerText.text = powerString;
        _powerTextShade.text = powerString;
    }

    #endregion
}
